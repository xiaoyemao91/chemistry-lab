---
name: testing
description: Plan, implement, run, or review tests for the ICP-OES Unity simulation, including EditMode domain tests, PlayMode workflow tests, JSON validation, calculation reference cases, local record persistence, manual acceptance, and regression diagnosis. Use whenever behavior changes or a defect is investigated.
---

# Simulation Testing

Read `REQUIREMENTS.md`, `docs/MVP_ACCEPTANCE_TEST.md`, and the affected design documents before testing.

## Workflow

1. State the behavior and observable success criteria.
2. For defects, capture a failing test or repeatable check before changing code.
3. Use EditMode for pure state, validation, calculation, and serialization logic.
4. Use PlayMode only for Unity lifecycle, scene, input, visual binding, and full-path behavior.
5. Cover valid input, invalid input, boundaries, duplicate actions, reset, persistence failure, and regression behavior as relevant.
6. Run the narrowest reliable suite, record the exact command and result, then expand only when risk requires it.

## Scientific verification

Use only teacher-approved datasets as scientific oracles. Record content and source versions with expected results and tolerances. Synthetic datasets may test software mechanics but must be labelled synthetic and cannot prove scientific accuracy.

## Reporting

Report passed, failed, skipped, and unavailable checks separately. Never describe an unrun Unity test or manual check as passed.

