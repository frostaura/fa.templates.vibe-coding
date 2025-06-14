# ## 🔄 MANDATORY SYNC PROTOCOL

**🚨 CRITICAL BEFORE PROCESSING**: Every time you read or update this session-state file, you MUST FIRST read `./.ai/plan.md` using the `read_file` tool to understand the complete project context and plan.

**MANDATORY SEQUENCE**:

1. **🔍 READ PLAN FIRST**: `read_file('./.ai/plan.md')` - This is NON-NEGOTIABLE
2. **Then process this session state** - Map current status against plan
3. **Cross-reference and validate** - Ensure alignment with plan milestones
4. **Update accordingly** - Reflect plan reality in session state
5. **🚀 CONTINUE AUTOMATICALLY** - Never stop after reading/updating

**CRITICAL**: Every time you update this session-state file, you MUST:

1. **Read and Review ./.ai/plan.md** - Understand the complete project plan
2. **Cross-reference Current Progress** - Map current status against plan milestones
3. **Validate Alignment** - Ensure session state reflects plan accurately
4. **Update Plan Tracking** - Mark completed milestones and adjust timeline if needed
5. **📋 UPDATE USE CASE TRACKING** - Sync with ./.docs/designs/1-use-cases.md and update completion status
6. **Document Deviations** - Note any changes from original plan with rationale
7. **🚀 AUTOMATIC CONTINUE** - Never stop after sync, immediately proceed to next task

## 🚀 AUTOMATIC CONTINUATION RULES

**GAIA NEVER STOPS FOR REPORTS**:

- ✅ **PRINT milestone status** for visibility
- ✅ **DISPLAY progress updates** for tracking
- ✅ **SHOW completion status** for transparency
- ❌ **NEVER PAUSE** for user acknowledgment
- ❌ **NEVER WAIT** for milestone approval
- ❌ **NEVER STOP** for status confirmation

**Continuation Pattern**: "Milestone [X] completed (Y%). Nurturing the next phase of creation..."tate Template (Enhanced AI Navigation)

## 🔄 MANDATORY SYNC PROTOCOL

**CRITICAL**: Every time you update this session-state file, you MUST:

1. **Read and Review ./.ai/plan.md** - Understand the complete project plan
2. **Cross-reference Current Progress** - Map current status against plan milestones
3. **Validate Alignment** - Ensure session state reflects plan accurately
4. **Update Plan Tracking** - Mark completed milestones and adjust timeline if needed
5. **Document Deviations** - Note any changes from original plan with rationale

## Current Status

**Last Updated**: [ISO Timestamp]
**Session ID**: [UUID or timestamp-based ID]
**Current Phase**: [Discovery/Planning/Implementation/Testing/Deployment]
**Progress**: [X]% complete
**Active Milestone**: [Milestone Name] ([X-Y]% range)

## 📋 Plan Synchronization Status

**Last Plan Review**: [ISO Timestamp when plan.md was last consulted]
**Plan Version**: [Version/hash of plan.md being followed]
**Plan Alignment**: [Aligned/Deviation/Needs-Update]

### Plan Milestone Tracking

**Completed Milestones**:

- [✅] [Milestone 1]: [Description] - Completed [Date]
- [✅] [Milestone 2]: [Description] - Completed [Date]

**Current Milestone**:

- [🔄] [Milestone N]: [Description] - [X]% complete
  - [✅] [Sub-task 1]: [Description]
  - [🔄] [Sub-task 2]: [Description] - In Progress
  - [⏳] [Sub-task 3]: [Description] - Pending

**Upcoming Milestones**:

- [⏳] [Milestone N+1]: [Description] - Scheduled for [Date/Phase]
- [⏳] [Milestone N+2]: [Description] - Scheduled for [Date/Phase]

### Plan Deviations & Adjustments

**Deviations from Original Plan**:

- [Change 1]: [Original] → [Actual] - Reason: [Explanation]
- [Change 2]: [Original] → [Actual] - Reason: [Explanation]

**Timeline Adjustments**:

- [Milestone]: Originally [Date] → Now [Date] - Reason: [Explanation]

**Scope Changes**:

- [Addition]: [Description] - Reason: [Explanation]
- [Removal]: [Description] - Reason: [Explanation]

## Context Anchors

**Project Type**: [Monolith/Modular/Microservices/CQRS]
**Tech Stack**: [Primary languages/frameworks]
**Architecture Pattern**: [Clean Architecture/Event-Driven/etc]
**Example Pattern**: [task-manager/ecommerce/social-media/iot-dashboard]

