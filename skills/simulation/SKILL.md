---
name: simulation
description: Implement or review ICP-OES teaching simulation behavior in this Unity project, including experiment workflow states, instrument commands, parameter handling, measurements, calibration, concentration calculation, and local records. Use for changes under Assets/Scripts/Core or Application and for any task that alters the end-to-end Fe experiment flow.
---

# Simulation

Read `REQUIREMENTS.md`, `ARCHITECTURE.md`, `API.md`, and `CODING_RULES.md` before changing behavior.

## Workflow

1. Define one module's user-visible success criteria and affected workflow states.
2. Identify which scientific inputs are teacher-approved and which remain draft.
3. Keep state, rules, and calculations in plain C# code; keep MonoBehaviour limited to Unity binding.
4. Model student actions as explicit commands and reject invalid commands without partial state changes.
5. Add EditMode tests for domain logic and a focused PlayMode test when Unity interaction changes.
6. Update relevant documentation and `CHANGELOG.md` after verification.

## Boundaries

- Do not invent wavelengths, gas flow, power, concentrations, formulas, thresholds, or instrument behavior.
- Mark placeholder content clearly and prevent draft scientific content from entering release behavior.
- Do not add backend, database, AI, hardware control, or 3D scope to solve an MVP task.
- Preserve deterministic calculations and stable error codes.

## Completion

Report changed files, exact tests run, failures or skipped checks, and the next smallest module. Do not commit or push without separate user approval.

