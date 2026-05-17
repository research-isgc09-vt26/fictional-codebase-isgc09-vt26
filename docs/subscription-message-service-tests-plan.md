# Plan: Improve unit tests for `SubscriptionMessageService`

Branch: `pilot-planning` · Date: 2026-05-17

This plan covers only test-side changes. No production code, no new files outside this
plan, no formatting runs.

---

## 1. Files inspected

### Production code
- `src/SkillStarLearning.SubscriptionRules.Application/Services/SubscriptionMessageService.cs` — the system under test.
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Services/ISubscriptionMessageService.cs` — interface; single method `GetMessage(SubscriptionMessageFlowType, Segmentation)`.
- `src/SkillStarLearning.SubscriptionRules.Application/Models/SubscriptionMessageDto.cs` — return type. Fields: `FlowType`, `CustomerText` (default `string.Empty`), `RefersToMembershipSignup`, `Segmentation`.
- `src/SkillStarLearning.SubscriptionRules.Core/Enums/SubscriptionMessageFlowType.cs` — `OnlineSubscription = 1`, `CommunityMembershipSubscription = 2`, `MembershipSignup = 3`.
- `src/SkillStarLearning.SubscriptionRules.Core/Enums/Segmentation.cs` — `SegmentationA = 1`, `SegmentationB = 2` (no zero-value member).
- `src/SkillStarLearning.SubscriptionRules.Application/Services/MembershipSignupService.cs` — caller. Uses the message’s `CustomerText` only; under SegmentationB it never reaches `GetMessage` because `MarketSubscriptionPolicy` blocks signup first.
- `src/SkillStarLearning.SubscriptionRules.UI/Controllers/SubscriptionMessageController.cs` — caller. Returns the DTO straight to the API client, defaulting `segmentation` to `SegmentationA`.
- `src/SkillStarLearning.SubscriptionRules.Application/ApplicationServiceRegistration.cs` — DI registers `ISubscriptionMessageService → SubscriptionMessageService` as scoped (stateless service, trivial to construct in tests).

### Tests / helpers
- `tests/SkillStarLearning.SubscriptionRules.UnitTests/SubscriptionMessageTests.cs` — the file to be improved (four tests today).
- `tests/SkillStarLearning.SubscriptionRules.UnitTests/Util/TestFactory.cs` — already exposes `CreateSubscriptionMessageService()`; existing tests bypass it and call `new SubscriptionMessageService()` directly.
- `tests/SkillStarLearning.SubscriptionRules.UnitTests/MarketSubscriptionPolicyTests.cs`, `MembershipSignupTests.cs`, `NewOldSubscriptionServiceTests.cs` — read for style/convention (MSTest, AAA layout, one assertion-cluster per test, no FluentAssertions).
- `tests/SkillStarLearning.SubscriptionRules.UnitTests/SkillStarLearning.SubscriptionRules.UnitTests.csproj` — MSTest 3.1.1 on `net10.0`; no mocking library is referenced.

### Documentation
- `docs/subscription-products.md` — defines the three flow types.
- `docs/market-b-subscription-model.md` — Market B currently uses only `CommunityMembershipSubscription`; stakeholders call it just “subscription”. No online-only or signup-trial in Market B at launch.
- `docs/membership-signup.md` — context for the `MembershipSignup` flow.
- `docs/refactoring-notes.md` — unrelated to this service; flagged for awareness only.

---

## 2. Current behavior (the truth table the tests must pin)

| `segmentation`        | `flowType`                        | `FlowType` echoed | `CustomerText`                              | `RefersToMembershipSignup` | `Segmentation` on DTO          | Throws?                       |
|-----------------------|-----------------------------------|-------------------|---------------------------------------------|----------------------------|--------------------------------|-------------------------------|
| `SegmentationA`       | `OnlineSubscription`              | yes               | `""` (empty)                                | `false`                    | `default` (numeric `0`)        | no                            |
| `SegmentationA`       | `CommunityMembershipSubscription` | yes               | `""` (empty)                                | `false`                    | `default` (numeric `0`)        | no                            |
| `SegmentationA`       | `MembershipSignup`                | yes               | `"Your community signup is complete."`      | `true`                     | `default` (numeric `0`)        | no                            |
| `SegmentationA`       | undefined value (e.g. `(SubscriptionMessageFlowType)999`) | — | — | — | — | `ArgumentOutOfRangeException` |
| `SegmentationB`       | any of the three flow types       | yes               | `"Your community subscription is activated."` | `false` (even for `MembershipSignup`) | `default` (numeric `0`) | no                            |
| `SegmentationB`       | undefined value                   | yes               | `"Your community subscription is activated."` | `false`                    | `default` (numeric `0`)        | no (short-circuits before the switch) |
| undefined `Segmentation` (e.g. `(Segmentation)0` or `(Segmentation)999`) | a defined flow type | yes | per `SegmentationA` rules above | per `SegmentationA` rules | `default` | no                            |

Two findings worth flagging (the tests below pin them; fixes are out of scope):

- **F1**: The service never writes `SubscriptionMessageDto.Segmentation`, so the returned DTO carries an unnamed enum value (`0`). Looks like a missed field; no caller currently reads it, but the controller exposes the DTO publicly.
- **F2**: Under SegmentationB the service ignores `flowType` for both the customer text and `RefersToMembershipSignup`. So a caller passing `MembershipSignup` + `SegmentationB` would receive `RefersToMembershipSignup = false` — semantically wrong but currently unreachable in production because `MembershipSignupService` is blocked earlier by `MarketSubscriptionPolicy`.

---

## 3. Gaps in the current four tests

| Existing test                                                           | Assertions today                                                        | What is missing                                                                                       |
|-------------------------------------------------------------------------|-------------------------------------------------------------------------|-------------------------------------------------------------------------------------------------------|
| `OnlineSubscription_FetchesDefaultMessage`                              | only `RefersToMembershipSignup == false`                                | `FlowType` round-trip; that `CustomerText` is empty                                                   |
| `CommunityMembershipSubscription_FetchesDefaultMessage`                 | only `RefersToMembershipSignup == false`                                | `FlowType` round-trip; that `CustomerText` is empty                                                   |
| `MembershipSignup_FetchesCustomCommunication`                           | `CustomerText` contains substring; `RefersToMembershipSignup == true`   | exact-equality on text; `FlowType` round-trip                                                         |
| `MarketB_SubscriptionMessageUsesLocalShorthandForCommunityMembership`   | `CustomerText` starts with substring                                    | `RefersToMembershipSignup == false`; `FlowType` round-trip; behavior across the *other* flow types under SegmentationB |
| —                                                                       | —                                                                       | Exception path for unsupported `flowType` (`ArgumentOutOfRangeException`)                             |
| —                                                                       | —                                                                       | Undefined `Segmentation` values fall through to SegmentationA logic                                   |
| —                                                                       | —                                                                       | SegmentationB + MembershipSignup (the surprising **F2** case)                                         |
| —                                                                       | —                                                                       | SegmentationB + undefined `flowType` does not throw (short-circuits before switch)                    |
| —                                                                       | —                                                                       | Inconsistent construction: tests use `new SubscriptionMessageService()` while `TestFactory.CreateSubscriptionMessageService()` exists                |

---

## 4. Recommended implementation path

Touch one file only: `tests/SkillStarLearning.SubscriptionRules.UnitTests/SubscriptionMessageTests.cs`.

1. **Switch construction to `TestFactory.CreateSubscriptionMessageService()`** for consistency with `MembershipSignupTests` / `NewOldSubscriptionServiceTests`. The factory already exists and returns `ISubscriptionMessageService`.
2. **Strengthen the four existing tests** so each asserts every observable field on the returned DTO (see §5).
3. **Add the missing coverage** as small, focused tests; prefer `[DataTestMethod]` + `[DataRow]` for the SegmentationB-across-flow-types table so the intent ("Market B collapses all flows to one message") is obvious from the data rows.
4. **Use exact-equality assertions** on `CustomerText` where the text is a fixed literal (today's tests use `StringAssert.Contains` / `StartsWith`, which silently allow drift). The Market B doc warns the wording matters to stakeholders — exact-match makes regressions loud.
5. **Pin findings F1 and F2 with characterization tests** named clearly so a future reader knows the assertion is intentional, not aspirational. Add a one-line code comment on each (`// Pins current behavior — see plan: …`) — the only place in the file where we deviate from the project's "no comments" norm, because the surprise factor warrants it.
6. **No new files, no FluentAssertions, no mocking library.** The service is stateless and has no dependencies, so a parameterless constructor is enough.
7. **Do not modify** `SubscriptionMessageService.cs`, the DTO, the controller, or any other test file. F1 and F2 are out of scope for this task and should be tracked separately if the team agrees they are bugs.

