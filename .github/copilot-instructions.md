# copilot-instructions.md

## Coding Standards

### General Principles
- **Target Framework:** All projects target .NET 8.0. Ensure compatibility with this version.
- **Language:** Use C# for all code unless otherwise specified.
- **Readability:** Write clear, self-explanatory code. Use meaningful variable, method, and class names.
- **Consistency:** Follow consistent naming conventions and code formatting throughout the solution.
- **Comments:** Add inline comments only where necessary to clarify complex logic and in english, always in english.
- **Error Handling:** Use structured exception handling. Avoid swallowing exceptions; log or rethrow as appropriate.
- **SOLID Principles:** Adhere to SOLID design principles for maintainable and extensible code.
- **Dependency Injection:** Prefer constructor injection for dependencies.
- **Async/Await:** Use asynchronous programming patterns where appropriate, especially for I/O-bound operations.
- **Magic Numbers:** Avoid magic numbers; use named constants or enums.
- **File Organization:** One class per file. Organize files into appropriate folders by feature or layer.

### Naming Conventions
- **Classes & Interfaces:** PascalCase (e.g., `UserService`, `IUserRepository`)
- **Methods & Properties:** PascalCase (e.g., `GetUserById`)
- **Private & Public variables:** camelCase (e.g., `_userId`, `UserId`)
- **Parameters:** camelCase (e.g., `userId`)
- **Constants:** PascalCase (e.g., `DefaultTimeout`)
- **Unit Test Methods:** Use descriptive names indicating the scenario and expected outcome (e.g., `GetUserById_ReturnsUser_WhenUserExists`)

### Code Style
- **Braces:** Use Allman style (braces on new lines).
- **Indentation:** Use one tabulation per indentation level.
- **Line Length:** Limit lines to 120 characters.
- **Usings:** Place `using` statements outside the namespace and remove unused usings.

---

## Copilot Usage

- **Adhere to these standards** when generating or modifying code.
- **Prefer existing patterns** and conventions found in the solution.
- **Generate code that is ready to use** and fits seamlessly into the current structure.
- **Document any deviations** from these standards in pull requests or code reviews.

---

## Unit Test Standards

### General Principles
- **Test Framework:** Use XUnit as appropriate for the project.
- **Test Naming:** Use descriptive method names: `MethodName_ExpectedBehavior_StateUnderTest`.
- **Test Structure:** Follow Arrange-Act-Assert (AAA) pattern in all tests.
- **Isolation:** Each test should be independent and not rely on the outcome of other tests.
- **Mocking:** Use mocks or fakes for external dependencies (e.g. APIs) to ensure tests are deterministic.