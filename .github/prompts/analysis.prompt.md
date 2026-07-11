# Purpose

Produce a business-focused requirement analysis document for the Analysis phase of a Specification Driven Development workflow. The output must capture the business requirement accurately, preserve the source intent, and remain free of implementation guidance.

# When To Use

Use this prompt during the Analysis phase when a new feature, change request, or initiative needs to be translated into a structured business requirements artifact before Design begins.

# Execution Mode

Execute this prompt in GitHub Copilot Agent Mode.

- Treat the attached Business Requirement PDF as the primary source of truth.
- Use `#file:.github/copilot-instructions.md` only for repository conventions, formatting, and development standards.
- Do not convert repository instructions into business requirements.
- Work autonomously, preserve existing terminology where relevant, and avoid introducing unsupported assumptions.

# Workspace Context

Review the following before generating the analysis:

- `#file:.github/copilot-instructions.md` for repository conventions and formatting only
- The attached Business Requirement PDF as the authoritative business source
- `#codebase` to understand any existing documentation or terminology
- `#selection` when updating an existing document to preserve unchanged sections

# Inputs

- <ProjectName>
- <BusinessRequirement>
- <TechnologyStack>
- <ArchitectureStyle>
- <SpecificationFile>
- <OutputFolder>
- <ExistingCodebase>
- <Constraints>

# Execution Instructions

You are a Senior Business Analyst and Requirements Engineer working in GitHub Copilot Agent Mode.

1. Read the attached Business Requirement PDF first and treat it as the authoritative source for all business analysis.
2. Use `#file:.github/copilot-instructions.md` only to preserve repository conventions, structure, and documentation standards; do not transform these instructions into business requirements.
3. Review existing workspace documentation and terminology to maintain consistency where appropriate.
4. Extract only business-analysis content from the source document and organize it into a detailed, review-ready structure with the following sections:
   - Executive Summary
   - Problem Statement
   - Business Objectives
   - Scope
   - Stakeholders
   - User Roles
   - Functional Requirements
   - API Requirements
   - Business Rules
   - Validation Rules
   - Non-Functional Requirements
   - Assumptions
   - Constraints
   - Risks
   - Dependencies
   - Acceptance Criteria
   - Requirement Traceability Matrix
   - Glossary
   - Open Questions
5. Write each section in a business-analysis style with sufficient depth for design handoff. Do not produce a shallow summary; each section should be substantive and specific.
6. Separate functional requirements from business rules and keep business rules distinct from non-functional requirements.
7. Capture every explicitly stated requirement from the attached PDF. If a requirement is not explicitly stated, record it as an open question or leave it out rather than inventing it.
8. Preserve the source wording where possible and avoid generalizing into a typical product scenario that is not supported by the PDF.
9. Use clear requirement identifiers for traceability, such as FR-001, BR-001, NFR-001, and similar where appropriate.
10. Include explicit detail for scope by listing both in-scope and out-of-scope items where supported by the source.
11. Include stakeholder and user-role sections when the source document defines them or implies a business audience.
12. Where the source describes behaviors, rules, or constraints, state them as business requirements rather than implementation steps.
13. Do not include implementation guidance such as:
   - Clean Architecture
   - Minimal API
   - Dependency Injection
   - Repository Pattern
   - CQRS
   - SOLID
   - Logging implementation
   - Technology-specific coding practices
14. Do not introduce solution design, implementation strategy, or coding patterns in the analysis document.
15. Produce a concise, complete, and review-ready analysis artifact suitable for handoff to Design.

# Output

Generate or update a requirement analysis document in the requested output folder containing:

- An executive summary and problem statement
- A clear business objective summary
- A detailed scope statement with in-scope and out-of-scope items
- Stakeholder and user-role sections
- A structured set of functional requirements with traceable identifiers
- A separate section for API requirements
- A separate section for business rules
- A separate section for validation rules
- A separate section for non-functional requirements
- Assumptions, constraints, risks, dependencies, and open questions
- Acceptance criteria that map to the functional requirements
- A requirement traceability matrix
- A glossary of key terms
- Content that is detailed enough to support downstream design and review

# AI Verification

Before completing the task, verify that:

- Every requirement from the attached PDF is captured.
- Functional Requirements are separated from Business Rules.
- Business Rules are separated from Non-Functional Requirements.
- The analysis includes the required business-analysis sections in a detailed structure.
- Acceptance Criteria map back to Functional Requirements.
- Stakeholders, user roles, assumptions, constraints, risks, dependencies, and open questions are documented where the source supports them.
- No unsupported assumptions are introduced.
- Repository instructions are not transformed into business requirements.
- The document remains focused on business analysis and does not drift into implementation guidance.

# Validation Checklist

- [ ] The document is grounded in the attached Business Requirement PDF as the primary source of truth.
- [ ] The analysis contains all required sections for business analysis.
- [ ] The analysis is detailed enough for downstream design review rather than being a shallow summary.
- [ ] Functional Requirements, Business Rules, and Non-Functional Requirements are clearly separated.
- [ ] API Requirements are explicitly captured.
- [ ] Validation Rules, assumptions, constraints, risks, dependencies, and open questions are documented.
- [ ] Acceptance Criteria trace back to the Functional Requirements.
- [ ] No implementation guidance or technology-specific coding practices are included.
- [ ] The output is complete, reviewable, and suitable for the Design phase.