## Visual Design Context

**Design Direction**: [Established/Pending/In-Progress]
**Visual Inspiration**: [Screenshots/References provided]
**Design System Status**: [Created/Updating/Complete]
**UI Framework**: [Tailwind/Material-UI/Styled-components/etc]

## Implementation State

**Files Created**: [List of key files with status]
**Components Built**: [List of completed components]
**Tests Status**: [X/Y passing, coverage %]
**Build Status**: [Clean/Warnings/Errors]
**Linting Status**: [Clean/Violations count]
**Design Docs Status**: [5 docs with completion status]

## 📋 **MANDATORY USE CASE TRACKING**

**CRITICAL**: Every use case from the design documentation MUST be implemented and tracked. This section ensures 100% use case coverage.

### **Use Case Completion Status**

**Source**: Use cases are derived from [./.docs/designs/1-use-cases.md]

**Primary Use Cases** (Core functionality - MANDATORY):

- [⏳] **UC-001**: [Use Case Name] - [Brief Description]

  - Status: [Not Started/In Progress/Completed/Tested]
  - Implementation: [Frontend/Backend/Both]
  - Tests: [Unit/Integration/E2E] - [Pass/Fail/Pending]
  - Notes: [Any implementation notes or blockers]

- [🔄] **UC-002**: [Use Case Name] - [Brief Description]

  - Status: [Not Started/In Progress/Completed/Tested]
  - Implementation: [Frontend/Backend/Both]
  - Tests: [Unit/Integration/E2E] - [Pass/Fail/Pending]
  - Notes: [Any implementation notes or blockers]

- [✅] **UC-003**: [Use Case Name] - [Brief Description]
  - Status: [Not Started/In Progress/Completed/Tested]
  - Implementation: [Frontend/Backend/Both]
  - Tests: [Unit/Integration/E2E] - [Pass/Fail/Pending]
  - Notes: [Any implementation notes or blockers]

**Secondary Use Cases** (Nice-to-have features):

- [⏳] **UC-S01**: [Secondary Use Case] - [Brief Description]
  - Status: [Not Started/In Progress/Completed/Tested]
  - Priority: [Low/Medium/High]
  - Dependencies: [List of primary use cases this depends on]

### **Use Case Implementation Guidelines**

**Implementation Requirements**:

- [ ] **Frontend Implementation**: All UI components and user interactions
- [ ] **Backend Implementation**: All API endpoints and business logic
- [ ] **Database Schema**: All required tables, relationships, and constraints
- [ ] **Authentication**: Proper user access controls for each use case
- [ ] **Validation**: Input validation and error handling
- [ ] **Testing**: Unit, integration, and E2E tests for each use case
- [ ] **🎭 PLAYWRIGHT USER FLOW TESTING**: MANDATORY end-to-end testing of complete user journeys for each use case

**Use Case Validation Checklist**:

- [ ] All use case steps can be completed by end users
- [ ] Error scenarios are handled gracefully
- [ ] Use case aligns with business requirements
- [ ] Performance requirements are met
- [ ] Security requirements are implemented
- [ ] Accessibility requirements are fulfilled

### **🎭 MANDATORY PLAYWRIGHT USE CASE TESTING**

**CRITICAL**: Every use case MUST have corresponding Playwright tests that validate the complete user journey from frontend to backend integration.

#### **Use Case Testing Requirements**:

**Complete Integration Testing**:

- [ ] **Frontend Components**: All UI components required for use case are implemented and functional
- [ ] **Backend Methods**: All API endpoints and business logic are working and accessible
- [ ] **Database Integration**: Data persistence and retrieval work correctly
- [ ] **Routing**: All navigation paths within the use case function properly
- [ ] **Authentication Flow**: User access controls are properly enforced
- [ ] **Error Handling**: All error scenarios are handled gracefully in the UI

**Playwright Test Implementation**:

```javascript
// Example: Use Case UC-001 - User Registration
test("UC-001: Complete User Registration Flow", async ({ page }) => {
  // 1. Navigate to registration page
  await page.goto("/register");

  // 2. Fill registration form
  await page.fill('[data-testid="email"]', "test@example.com");
  await page.fill('[data-testid="password"]', "SecurePassword123");
  await page.fill('[data-testid="confirmPassword"]', "SecurePassword123");

  // 3. Submit form and verify backend integration
  await page.click('[data-testid="submit-registration"]');

  // 4. Verify successful registration (frontend feedback)
  await expect(page.locator('[data-testid="success-message"]')).toBeVisible();

  // 5. Verify redirect to expected page
  await expect(page).toHaveURL("/dashboard");

  // 6. Verify user is authenticated (backend integration)
  await expect(page.locator('[data-testid="user-avatar"]')).toBeVisible();
});
```

