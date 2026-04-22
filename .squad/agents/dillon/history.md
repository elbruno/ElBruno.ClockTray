# Dillon — History

## Project Context
- **Project:** ClockTray — Windows system tray app to show/hide date and time in the taskbar
- **Stack:** C# / .NET / WPF or WinUI, Win32 interop
- **User:** Bruno Capuano

## Learnings

### 2025-01-29: CLI Command Testing
- **Task:** Created comprehensive xUnit test suite for CLI commands (show, hide, status)
- **Approach:** Built CliTestHelper class for in-process CLI simulation
  - Captures stdout/stderr separately
  - Returns exit codes for validation
  - Uses actual ClockToggler methods for integration testing
- **Test Coverage:** 44 test cases across 8 categories:
  - Argument parsing (10 tests)
  - Show command (5 tests)
  - Hide command (5 tests)
  - Status command (7 tests)
  - Integration tests (2 tests)
  - Edge cases (6 tests)
  - Exit code validation (3 tests)
  - Output validation (6 tests)
- **Key Patterns:**
  - Theory tests for case variations (show, SHOW, Show) - Windows convention
  - Idempotency testing (show when visible, hide when hidden)
  - Sequential integration tests to verify state changes
  - Exit codes follow Unix convention (0 = success, non-zero = error)
- **Architecture Decisions:**
  - In-process testing (no subprocess) for speed and determinism
  - Real registry access (not mocked) to validate Win11 vs Win10 behavior
  - String output validation for all user-facing messages
- **File Created:** `ClockTray.Tests\CliCommandTests.cs`
- **Documentation:** Test plan written to `.squad\decisions\inbox\dillon-cli-test-plan.md`
