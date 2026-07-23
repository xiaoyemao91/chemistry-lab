---
name: documentation
description: Create or update project documentation for the ICP-OES Unity simulation, including requirements, architecture, interfaces, coding rules, roadmap, content schema, teacher input checklist, acceptance tests, README, and changelog. Use after verified behavior, scope, schema, workflow, or development process changes.
---

# Project Documentation

Treat implemented code, verified tests, approved teacher material, and explicit user decisions as primary evidence.

## Workflow

1. Identify the verified change and the documents it affects.
2. Separate current behavior, planned behavior, assumptions, and unresolved scientific inputs.
3. Update the smallest authoritative document set; link instead of duplicating long details.
4. Keep requirements testable, architecture aligned with real dependencies, and API descriptions aligned with code.
5. Add a concise entry to `CHANGELOG.md` for user-visible or structural changes.
6. Check links, paths, versions, terminology, and contradiction across documents.

## Integrity

- Do not turn a plan into a claim of completed work.
- Do not present synthetic or placeholder values as teacher-approved science.
- Do not add private identities, serial numbers, credentials, or unauthorized lab material.
- Keep instructions compatible with Windows, PowerShell, Unity 2022.3 LTS, and the actual repository.