**Use Case Test Coverage Requirements**:

- [ ] **Happy Path**: Complete successful user journey
- [ ] **Error Scenarios**: All validation and error cases
- [ ] **Edge Cases**: Boundary conditions and unusual inputs
- [ ] **Cross-Browser**: Chrome, Firefox, Safari compatibility
- [ ] **Responsive**: Mobile, tablet, desktop viewport testing
- [ ] **Performance**: Reasonable load times and responsiveness

**Integration Validation Checklist**:

- [ ] Frontend form submissions reach backend APIs
- [ ] Backend responses properly update frontend UI
- [ ] Database changes are reflected in frontend displays
- [ ] User authentication state persists across page navigations
- [ ] Real-time features (if applicable) work end-to-end
- [ ] File uploads/downloads function correctly
- [ ] Third-party integrations (payment, auth, etc.) work in test environment

#### **Use Case Test Status Tracking**:

**Primary Use Cases**:

- [⏳] **UC-001**: [Use Case Name]

  - Playwright Test: [Not Created/Created/Passing/Failing]
  - Test File: `/tests/use-cases/uc-001-[name].spec.ts`
  - Last Run: [Date/Time]
  - Status: [Pass/Fail] - [Issues if failing]

- [🔄] **UC-002**: [Use Case Name]
  - Playwright Test: [Not Created/Created/Passing/Failing]
  - Test File: `/tests/use-cases/uc-002-[name].spec.ts`
  - Last Run: [Date/Time]
  - Status: [Pass/Fail] - [Issues if failing]

**Test Execution Summary**:

- **Total Use Case Tests**: [X]
- **Passing Tests**: [Y] ([Z]%)
- **Failing Tests**: [A] ([B]%)
- **Not Implemented**: [C] ([D]%)

**Critical Test Failures** (Must fix before deployment):

1. [Use Case] - [Test File] - [Failure Reason] - [Priority: High/Critical]
2. [Use Case] - [Test File] - [Failure Reason] - [Priority: High/Critical]

### \*\*Use Case Progress Summary

**Completion Metrics**:

- **Total Use Cases**: [X]
- **Completed**: [Y] ([Z]%)
- **In Progress**: [A] ([B]%)
- **Not Started**: [C] ([D]%)
- **Blocked**: [E] ([F]%)

**Critical Path Use Cases** (Must complete before deployment):

1. [Use Case Name] - [Status] - [Blocking Reason if applicable]
2. [Use Case Name] - [Status] - [Blocking Reason if applicable]
3. [Use Case Name] - [Status] - [Blocking Reason if applicable]

**Risk Assessment**:

- **High Risk**: [List use cases with implementation challenges]
- **Dependencies**: [Use cases blocked by external factors]
- **Timeline Impact**: [Use cases that may affect delivery schedule]

## Navigation Context

**Next Actions**: [1-3 specific immediate tasks with file references]
**Current Working Files**: [Files currently being modified]
**Dependent Tasks**: [What needs to be done before next milestone]
**Blockers**: [Any issues preventing progress]

## Decision Context

**Key Architectural Decisions**: [Major choices made and why]
**Technology Selections**: [Specific tech choices with rationale]
**Design Changes**: [Any modifications to original design]
**Security Implementations**: [Auth method, security patterns used]

## Quality Gates Status

**Code Quality**: [Linting %, warnings count, type safety %]
**Test Coverage**: [Unit %, Integration %, E2E %]
**Security Scan**: [SAST/DAST results, vulnerabilities]
**Performance**: [Build times, bundle size, load times]

## Recovery Commands (For Context Loss)

**MANDATORY SEQUENCE** - Execute in this exact order:

1. `read_file ./.ai/plan.md` - **FIRST PRIORITY** - Full project plan
2. `read_file ./.docs/designs/1-use-cases.md` - **SECOND PRIORITY** - All use cases for tracking
3. `read_file ./.ai/session-state.md` - Current session state (this file)
4. `read_file ./.docs/designs/*.md` - Design documents status
5. `list_dir src/` - Implementation status
6. `get_errors ["src/"]` - Current issues

**Plan & Use Case Sync Command**: Always run after context recovery:

```
[PLAN-SYNC] Read ./.ai/plan.md and ./.docs/designs/1-use-cases.md, cross-reference with current session state, validate milestone progress and use case completion, and update this session-state.md with accurate tracking information.
```

