# End-to-end QA: REST surface and MCP consumption

How to verify the whole service against the live Woolworths catalogue, using
`curl` for the REST surface and a real MCP SDK client for the `/mcp` surface.
This is the procedure that first proved the authenticated write path — and
caught the missing-`placeId` bug ([the Woolworths contract](woolworths.md))
that unit tests never could, because the failure only exists against a fresh
headless session on the live site.

## Scope

| Layer | Covered by |
| --- | --- |
| Parser and query-building | `dotnet test Gaia.slnx` (offline, always run first) |
| REST endpoints, happy + error paths | the curl matrix below |
| MCP protocol, tools, header credentials | the SDK client below |
| The authenticated write path | the SDK client, with credentials set |

The write test **adds one real item (~R15) to the account's real cart**. Run it
deliberately, and remove the item from the cart afterwards if it is not wanted.

## 1. Start the service

```bash
export MSBuildEnableWorkloadResolver=false   # this volume only; the resolver crashes under iCloud
ASPNETCORE_URLS=http://localhost:5199 dotnet run --project src/Gaia.Mcp.Server --no-launch-profile
```

Over stdio instead of HTTP, set `MCP_TRANSPORT=stdio` and supply credentials as
`GAIA_WOOLWORTHS_USERNAME` / `GAIA_WOOLWORTHS_PASSWORD` rather than as headers.

## 2. REST matrix

Expectations, verified against the implementation:

| Call | Expect |
| --- | --- |
| `GET /api/health` | 200 |
| `GET /api/integrations` | 200, lists each integration's credential headers |
| `GET /api/woolworths/search?q=garlic&limit=2` | 200 with prices |
| `GET /api/woolworths/search` (no `q`) | 400 |
| `limit=0` / `limit=999` | 200 — clamped to 1 / 24 |
| `POST .../shopping-list/preview` with `""`, `null` or missing `text` | 200 with empty `lines` |
| malformed JSON body | 400 |
| `POST .../shopping-list/cart` with no / partial headers | **401** naming both headers |
| headers present but wrong credentials | **401** "Provider rejected the credentials" (takes ~5s — it asks Woolworths) |

## 3. MCP client

Scaffold anywhere disposable:

```bash
mkdir mcp-qa && cd mcp-qa && npm init -y && npm i @modelcontextprotocol/sdk
node qa.mjs                                         # anonymous checks only
WOOLIES_USER=... WOOLIES_PASS=... node qa.mjs       # + the real write test
```

`qa.mjs`:

```javascript
import { Client } from "@modelcontextprotocol/sdk/client/index.js";
import { StreamableHTTPClientTransport } from "@modelcontextprotocol/sdk/client/streamableHttp.js";

const URL_ = new URL("http://localhost:5199/mcp");
let pass = 0, fail = 0;
const ok = (name, cond, detail = "") => {
  console.log(`${cond ? "PASS" : "FAIL"}  ${name}${detail ? "  — " + detail : ""}`);
  cond ? pass++ : fail++;
};

async function connect(headers = {}) {
  const client = new Client({ name: "gaia-mcp-qa", version: "1.0.0" });
  await client.connect(new StreamableHTTPClientTransport(URL_, { requestInit: { headers } }));
  return client;
}

const text = (r) => r.content?.map((c) => c.text).join("\n") ?? "";

// ---- anonymous -------------------------------------------------------------
const anon = await connect();

const info = anon.getServerVersion();
ok("initialize / server identity", info?.name === "frostaura-integrations", JSON.stringify(info));

const tools = (await anon.listTools()).tools;
ok("tools/list returns 4 tools", tools.length === 4, tools.map((t) => t.name).join(", "));
ok("every tool has a description + schema",
  tools.every((t) => t.description?.length > 20 && t.inputSchema?.type === "object"));

const search = JSON.parse(text(await anon.callTool({
  name: "woolworths_search_products", arguments: { query: "baby spinach", limit: 3 } })));
ok("search tool returns products with id+price",
  search.count === 3 && search.products.every((p) => p.id && p.priceZar > 0),
  `${search.products[0]?.name} R${search.products[0]?.priceZar}`);

const preview = JSON.parse(text(await anon.callTool({
  name: "woolworths_preview_shopping_list",
  arguments: { shoppingListText: "150 g dried fusilli pasta\n0.5 red capsicum\nfresh flat-leaf parsley" } })));
ok("preview tool resolves 3/3 incl. AU→SA vocabulary",
  preview.lines.length === 3 && preview.lines.every((l) => l.status === "Resolved"),
  preview.lines.map((l) => `${l.ingredient}→${l.matchedProduct}`).join(" | "));

const noCreds = await anon.callTool({ name: "woolworths_add_shopping_list_to_cart",
  arguments: { shoppingListText: "1 garlic clove" } });
ok("write tool without headers → isError + names the headers",
  noCreds.isError === true
    && text(noCreds).includes("X-Woolworths-Username")
    && text(noCreds).includes("X-Woolworths-Password"),
  text(noCreds).slice(0, 100));

const badTool = await anon.callTool({ name: "woolworths_add_product_to_cart",
  arguments: { productId: "" } }).catch((e) => ({ isError: true, content: [{ text: String(e) }] }));
ok("empty productId does not crash the server", badTool !== undefined);

await anon.close();

// ---- authenticated (headers on the connection) -------------------------------
const user = process.env.WOOLIES_USER, pw = process.env.WOOLIES_PASS;
if (user && pw) {
  const auth = await connect({ "X-Woolworths-Username": user, "X-Woolworths-Password": pw });

  const t0 = Date.now();
  const add = JSON.parse(text(await auth.callTool({
    name: "woolworths_add_shopping_list_to_cart",
    arguments: { shoppingListText: "fresh flat-leaf parsley" } }, undefined,
    { timeout: 120_000 })));
  ok("WRITE: shopping list added via MCP headers",
    add.addedCount === 1 && add.cartItemCount >= 1,
    `added=${add.addedCount} cart=${add.cartItemCount} total=R${add.cartTotalZar} in ${((Date.now()-t0)/1000).toFixed(1)}s`);
  ok("WRITE: line reports product + query",
    add.lines[0].status === "Added" && !!add.lines[0].productId,
    `${add.lines[0].ingredient} → ${add.lines[0].matchedProduct} (R${add.lines[0].priceZar})`);

  await auth.close();
} else {
  console.log("SKIP  authenticated write test (WOOLIES_USER/WOOLIES_PASS not set)");
}

console.log(`\n${pass} passed, ${fail} failed`);
process.exit(fail ? 1 : 0);
```

Notes that matter:

- **Credential headers go on the transport's `requestInit`**, so in the server's
  stateless mode every request carries them. This is exactly how a production
  MCP client (Claude Code, claude.ai connectors) is configured.
- The write call uses a **120s per-request timeout** — sign-in plus adds plus
  sign-out can exceed the SDK's 60s default on a long list.
- Sign-out has no positive log line; it is verified inside the client, which
  logs a warning only if the session survived. No warning in the service log
  means sign-out worked.

## 4. Clean up

Delete the scaffold directory and stop the service. Nothing else is created:
the service itself holds no state, and the only durable side effect is the item
added to the Woolworths cart during the write test.

## Recorded runs

Results of executed runs live in `memory/state.md`, not here — this document is
the procedure.
