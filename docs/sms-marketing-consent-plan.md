# SMS marketing consent — implementation plan (S2-P-01)

## Goal

Add an **optional** SMS marketing consent to the ordinary subscription sign-up
flow for `OnlineSubscription` and `CommunityMembershipSubscription`. When the
consent is selected, a phone number **must** be provided.

Per clarification, "ordinary subscription sign-up flow" here is the existing
`UpdateSubscriptionSettings` flow (the profile-completion step that finalises
sign-up for an already-created account). This plan therefore extends that
feature rather than introducing a new one.

---

## 1. Files inspected

### Documentation
- `docs/subscription-products.md`
- `docs/membership-signup.md`
- `docs/market-b-subscription-model.md`
- `docs/refactoring-notes.md`

### Core (domain)
- `src/SkillStarLearning.SubscriptionRules.Core/Common/AuditableEntity.cs`
- `src/SkillStarLearning.SubscriptionRules.Core/Entities/UserProfile.cs`
- `src/SkillStarLearning.SubscriptionRules.Core/Entities/SubscriptionAccount.cs`
- `src/SkillStarLearning.SubscriptionRules.Core/Entities/MembershipSignup.cs`
- `src/SkillStarLearning.SubscriptionRules.Core/Entities/SubscriptionMessage.cs`
- `src/SkillStarLearning.SubscriptionRules.Core/Enums/SubscriptionType.cs`
- `src/SkillStarLearning.SubscriptionRules.Core/Enums/SubscriptionStatus.cs`
- `src/SkillStarLearning.SubscriptionRules.Core/Enums/PaymentStatus.cs`
- `src/SkillStarLearning.SubscriptionRules.Core/Enums/Segmentation.cs`
- `src/SkillStarLearning.SubscriptionRules.Core/Enums/MarketSubscriptionModel.cs`
- `src/SkillStarLearning.SubscriptionRules.Core/Enums/SubscriptionMessageFlowType.cs`
- `src/SkillStarLearning.SubscriptionRules.Core/Enums/MembershipSignupType.cs`

