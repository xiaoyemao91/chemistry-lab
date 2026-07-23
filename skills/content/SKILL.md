---
name: content
description: Create, validate, or review versioned JSON experiment content for the ICP-OES Unity simulation, including steps, parameters, units, rules, prompts, standards, formulas, and teacher approval metadata. Use for Assets/StreamingAssets/content, content loaders, schemas, migrations, or scientific content review.
---

# Experiment Content

Read `docs/CONTENT_SCHEMA.md` and `docs/TEACHER_INPUT_CHECKLIST.md` before editing content.

## Workflow

1. Record the source document, version, units, and approval status for every scientific value.
2. Use stable kebab-case IDs and explicit `schemaVersion` and `contentVersion` fields.
3. Mark unapproved material as `reviewStatus: "draft"` and use unmistakable placeholder tokens.
4. Validate required fields, duplicate IDs, references, finite numbers, ranges, units, reachable steps, and supported commands.
5. Add valid and invalid configuration tests whenever the schema or loader changes.
6. Update the schema documentation and changelog after verification.

## Safety

- Never infer missing scientific values from generic ICP-OES examples.
- Never store personal data, credentials, instrument serial numbers, or unlicensed photos in content files.
- Do not silently migrate incompatible schemas or accept partially valid experiments.
- Release behavior must reject draft content.

