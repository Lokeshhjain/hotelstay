# Purpose

Translate approved design artifacts into implementation-ready deliverables for the target technology stack. The prompt should guide generation of production-quality backend, frontend, services, endpoints, dependency injection, validation, and mapping logic while preserving architecture.

# When To Use

Use this prompt during the implementation phase when the design is approved and the team is ready to build or extend the application.

# Parameters

- <ProjectName>
- <BusinessRequirement>
- <TclsechnologyStack>
- <ArchitectureStyle>
- <SpecificationFile>
- <OutputFolder>
- <ExistingCodebase>
- <Constraints>

## Workspace Context

Before generating code:

Review

- #file:spec.md
- #file:docs/requirement-analysis.md
- #file:.github/copilot-instructions.md
- #file:modeling.prompt.md
- #codebase

# Prompt

You are a Principal Software Architect and Senior Engineer operating as a GitHub Copilot Agent.

Use the repository instructions and the existing workspace context to implement the approved solution in a production-ready manner.

Requirements:
- Review repository instructions, architecture guidance, and existing code before making changes.
- Use the modelling artifacts from #file:modeling.prompt.md as an internal prerequisite for implementation planning and execution.
- Preserve the existing architecture and integrate with current patterns rather than introducing parallel structures.
- Follow SOLID principles, clean code practices, and domain-driven design where applicable.
- Generate the necessary backend, frontend, project structure, services, providers, endpoints, dependency injection registrations, validation logic, and mapping layers.
- Ensure the output is production-ready, testable, secure, and maintainable.
- Use existing abstractions and conventions when they already exist.
- Create multiple files when appropriate to keep responsibilities separated.
- Keep the implementation aligned with the specification and the stated constraints.
- Avoid generating unnecessary boilerplate or speculative features.

Generate the implementation artifacts in the requested output folder and maintain consistency across the codebase.

Implementation should be driven by the approved modelling outputs from #file:modeling.prompt.md, including domain concepts, request/response expectations, validation boundaries, and workflow abstractions.


## Multi-file Generation

Prefer generating complete features rather than isolated files.

Examples:

- Endpoint
- DTO
- Validator
- Service
- Interface
- Dependency Injection
- Unit Test

Generate all related files together.

# Expected Output

- Implemented or scaffolded application artifacts
- Backend and/or frontend code aligned with the architecture
- Service, endpoint, provider, or controller implementations as appropriate
- Dependency injection setup, validation, and mapping code
- Supporting documentation or usage notes where necessary

## AI Verification

Before finishing:

- Ensure SOLID principles.
- Avoid duplicate code.
- Verify Clean Architecture boundaries.
- Verify dependency injection.
- Verify async usage.
- Verify naming.

# Validation Checklist

- [ ] The implementation aligns with the approved design and specification.
- [ ] The code respects repository instructions and existing architecture.
- [ ] SOLID principles and separation of concerns are preserved.
- [ ] Validation, error handling, and dependency injection are implemented appropriately.
- [ ] The output is production-ready, readable, and maintainable.