### Application
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Features/UpdateSubscriptionSettings/UpdateSubscriptionSettingsCommand.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Features/UpdateSubscriptionSettings/IUpdateSubscriptionSettingsHandler.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Features/UpdateSubscriptionSettings/UpdateSubscriptionSettingsHandler.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Features/CreateMembershipSignup/CreateMembershipSignupCommand.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Persistence/IUserProfileRepository.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Persistence/ISubscriptionRepository.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Persistence/IMembershipSignupRepository.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Infrastructure/IAuditLogWriter.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Services/IMembershipSignupService.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Services/INewSubscriptionService.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Services/IOldSubscriptionService.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Services/ISubscriptionAvailabilityService.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Services/ISubscriptionMessageService.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Services/IMarketSubscriptionPolicy.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Services/OldSubscriptionService.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Services/NewSubscriptionService.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Services/MembershipSignupService.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Services/SubscriptionAvailabilityService.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Services/SubscriptionMessageService.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Services/MarketSubscriptionPolicy.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Models/SubscriptionOverviewDto.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Models/SubscriptionProfileDto.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Models/MembershipSignupResultDto.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Models/SubscriptionAvailabilityDto.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Models/SubscriptionMessageDto.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Models/SimpleSubscriptionSummaryDto.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Exceptions/BusinessRuleException.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Exceptions/NotFoundException.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/ApplicationServiceRegistration.cs`

### Infrastructure
- `src/SkillStarLearning.SubscriptionRules.Infrastructure/InfrastructureServiceRegistration.cs`
- `src/SkillStarLearning.SubscriptionRules.Infrastructure/Repositories/InMemorySubscriptionRepository.cs`
- `src/SkillStarLearning.SubscriptionRules.Infrastructure/Repositories/InMemoryUserProfileRepository.cs`
- `src/SkillStarLearning.SubscriptionRules.Infrastructure/Repositories/InMemoryMembershipSignupRepository.cs`
- `src/SkillStarLearning.SubscriptionRules.Infrastructure/AuditLogWriter/AuditLogWriter.cs`

### UI
- `src/SkillStarLearning.SubscriptionRules.UI/Program.cs`
- `src/SkillStarLearning.SubscriptionRules.UI/Controllers/SubscriptionSettingsController.cs`
- `src/SkillStarLearning.SubscriptionRules.UI/Controllers/MembershipSignupController.cs`
- `src/SkillStarLearning.SubscriptionRules.UI/Controllers/SubscriptionAvailabilityController.cs`
- `src/SkillStarLearning.SubscriptionRules.UI/Controllers/SubscriptionMessageController.cs`
- `src/SkillStarLearning.SubscriptionRules.UI/Controllers/SubscriptionWidgetController.cs`

### Tests
- `tests/SkillStarLearning.SubscriptionRules.UnitTests/MarketSubscriptionPolicyTests.cs`
- `tests/SkillStarLearning.SubscriptionRules.UnitTests/MembershipSignupTests.cs`
- `tests/SkillStarLearning.SubscriptionRules.UnitTests/NewOldSubscriptionServiceTests.cs`
- `tests/SkillStarLearning.SubscriptionRules.UnitTests/SubscriptionMessageTests.cs`
- `tests/SkillStarLearning.SubscriptionRules.UnitTests/Util/TestFactory.cs`
- `tests/SkillStarLearning.SubscriptionRules.UnitTests/Util/FixedTimeProvider.cs`

### Solution / out-of-scope
- `SkillStarLearning.SubscriptionRules.slnx` — confirms the active projects.
- `src/SkillStarLearning.SubscriptionRules.Domain/Entities/UserProfile.cs` and
  `…/Entities/SubscriptionAccount.cs` — present on disk but **not in the
  solution file**; treated as stale duplicates of `Core` and intentionally not
  touched.

---

## 2. Recommended implementation path

### 2.1 Naming

Use `AllowsSmsMarketing` (boolean) to stay consistent with the existing
`AllowsEventCommunication` flag on `UserProfile`. The two are distinct:

| Flag | Meaning |
|---|---|
| `AllowsEventCommunication` | Existing — community-events communication consent. |
| `AllowsSmsMarketing` | **New** — opt-in to receive marketing messages via SMS. |

Keeping them separate avoids changing the semantics of the existing field and
matches the task wording ("SMS marketing consent").

### 2.2 Step-by-step changes

**Step 1 — Add the field to the domain entity.**
`src/SkillStarLearning.SubscriptionRules.Core/Entities/UserProfile.cs`
- Add `public bool AllowsSmsMarketing { get; set; }` (default `false`).

**Step 2 — Surface the field on the inbound command.**
`src/.../Application/Contracts/Features/UpdateSubscriptionSettings/UpdateSubscriptionSettingsCommand.cs`
- Add `public bool AllowsSmsMarketing { get; set; }`.
- `PhoneNumber` already exists on the command (and currently defaults to `string.Empty`).

**Step 3 — Surface the field on the outbound DTO.**
`src/.../Application/Models/SubscriptionProfileDto.cs`
- Add `public bool AllowsSmsMarketing { get; set; }` so the overview reflects
  what was stored.

**Step 4 — Validate the consent → phone number rule in the handler.**
`src/.../Application/Contracts/Features/UpdateSubscriptionSettings/UpdateSubscriptionSettingsHandler.cs`

- Add a new private validation step that runs **only when**
  `account.SubscriptionType` is `OnlineSubscription` **or**
  `CommunityMembershipSubscription` (per task scope; `MembershipSignupSubscription`
  is excluded because it is the offline/trial flow handled elsewhere).
- Throw `BusinessRuleException("A phone number is required when SMS marketing consent is given.")`
  when `command.AllowsSmsMarketing == true` and `command.PhoneNumber` is null
  or whitespace.
- Existing `ValidateMembershipFields` remains unchanged; the new check is
  additive and lives next to it.
- After validation, copy `command.AllowsSmsMarketing` onto `profile` before
  `SaveAsync`, alongside the other property assignments.

Sketch of the new validator (illustrative — actual implementation happens in
the code task, not here):

```csharp
private static void ValidateSmsMarketingConsent(UpdateSubscriptionSettingsCommand command)
{
    if (command.AllowsSmsMarketing && string.IsNullOrWhiteSpace(command.PhoneNumber))
    {
        throw new BusinessRuleException(
            "A phone number is required when SMS marketing consent is given.");
    }
}
```

Call site inside `Handle`:

```csharp
if (account.SubscriptionType == SubscriptionType.OnlineSubscription
    || account.SubscriptionType == SubscriptionType.CommunityMembershipSubscription)
{
    ValidateSmsMarketingConsent(command);
}