### Notes on naming / style
- Keep the existing `[TestClass] public sealed class SubscriptionMessageTests` shell.
- Follow the project's `Subject_DoesSomething` test-naming style (used in `MarketSubscriptionPolicyTests`).
- Keep AAA structure with blank-line separators, matching the surrounding files.

---

## 5. Tests to add or update

Below, names mirror the project's existing style. Each entry lists the precise assertions to keep the plan unambiguous.

### Reworked (existing tests, stronger assertions)

1. **`OnlineSubscription_SegmentationA_ReturnsDefaultMessageWithEchoedFlowType`** *(replaces `OnlineSubscription_FetchesDefaultMessage`)*
   - `FlowType == SubscriptionMessageFlowType.OnlineSubscription`
   - `CustomerText == string.Empty`
   - `RefersToMembershipSignup == false`

2. **`CommunityMembershipSubscription_SegmentationA_ReturnsDefaultMessageWithEchoedFlowType`** *(replaces `CommunityMembershipSubscription_FetchesDefaultMessage`)*
   - `FlowType == SubscriptionMessageFlowType.CommunityMembershipSubscription`
   - `CustomerText == string.Empty`
   - `RefersToMembershipSignup == false`

3. **`MembershipSignup_SegmentationA_ReturnsSignupConfirmationMessage`** *(replaces `MembershipSignup_FetchesCustomCommunication`)*
   - `FlowType == SubscriptionMessageFlowType.MembershipSignup`
   - `CustomerText == "Your community signup is complete."` (exact)
   - `RefersToMembershipSignup == true`

