# Purpose

Generate comprehensive testing artifacts that validate correctness, resilience, and quality across unit, integration, and edge-case scenarios. The prompt should improve confidence in the solution and support meaningful code coverage.

# When To Use

Use this prompt during the testing phase after implementation or when improving an existing solution’s test coverage and regression protection.

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

You are a Senior QA Engineer and Software Test Architect working with GitHub Copilot Agent.

Use the repository instructions, implementation artifacts, and existing workspace context to produce high-quality tests for the solution.

Requirements:
- Review the repository instructions and any relevant implementation files before generating tests.
- Create unit tests, edge case tests, validation tests, and integration tests as appropriate.
- Ensure tests verify real behavior and observable outcomes rather than implementation details.
- Cover success paths, failure paths, invalid input, boundary conditions, and regression scenarios.
- Improve maintainability and coverage without over-mocking or introducing brittle tests.
- Preserve the existing testing style and framework conventions if present.
- Recommend additional validation or coverage improvements where relevant.

Generate the test artifacts in the requested output folder and make them ready for execution.

# Expected Output

- Unit test files
- Integration or end-to-end test artifacts where appropriate
- Edge case and validation test coverage
- A summary of test intent and coverage focus areas

## AI Verification

Ensure:

- Happy path
- Negative path
- Edge cases
- Invalid requests
- Null handling
- Boundary values

# Validation Checklist

- [ ] Tests are aligned with the business requirement and implementation.
- [ ] Critical paths and failure cases are covered.
- [ ] Tests are deterministic, readable, and maintainable.
- [ ] The test suite respects repository instructions and existing conventions.
- [ ] The generated tests improve confidence and coverage without unnecessary complexity.
