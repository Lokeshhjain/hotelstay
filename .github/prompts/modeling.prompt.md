# Purpose

Generate business and system modelling artifacts that clarify domain concepts, data contracts, rules, and workflow boundaries without drifting into implementation code.

# When To Use

Use this prompt when the team needs to define or refine domain models, request/response structures, validation semantics, and workflow abstractions before implementation begins.

# Parameters

- <ProjectName>
- <BusinessRequirement>
- <TechnologyStack>
- <ArchitectureStyle>
- <SpecificationFile>
- <OutputFolder>
- <ExistingCodebase>
- <Constraints>

## Workspace Context

Review

- #file:docs/requirement-analysis.md
- #file:spec.md
- #file:.github/copilot-instructions.md
- #codebase

Generate:

- domain model notes
- API contract outlines
- validation rule summaries
- workflow abstractions

Keep all terminology consistent with the approved requirements.

# Prompt

You are a Senior Business Analyst and Solution Modeller working with GitHub Copilot Agent.

Use the approved requirement analysis and specification to produce clear modelling artifacts that describe the domain, request/response expectations, validation rules, and workflow boundaries.

Requirements:
- Review the requirement analysis, specification, and repository conventions before drafting the model.
- Preserve the approved business language and avoid introducing implementation-specific patterns.
- Capture the core domain concepts, their responsibilities, and the relationships between them.
- Define request and response expectations in a business- and interface-friendly form.
- Clarify validation rules and business constraints without embedding technology details.
- Keep the model extensible and easy to hand off to design and implementation teams.

# Expected Output

- Domain concepts and responsibilities
- Request/response contract summaries
- Validation and rule summaries
- Workflow boundary notes
- Traceability to the approved requirement analysis

## AI Verification

Verify:

- every model element maps to a requirement or rule
- validation rules are explicit and distinct from implementation details
- the modelling artifacts remain aligned to the approved business scope

# Validation Checklist

- [ ] The model aligns with the requirement analysis and specification.
- [ ] The domain concepts are clear and traceable.
- [ ] Validation and workflow boundaries are documented.
- [ ] The output is suitable for downstream design and implementation handoff.
