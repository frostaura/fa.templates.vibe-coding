# Device Build and Deploy: .NET MAUI

Companion to `fa-engineering-stack-dotnet-maui`. The baseline skill establishes analyzers, a build
target and a unit lane. This file covers getting that build onto an emulator, a simulator or
physical hardware, and knowing which of those results may be believed.

It does not cover store submission, release distribution or signing-certificate administration.

Throughout, `<android-tfm>` and `<ios-tfm>` are the target frameworks the repository pins,
`<udid>` is the identifier the device tooling reports, and `<bundle-id>` is the application
identifier declared in the project.

## Order of operations

Build the pure shared or core project before any platform head. It carries no platform SDK
dependency, compiles in seconds, and catches almost every error a platform build would surface
minutes later. Only once it is clean is a head build worth starting.

Where a platform target sits behind an opt-in property, it still has to compile. A head that has
quietly stopped building is a defect even while a different platform is the one shipping.

## Android: two deploy paths, and they must not be mixed

Path A, build an installable package and install it by hand:

```
dotnet build -f <android-tfm> -p:EmbedAssembliesIntoApk=true -p:AndroidPackageFormat=apk
adb install -g -r <path-to-signed-apk>
```

Path B, let the SDK build, deploy and launch in one step:

```
dotnet build -t:Run -f <android-tfm>
```

**Never install a package built without `EmbedAssembliesIntoApk=true`.** A default Debug build uses
fast deployment: the managed assemblies are pushed to the device out of band and are not inside the
package. Installing that package by hand leaves the device executing the managed code deployed by
some earlier run. The install reports success, the app launches, and every observation in the QA
pass describes a build that was never made. Nothing warns in either direction, which is what makes
this the most expensive mistake on the platform.

Reinstalling mid-session also terminates whatever is running, discarding the state under test.
Finish an observation before deploying again.

## iOS simulator

The simulator needs no provisioning profile and no signing identity, and a repository that has
accidentally forced real signing onto it will fail with a codesigning error that reads like a
certificate problem.

**An entitlements file must not be named `Platforms/iOS/Entitlements.plist`.** MAUI auto-detects
that exact path and switches the simulator build to real signing. Give the file another name and
reference it explicitly from the project for the device configuration only.

## iOS physical device

The repeatable sequence, in order:

1. Create a throwaway Xcode project whose bundle identifier matches the app's, and let automatic
   signing mint or refresh the profile:

   ```
   xcodebuild -project <throwaway>.xcodeproj -scheme <scheme> \
     -destination 'generic/platform=iOS' -allowProvisioningUpdates build
   ```

   This exists because the .NET build cannot create a provisioning profile; Xcode can, and the
   profile it mints is then reusable by the real build. Discard the throwaway project afterwards.

2. Build the head against the device runtime, naming the identity and profile explicitly:

   ```
   dotnet build -f <ios-tfm> -p:RuntimeIdentifier=ios-arm64 \
     -p:CodesignKey="<signing identity>" -p:CodesignProvision="<profile name>"
   ```

3. Install and launch:

   ```
   xcrun devicectl device install app --device <udid> <path-to-app-bundle>
   xcrun devicectl device process launch --device <udid> <bundle-id>
   ```

Install and launch both fail while the device is locked, with an error that does not say so.
Unlock and retry before treating the first failure as a signing fault.

## Toolchain version suppression

Disabling the Xcode version check with `ValidateXcodeVersion=false` does not fix a mismatch; it
suppresses a real one between the installed Xcode and the installed workload. It is a legitimate
unblock and an unsafe permanent setting. Record why it is set, and re-check whether it is still
needed whenever either side moves, because the failures it hides surface as codesigning and
runtime faults rather than as build errors.

## App manifest and entitlements

Editing `Platforms/iOS/Info.plist` alone does not rebuild the app manifest. The generated
`AppManifest.plist` under the intermediate output directory is cached, so the change appears in
source and not in the bundle. Delete the generated manifest, rebuild, and verify the bundle that
was actually produced:

```
plutil -p <path-to-app-bundle>/Info.plist
```

Verifying the source file instead of the built bundle proves nothing.

## Cloud-synced or virtualised working trees

A working tree on a synced or virtualised filesystem breaks builds in ways that look like code
faults:

- **MSB3021, access denied while copying a file**, is a sync race rather than a permissions
  problem. Retry with `-p:CopyRetryCount=6 -p:CopyRetryDelayMilliseconds=2000`.
- **Two concurrent builds corrupt the intermediate output.** Incremental state is not safe against
  a second writer. Clean the intermediate directory and rebuild rather than trusting the result.
- **Install from a path outside the synced or virtualised filesystem.** Copy the built bundle to
  local scratch storage first; device install tooling reads the bundle in full, and a dataless
  placeholder or an extended attribute added by the sync client breaks the install or the
  signature. Where the toolchain requires it, strip extended attributes as a build step.
- **Large binary assets belong outside the tree.** A sync client evicts them to placeholders and
  the build or the app fails on a file that appears to be present.

## Validating on hardware

A green build is not a working app. Gradient alpha values, styles shared across shape instances,
and duplicated route or tab names all compile cleanly and fail only at runtime.

An emulator or simulator result is not an on-device result. Sensor, haptic, performance and
codesigning behaviour all diverge on real hardware, so on-device behaviour is reported only from a
run on the hardware itself.

Host-operating-system privacy controls can block synthetic taps against a simulator, which reads
as a broken app rather than a blocked harness. Drive tap-driven validation on the emulator, and
treat the simulator as a screenshot surface plus a QA entry point selected by environment
variable.
