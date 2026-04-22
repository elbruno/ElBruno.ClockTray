# CLI Command Test Plan - ClockTray

**Date:** 2025-01-29  
**Author:** Dillon (QA/Tester)  
**Status:** Initial Implementation Complete

## Overview
Comprehensive xUnit test suite for ClockTray CLI commands: `show`, `hide`, and `status`.

## Test Coverage

### 1. Argument Parsing (10 tests)
- ✅ No arguments → shows usage
- ✅ `--help` flag → shows usage
- ✅ `help` command → shows usage
- ✅ Invalid commands → error message + exit code 1
- ✅ Unknown commands (theory: xyz, toggle, version, empty, whitespace)

### 2. Show Command (5 tests)
- ✅ Valid show command → exit code 0
- ✅ Case insensitivity (theory: show, SHOW, Show, ShOw)
- ✅ Idempotency: show when already visible → "already visible" message

### 3. Hide Command (5 tests)
- ✅ Valid hide command → exit code 0
- ✅ Case insensitivity (theory: hide, HIDE, Hide, HiDe)
- ✅ Idempotency: hide when already hidden → "already hidden" message

### 4. Status Command (7 tests)
- ✅ Valid status command → exit code 0
- ✅ Case insensitivity (theory: status, STATUS, Status, StAtUs)
- ✅ Output contains "Visible" or "Hidden"
- ✅ Output contains Windows version info
- ✅ Status after show → reports "Visible"
- ✅ Status after hide → reports "Hidden"

### 5. Integration Tests (2 tests)
- ✅ Show → status → hide → status → show sequence
- ✅ ClockToggler.IsClockVisible() matches status command output

### 6. Edge Cases (6 tests)
- ✅ Empty string argument
- ✅ Whitespace-only argument
- ✅ Command with leading dashes (--show)
- ✅ Multiple arguments (only first used)
- ✅ Commands with extra characters (theory: show123, hide_clock, status!)

### 7. Exit Code Validation (3 tests)
- ✅ Valid commands return 0 (theory: show, hide, status, --help, help)
- ✅ Invalid commands return non-zero (theory: invalid, unknown, bad, xyz)

### 8. Output Validation (6 tests)
- ✅ Show produces stdout, no stderr
- ✅ Hide produces stdout, no stderr
- ✅ Status produces stdout, no stderr
- ✅ Invalid command produces stderr
- ✅ Help output contains all commands

## Total Tests: 44 test cases

## Test Architecture

### CliTestHelper Class
- **Purpose:** In-process CLI simulation for testing without launching external processes
- **Features:**
  - Captures stdout and stderr separately
  - Returns exit codes
  - Simulates full CLI argument parsing and command execution
  - Uses actual ClockToggler methods for integration

### Key Design Decisions
1. **In-process testing:** No subprocess spawning for faster, deterministic tests
2. **String output validation:** All outputs captured and validated
3. **Idempotency testing:** Critical for CLI tools - same command twice should be safe
4. **Case insensitivity:** Windows convention - all commands work regardless of case
5. **Exit codes:** Follow Unix conventions (0 = success, non-zero = error)

## Test Data Strategy
- **Theory tests** for case variations and invalid inputs
- **Sequential integration tests** to verify state changes
- **Edge cases** for malformed input

## Known Limitations & Future Tests

### Registry Mocking
- **Current:** Tests run against actual Windows registry (requires admin/user permissions)
- **Future:** Consider mocking registry access for isolated tests
- **Rationale:** Real registry testing validates Win11 vs Win10 behavior, but requires Windows environment

### Performance Testing
- Not included in this suite
- CLI commands should complete in < 1 second

### Multi-monitor / DPI Testing
- Out of scope for CLI tests
- Handled at ClockToggler level

### Hotkey Conflict Testing
- Not applicable to CLI (only system tray app uses hotkeys)

## Test Execution

### Prerequisites
- Windows 10 or Windows 11
- .NET 10.0 SDK
- xUnit test runner

### Run All Tests
```bash
cd D:\elbruno\ElBruno.ClockTray\ClockTray.Tests
dotnet test
```

### Run CLI Tests Only
```bash
dotnet test --filter "FullyQualifiedName~CliCommandTests"
```

### Expected Results
- All 44 tests should pass on both Windows 10 and Windows 11
- Tests are deterministic and repeatable
- No external dependencies or test order dependencies

## Quality Criteria

### Test Quality Checklist
- ✅ Each test has single, clear assertion
- ✅ Test names describe behavior, not implementation
- ✅ Theory tests reduce duplication
- ✅ Tests are fast (< 1s each)
- ✅ Tests are isolated (no shared state)
- ✅ Tests restore state (show at end of hide tests)

### Coverage Goals
- ✅ All CLI commands covered
- ✅ All exit codes validated
- ✅ All user-facing messages validated
- ✅ Case sensitivity edge cases
- ✅ Idempotency verified
- ✅ Integration with ClockToggler validated

## Next Steps

### Before Merge
1. Run full test suite on Windows 10 and Windows 11
2. Verify tests pass in CI/CD pipeline
3. Code review by Mac (Lead Dev)

### Future Enhancements
1. Add performance benchmarks (if needed)
2. Consider mocking registry for faster CI
3. Add tests for future CLI commands (e.g., `clocktray toggle`)

## Sign-off
**Tester:** Dillon  
**Status:** ✅ Ready for Review  
**Confidence:** High - comprehensive coverage of all requirements