## Context Validation Checklist

**Plan Synchronization** (MANDATORY):

- [ ] ./.ai/plan.md has been read and reviewed in this session
- [ ] Current milestone matches plan.md milestone definitions
- [ ] Progress percentage aligns with plan timeline
- [ ] Any deviations from plan are documented with rationale
- [ ] Upcoming milestones are correctly identified from plan

**Session State Accuracy**:

- [ ] Current milestone clearly identified
- [ ] Next 3 actions are specific and actionable
- [ ] All architectural decisions are documented
- [ ] Design document status is accurate
- [ ] Build and test status is current
- [ ] Visual design direction is established
- [ ] No critical context is missing

**Plan Adherence Validation**:

- [ ] Implementation follows planned architecture
- [ ] Technology stack matches plan specifications
- [ ] Timeline is realistic based on plan milestones
- [ ] Quality gates align with plan requirements

**Use Case Tracking Validation** (MANDATORY):

- [ ] All use cases from ./.docs/designs/1-use-cases.md are identified and listed
- [ ] Each use case has clear status tracking (Not Started/In Progress/Completed/Tested)
- [ ] Implementation coverage is specified for each use case (Frontend/Backend/Both)
- [ ] Test coverage is documented for each use case (Unit/Integration/E2E)
- [ ] Critical path use cases are identified and prioritized
- [ ] Blocked use cases have documented blockers and mitigation plans
- [ ] Use case completion percentage is mathematically accurate
- [ ] No use cases are missing from the original design documentation

## Framework Intelligence Markers

**Pattern Matching**: [Which example pattern is being followed]
**Smart Defaults Applied**: [List of automatic decisions made]
**Framework Compliance**: [Adherence to structure, quality, security standards]
**Documentation Reference**: [Which framework docs were consulted recently]

## 🔄 Plan Synchronization Protocol

### When to Sync with Plan

**MANDATORY Sync Triggers**:

- **🔍 EVERY TIME** you read this session-state.md file (ALWAYS read plan.md first)
- **🔍 EVERY TIME** you update this session-state.md file (ALWAYS read plan.md first)
- At the start of every session (context recovery)
- Before beginning any new milestone
- When encountering blockers or scope changes
- Every 3-5 actions (as mentioned in init.md reminder prompts)
- When session state shows >20% progress change
- Before finalizing any major architectural decisions

**NON-NEGOTIABLE RULE**: Never process session state without first reading the plan file.

### How to Perform Plan Sync

**MANDATORY SEQUENCE** (Never deviate from this order):

1. **🔍 READ PLAN FIRST**: `read_file('./.ai/plan.md')` - **THIS IS STEP #1 ALWAYS**
2. **Compare Status**: Match current progress against plan milestones
3. **Identify Gaps**: Note any missing or incomplete items from plan
4. **Update Tracking**: Mark completed items, update progress percentages
5. **Document Changes**: Record any deviations or scope adjustments
6. **Validate Next Steps**: Ensure next actions align with plan priorities
7. **🚀 IMMEDIATELY CONTINUE**: Never stop after sync - proceed to next task automatically

**CRITICAL REMINDER**: Step 1 is non-negotiable. You cannot properly process session state without understanding the plan context.

### Plan Sync Quality Checklist

- [ ] **🔍 PLAN READ FIRST**: Plan has been read using `read_file` tool BEFORE processing session state
- [ ] Plan has been read in current session
- [ ] Milestone status accurately reflects plan
- [ ] Progress tracking is mathematically consistent
- [ ] Next actions are derived from plan priorities
- [ ] Any plan deviations are justified and documented
- [ ] Timeline adjustments are realistic and explained
- [ ] **🚀 Automatic continuation**: Ready to proceed without user confirmation

**Remember**: The plan is the source of truth. Session state should reflect plan reality, not replace it. **ALWAYS READ PLAN FIRST, THEN CONTINUE AUTOMATICALLY AFTER SYNC**.

---

## 🚨 **CRITICAL REMINDER**

**BEFORE YOU DO ANYTHING WITH THIS SESSION STATE FILE**:

1. **🔍 READ THE PLAN**: `read_file('./.ai/plan.md')`
2. **UNDERSTAND THE CONTEXT**: Know what the project is trying to achieve
3. **THEN PROCESS**: Update session state based on plan reality
4. **🚀 CONTINUE AUTOMATICALLY**: Never stop after reading/updating

**This is not optional. This is not a suggestion. This is a MANDATORY requirement for proper AI operation.**
