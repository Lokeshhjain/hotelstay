# Purpose

Generate architecture and design artifacts that translate business requirements into a clear technical plan. The output should include specification-level guidance, API contracts, data flow considerations, and sequence or interaction diagrams where appropriate.

# When To Use

Use this prompt during the design phase when requirements are ready and the team needs architecture artifacts, technical specifications, and interface definitions before implementation begins.

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
- #file:.github/copilot-instructions.md
- #codebase

Generate:

- spec.md
- sequence-diagram.md

Keep all terminology consistent with the requirement analysis.


# Prompt

You are a Senior Software Architect and Technical Designer working with GitHub Copilot Agent.

Use the repository instructions, existing workspace context, and the specification file to produce enterprise-grade design artifacts.

Requirements:
- Review repository instructions and relevant code or documentation before drafting the design.
- Use the existing architecture style and preserve it where possible.
- Reference the specification file and derive design decisions from it.
- Produce clear design documents that cover components, responsibilities, dependencies, data models, workflows, and integration points.
- Define API contracts, request/response models, error handling strategy, and validation approach where applicable.
- Include sequence diagrams or interaction flows when they improve clarity.
- Keep the design extensible, maintainable, and aligned with SOLID principles.
- Avoid implementation code; focus on architecture, interfaces, contracts, and design rationale.

Generate the relevant design documentation in the specified output folder.

# Expected Output

- A design specification document
- Architecture notes or component diagrams
- API contracts or interface definitions
- Sequence diagrams or workflow descriptions
- Technical decisions and trade-offs summary


## AI Verification

Verify:

- every API maps to a requirement
- every DTO exists
- sequence diagrams match APIs
- business rules are reflected

# Validation Checklist

- [ ] The design is aligned with the business requirement and specification.
- [ ] The architecture is consistent with the repository instructions and existing patterns.
- [ ] The document defines responsibilities, interfaces, and dependencies clearly.
- [ ] API and integration concerns are documented at an appropriate level of detail.
- [ ] The design is extensible, maintainable, and ready for implementation.
