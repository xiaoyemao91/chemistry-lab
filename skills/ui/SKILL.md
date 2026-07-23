---
name: ui
description: Design, implement, or review Unity 2D user interfaces for the ICP-OES teaching simulation, including menus, workflow pages, parameter forms, instrument status, feedback dialogs, charts, records, accessibility, and photo-based interactions. Use for scenes, prefabs, Assets/Scripts/UI, or visual interaction changes.
---

# Simulation UI

Read `REQUIREMENTS.md`, `ARCHITECTURE.md`, and `CODING_RULES.md` before changing scenes or UI scripts.

## Workflow

1. Define the current step, student goal, accepted actions, error states, and recovery path.
2. Reuse existing prefabs, typography, spacing, colors, and interaction patterns.
3. Bind UI to application state and commands; do not place workflow rules or calculations in event handlers.
4. Show units beside every scientific input and provide immediate, specific validation feedback.
5. Express state with text or icons in addition to color; preserve keyboard navigation and readable contrast.
6. Verify normal flow, invalid input, duplicate clicks, resizing, reset, and offline behavior in PlayMode.

## Assets

Use only authorized, appropriately licensed, and privacy-reviewed photos or audio. Keep temporary or unapproved laboratory imagery outside version control.

