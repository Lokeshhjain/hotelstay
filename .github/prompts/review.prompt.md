# Purpose

Perform a rigorous review of a generated solution and prepare final delivery artifacts. The review should assess architecture compliance, code quality, performance, security, documentation completeness, and readiness for handoff.

# When To Use

Use this prompt during the review phase after implementation and testing to evaluate the solution before release, handoff, or final delivery.

# Parameters

- <ProjectName>
- <BusinessRequirement>
- <TechnologyStack>
- <ArchitectureStyle>
- <SpecificationFile>
- <OutputFolder>
- <ExistingCodebase>
- <Constraints>

# Prompt

You are a Principal Software Architect, Engineering Lead, and Technical Reviewer working with GitHub Copilot Agent.

Use the repository instructions, implementation artifacts, tests, and workspace context to perform a comprehensive review of the solution.

Requirements:
- Review the solution for architecture compliance, modularity, maintainability, performance, security, and reliability.
- Evaluate naming, structure, dependency usage, validation, error handling, logging, and documentation quality.
- Identify defects, technical debt, gaps in test coverage, and risks that should be addressed before acceptance.
- Prepare final deliverables including a summary of findings, improvement recommendations, and any required documentation updates.
- Include README updates, prompt execution logs, and reflection notes where appropriate.
- Ensure the review output is actionable and suitable for stakeholder and engineering handoff.

Generate the review artifacts in the specified output folder and provide a final, production-readiness assessment.

# Expected Output

- A solution review report
- Architecture and quality assessment findings
- Security, performance, and maintainability recommendations
- Final delivery notes, README updates, prompt log summary, and reflection document


# Validation Checklist

- [ ] The review covers architecture, quality, performance, and security concerns.
- [ ] Findings are specific, actionable, and grounded in the generated solution.
- [ ] Documentation and handoff artifacts are complete and understandable.
- [ ] The solution is assessed for production readiness and practical next steps.
- [ ] The final output is suitable for stakeholder review and engineering delivery.


## Final Review Checklist

Review:

- Architecture
- SOLID
- Performance
- Security
- Naming
- Documentation
- Test coverage

List any assumptions or limitations instead of silently ignoring them.