4. **`CommunityMembershipSubscription_SegmentationB_UsesMarketBShorthand`** *(replaces `MarketB_SubscriptionMessageUsesLocalShorthandForCommunityMembership`)*
   - `FlowType == SubscriptionMessageFlowType.CommunityMembershipSubscription`
   - `CustomerText == "Your community subscription is activated."` (exact)
   - `RefersToMembershipSignup == false`

### New tests (filling the gaps)

5. **`SegmentationB_ReturnsSameMessageRegardlessOfFlowType`** — `[DataTestMethod]` parameterised over `OnlineSubscription`, `CommunityMembershipSubscription`, `MembershipSignup`.
   - `FlowType` echoes the input
   - `CustomerText == "Your community subscription is activated."` for every row
   - `RefersToMembershipSignup == false` for every row
   - *Pins F2 explicitly* (the `MembershipSignup` row).

6. **`SegmentationA_UnsupportedFlowType_Throws`**
   - Calling `GetMessage((SubscriptionMessageFlowType)999, Segmentation.SegmentationA)` throws `ArgumentOutOfRangeException`.
   - Optionally assert `ParamName == "flowType"`.

7. **`SegmentationB_UnsupportedFlowType_DoesNotThrow_ReturnsMarketBMessage`**
   - Calling `GetMessage((SubscriptionMessageFlowType)999, Segmentation.SegmentationB)` returns a DTO with the Market B text and does not throw.
   - Characterization test for the short-circuit ordering in the implementation.

8. **`UndefinedSegmentation_FallsThroughToSegmentationABranch`** — `[DataTestMethod]` with two rows: `(Segmentation)0` and `(Segmentation)999`, each paired with `OnlineSubscription`.
   - Returns the same DTO as the SegmentationA + OnlineSubscription case (empty text, `RefersToMembershipSignup = false`).
   - Pins that the `if (segmentation == Segmentation.SegmentationB)` check uses strict equality, not range validation.

9. **`ReturnedDto_SegmentationPropertyIsDefaultValue`** *(characterization, pins F1)*
   - For any well-formed input, `returned.Segmentation == default(Segmentation)`.
   - In-code comment: "Pins current behavior — the service does not populate DTO.Segmentation. See `docs/subscription-message-service-tests-plan.md` §2 (F1)."

10. **`GetMessage_ReturnsFreshDtoInstanceEachCall`** *(optional, low-cost regression guard)*
    - Two successive calls with identical arguments produce different object references.
    - Justification: cheap insurance that nobody introduces shared/cached state later.

After these changes the file grows from 4 tests to ~10 tests + 2 `[DataTestMethod]` blocks expanding to ~5 data rows, while still being a single class targeted at one service.

---

## 6. Assumptions

1. **"Existing behavior" is the spec.** Anything that looks like a bug (F1, F2, the SegmentationB short-circuit eating the unsupported-flowType throw) gets a characterization test, not a fix. If the team disagrees, F1 and F2 should be filed as separate tickets — this task does not touch production code.
2. **Test framework stays MSTest 3.1.1.** No introduction of FluentAssertions, xUnit, or a mocking framework — none are referenced anywhere in the repo and the service is dependency-free.
3. **Construction goes through `TestFactory.CreateSubscriptionMessageService()`.** It already exists for this exact purpose. The current direct-`new` pattern in `SubscriptionMessageTests.cs` looks like an oversight rather than an intentional deviation.
4. **The wording of customer-facing strings is load-bearing.** The Market B doc explicitly notes stakeholders care about the word "subscription". So tests assert exact equality on `CustomerText`, not substring containment. If product wants to translate or A/B-test the text, the test (and likely the service) need to change deliberately.
5. **`Segmentation` enum defaults to numeric `0`** even though no enum member maps to `0`. This is standard .NET enum behavior; the truth table in §2 treats `(Segmentation)0` as an undefined value and includes it in the fall-through case.
6. **The controller and `MembershipSignupService` are not in scope.** This task is "unit tests for `SubscriptionMessageService`". Integration coverage of the controller and the membership-signup-message hand-off already exists implicitly via `MembershipSignupTests` and can be revisited separately.
7. **`net10.0` and the existing `.slnx`/csproj structure are untouched.** No new package references; no new `<ItemGroup>` entries.
8. **No formatter runs, no production-code edits, no renames.** This plan is the only artifact this turn.

---

## 7. Open questions for the team (not blocking the test work)

- F1 (DTO `Segmentation` never populated): intentional or bug? If bug, opening a ticket is preferable to expanding the test scope here.
- F2 (SegmentationB hard-codes `RefersToMembershipSignup = false` even for `MembershipSignup` flow): currently masked because `MarketSubscriptionPolicy` rejects Market B signups upstream. Worth deciding whether the message service should fail closed (throw) or echo the flow type's expected `RefersToMembershipSignup`. The new characterization test will catch any future drift either way.
- Should the Market B short-circuit be moved *after* the flow-type switch so unsupported values throw consistently? Discoverable via the new test #7.