if (account.SubscriptionType == SubscriptionType.CommunityMembershipSubscription)
{
    ValidateMembershipFields(command);
}
```

**Step 5 — Map the new field in the overview projection.**
`src/.../Application/Services/OldSubscriptionService.cs` (`ToOverview`)
- Add `AllowsSmsMarketing = profile.AllowsSmsMarketing` to the
  `SubscriptionProfileDto` initializer.
- No change to `RequiresMembershipProfileReview` logic — SMS consent is
  optional, so an absent consent must not trigger the membership review
  nudge.

**Step 6 — Seed coverage in the in-memory repo (optional but useful for tests).**
`src/.../Infrastructure/Repositories/InMemoryUserProfileRepository.cs`
- Leave existing fixtures defaulting to `false` (the default for `bool`).
- No new seed users are strictly required; tests can exercise the new
  behaviour by toggling the command.

### 2.3 What deliberately does NOT change

- `MembershipSignupService` / `CreateMembershipSignupCommand` — the offline
  flow is out of scope for SMS marketing consent.
- `MarketSubscriptionPolicy`, `SubscriptionAvailabilityService`,
  `SubscriptionMessageService` — SMS consent is per-user, not per-market.
- `NewSubscriptionService` / `SimpleSubscriptionSummaryDto` — widget summary
  doesn't expose profile fields and shouldn't grow them ahead of TECHDEBT-123.
- The stale `src/SkillStarLearning.SubscriptionRules.Domain/...` files — not
  in the solution, not referenced, not touched.

---

## 3. Tests to add or update

All tests live in `tests/SkillStarLearning.SubscriptionRules.UnitTests/`.

### 3.1 New test file
**`UpdateSubscriptionSettingsHandlerTests.cs`** (the handler currently has no
direct coverage — adding a file dedicated to it keeps the existing test files
focused).

Cases:

1. **`SmsConsentFalse_PassesValidation_ForOnlineSubscription`** — sets
   `AllowsSmsMarketing = false` with empty `PhoneNumber` on `online-user-01`;
   expects success and `overview.Profile.AllowsSmsMarketing == false`.
2. **`SmsConsentTrueWithPhone_PassesValidation_ForOnlineSubscription`** —
   sets `AllowsSmsMarketing = true` with a non-empty `PhoneNumber` on
   `online-user-01`; expects success and the flag and phone number to round-trip
   into the returned `SubscriptionOverviewDto`.
3. **`SmsConsentTrueWithoutPhone_Throws_ForOnlineSubscription`** — sets
   `AllowsSmsMarketing = true` with empty `PhoneNumber` on `online-user-01`;
   expects `BusinessRuleException`.
4. **`SmsConsentTrueWithoutPhone_Throws_ForCommunityMembershipSubscription`** —
   same as above on `community-user-01`; expects `BusinessRuleException` (and
   ensures the rule fires *before* the existing `ValidateMembershipFields`
   rule, or independently of it — assertion is just that the SMS-consent
   message is the one thrown when phone is missing but the other community
   fields are valid).
5. **`SmsConsentTrueWithPhone_PassesValidation_ForCommunityMembership`** —
   sets `AllowsSmsMarketing = true`, valid phone, valid community fields on
   `community-user-01`; expects success.
6. **`SmsConsentTrue_DoesNotApply_ForMembershipSignupSubscription`** — picks
   an account with `SubscriptionType == MembershipSignupSubscription` (e.g.
   `signup-user-01`) and verifies the SMS-consent validation is **not** run
   (i.e. setting `AllowsSmsMarketing = true` with empty `PhoneNumber` does
   not throw, because this type is outside the task's scope). Documents the
   scoping decision.

`TestFactory` will need a small helper to construct
`UpdateSubscriptionSettingsHandler` from the existing in-memory repositories
+ `OldSubscriptionService` — add `CreateUpdateSubscriptionSettingsHandler()`
alongside the existing factory methods.

### 3.2 Updates to existing tests

- **`NewOldSubscriptionServiceTests.cs`**
  - `OldSubscriptionService_ReturnsMembershipProfileFields_WhenMembershipCommunitySubscription`
    — extend to assert `overview.Profile.AllowsSmsMarketing` value
    round-trips (defaults to `false` for the seeded `community-user-01`).
  - No other functional change; just one additional assertion to lock the
    field into the overview projection contract.

- **`MembershipSignupTests.cs`, `SubscriptionMessageTests.cs`,
  `MarketSubscriptionPolicyTests.cs`** — no changes; SMS consent does not
  affect those flows.

### 3.3 Manual / API-level verification (optional)

If a quick request smoke-test is desired post-implementation:
`PUT /api/subscription-settings/online-user-01` with
`{"allowsSmsMarketing": true, "phoneNumber": ""}` should return a 4xx
mapping of `BusinessRuleException`; with a populated `phoneNumber`, 200 OK
with the new field reflected. (Note: there is currently no exception-to-status
middleware in `Program.cs`, so an unmapped `BusinessRuleException` would
surface as a 500 — same behaviour as the existing community validation. Out
of scope for this ticket.)

---

## 4. Assumptions

1. **"Ordinary sign-up flow" = `UpdateSubscriptionSettings`.** Confirmed with
   the user. The repo has no separate online sign-up handler; the closest
   user-driven, command-style flow that touches the relevant profile fields
   is `UpdateSubscriptionSettings`. `MembershipSignupService` is explicitly
   the offline event flow and is out of scope.
2. **Scope is `OnlineSubscription` and `CommunityMembershipSubscription` only.**
   `MembershipSignupSubscription` accounts are excluded from the new
   validation, mirroring the task wording. If product wants the rule to apply
   to that type too, the conditional in step 4 is a one-line change.
3. **Optional means default-false.** A user who does not opt in continues to
   see no phone-number requirement; `AllowsSmsMarketing` defaults to `false`
   on the entity, command, and DTO.
4. **Phone "provided" = non-null and non-whitespace.** Matches the existing
   `string.IsNullOrEmpty(profile.PhoneNumber)` check used in
   `OldSubscriptionService.ToOverview`. Format validation (E.164, country
   code, length) is out of scope — the task only requires that *a* phone
   number be present.
5. **SMS marketing consent is distinct from `AllowsEventCommunication`.** The
   existing flag is community-events-oriented and is part of the membership
   profile-review nudge. Overloading it would silently change the nudge
   semantics, so a new field is added rather than reused.
6. **Storage stays on `UserProfile`.** The existing in-memory repository
   pattern is preserved; no new entity or repository is introduced. If a
   later ticket moves consent into the Privacy module referenced in
   `docs/refactoring-notes.md`, the new field will need to move with it —
   noted as a future migration concern, not blocking this ticket.
7. **The `Domain` project on disk is stale.** The `.slnx` only includes
   `Core`, `Application`, `Infrastructure`, `UI`, and `UnitTests`. The
   duplicated entities under `src/SkillStarLearning.SubscriptionRules.Domain/`
   are not part of the build and are not modified.
8. **No market-policy gating.** Both Market A (Segmentation A) and Market B
   (Segmentation B) get the SMS consent option, since the requirement does
   not mention a market restriction and SMS consent is a per-user property.
   If legal requires gating by market later, `MarketSubscriptionPolicy` is
   the natural place to add an `IsSmsMarketingAvailable(Segmentation)`
   predicate.
9. **No customer-facing message change.** `SubscriptionMessageService` is not
   extended; copy for the consent checkbox is a UI concern outside this
   repo.
10. **No widget impact.** The subscription widget (`NewSubscriptionService` /
    `SimpleSubscriptionSummaryDto`) intentionally avoids profile fields per
    the refactoring notes; SMS consent stays off the widget.

---

## Out-of-scope follow-ups (not part of this ticket)

- Phone-number format validation / normalization.
- Mapping `BusinessRuleException` to HTTP 400 in `Program.cs`.
- Persisting consent history (timestamp, source) for audit/legal — the
  `IAuditLogWriter` could be invoked when the flag flips, but no log is
  added here because the existing handler writes no audit entries today.
- Migrating consent to the Privacy module referenced in
  `docs/refactoring-notes.md`.
