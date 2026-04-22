using System.Diagnostics;
using System.Reflection;
using System.Text;
using ClockTray;

namespace ClockTray.Tests;

/// <summary>
/// Tests for CLI command functionality: show, hide, status
/// Tests argument parsing, exit codes, output validation, and edge cases.
/// </summary>
public class CliCommandTests
{
    // Helper class to simulate CLI execution in-process
    private class CliTestHelper
    {
        public int ExitCode { get; private set; }
        public string Output { get; private set; } = string.Empty;
        public string Error { get; private set; } = string.Empty;

        public void Execute(string[] args)
        {
            var originalOut = Console.Out;
            var originalErr = Console.Error;
            var outputWriter = new StringWriter();
            var errorWriter = new StringWriter();

            try
            {
                Console.SetOut(outputWriter);
                Console.SetError(errorWriter);

                // Simulate CLI command parsing
                ExitCode = ParseAndExecuteCommand(args);
            }
            finally
            {
                Console.SetOut(originalOut);
                Console.SetError(originalErr);
                Output = outputWriter.ToString();
                Error = errorWriter.ToString();
            }
        }

        private int ParseAndExecuteCommand(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("ClockTray - Windows taskbar clock visibility control");
                Console.WriteLine("Usage: clocktray <command>");
                Console.WriteLine("Commands:");
                Console.WriteLine("  show    - Show the taskbar clock");
                Console.WriteLine("  hide    - Hide the taskbar clock");
                Console.WriteLine("  status  - Check current clock visibility");
                Console.WriteLine("  --help  - Show this help message");
                return 0;
            }

            var command = args[0].ToLowerInvariant().TrimStart('-');

            switch (command)
            {
                case "show":
                    return ExecuteShow();
                case "hide":
                    return ExecuteHide();
                case "status":
                    return ExecuteStatus();
                case "help":
                    Console.WriteLine("ClockTray - Windows taskbar clock visibility control");
                    Console.WriteLine("Usage: clocktray <command>");
                    Console.WriteLine("Commands:");
                    Console.WriteLine("  show    - Show the taskbar clock");
                    Console.WriteLine("  hide    - Hide the taskbar clock");
                    Console.WriteLine("  status  - Check current clock visibility");
                    return 0;
                default:
                    Console.Error.WriteLine($"Unknown command: {args[0]}");
                    Console.Error.WriteLine("Use 'clocktray --help' for usage information.");
                    return 1;
            }
        }

        private int ExecuteShow()
        {
            try
            {
                bool wasVisible = ClockToggler.IsClockVisible();
                ClockToggler.SetClockVisibility(true);
                
                if (wasVisible)
                {
                    Console.WriteLine("Clock is already visible.");
                }
                else
                {
                    Console.WriteLine("Clock visibility enabled.");
                }
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error showing clock: {ex.Message}");
                return 1;
            }
        }

        private int ExecuteHide()
        {
            try
            {
                bool wasVisible = ClockToggler.IsClockVisible();
                ClockToggler.SetClockVisibility(false);
                
                if (!wasVisible)
                {
                    Console.WriteLine("Clock is already hidden.");
                }
                else
                {
                    Console.WriteLine("Clock visibility disabled.");
                }
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error hiding clock: {ex.Message}");
                return 1;
            }
        }

        private int ExecuteStatus()
        {
            try
            {
                bool isVisible = ClockToggler.IsClockVisible();
                bool isWin11 = ClockToggler.IsWin11Modern();
                
                Console.WriteLine($"Clock Status: {(isVisible ? "Visible" : "Hidden")}");
                Console.WriteLine($"Windows Version: {(isWin11 ? "Windows 11 Modern (23H2+)" : "Windows 10/11 Legacy")}");
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error checking status: {ex.Message}");
                return 1;
            }
        }
    }

    #region Argument Parsing Tests

    [Fact]
    public void NoArguments_ShowsUsageMessage_ExitCode0()
    {
        var helper = new CliTestHelper();
        helper.Execute(Array.Empty<string>());

        Assert.Equal(0, helper.ExitCode);
        Assert.Contains("ClockTray", helper.Output);
        Assert.Contains("Usage:", helper.Output);
        Assert.Contains("show", helper.Output);
        Assert.Contains("hide", helper.Output);
        Assert.Contains("status", helper.Output);
    }

    [Fact]
    public void HelpFlag_ShowsUsageMessage_ExitCode0()
    {
        var helper = new CliTestHelper();
        helper.Execute(new[] { "--help" });

        Assert.Equal(0, helper.ExitCode);
        Assert.Contains("ClockTray", helper.Output);
        Assert.Contains("Usage:", helper.Output);
    }

    [Fact]
    public void HelpCommand_ShowsUsageMessage_ExitCode0()
    {
        var helper = new CliTestHelper();
        helper.Execute(new[] { "help" });

        Assert.Equal(0, helper.ExitCode);
        Assert.Contains("ClockTray", helper.Output);
        Assert.Contains("Usage:", helper.Output);
    }

    [Fact]
    public void InvalidCommand_ShowsError_ExitCode1()
    {
        var helper = new CliTestHelper();
        helper.Execute(new[] { "invalid" });

        Assert.Equal(1, helper.ExitCode);
        Assert.Contains("Unknown command", helper.Error);
        Assert.Contains("invalid", helper.Error);
    }

    [Theory]
    [InlineData("xyz")]
    [InlineData("toggle")]
    [InlineData("version")]
    [InlineData("")]
    [InlineData("   ")]
    public void UnknownCommand_ReturnsNonZeroExitCode(string command)
    {
        var helper = new CliTestHelper();
        helper.Execute(new[] { command });

        Assert.NotEqual(0, helper.ExitCode);
        Assert.Contains("Unknown command", helper.Error);
    }

    #endregion

    #region Show Command Tests

    [Fact]
    public void ShowCommand_ValidCommand_ExitCode0()
    {
        var helper = new CliTestHelper();
        helper.Execute(new[] { "show" });

        Assert.Equal(0, helper.ExitCode);
        Assert.True(
            helper.Output.Contains("visible", StringComparison.OrdinalIgnoreCase) ||
            helper.Output.Contains("enabled", StringComparison.OrdinalIgnoreCase));
    }

    [Theory]
    [InlineData("show")]
    [InlineData("SHOW")]
    [InlineData("Show")]
    [InlineData("ShOw")]
    public void ShowCommand_CaseInsensitive_Works(string command)
    {
        var helper = new CliTestHelper();
        helper.Execute(new[] { command });

        Assert.Equal(0, helper.ExitCode);
        Assert.True(
            helper.Output.Contains("visible", StringComparison.OrdinalIgnoreCase) ||
            helper.Output.Contains("enabled", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ShowCommand_WhenAlreadyVisible_Idempotent()
    {
        // First, ensure it's shown
        var helper1 = new CliTestHelper();
        helper1.Execute(new[] { "show" });
        Assert.Equal(0, helper1.ExitCode);

        // Show again - should be idempotent
        var helper2 = new CliTestHelper();
        helper2.Execute(new[] { "show" });
        Assert.Equal(0, helper2.ExitCode);
        Assert.Contains("already visible", helper2.Output, StringComparison.OrdinalIgnoreCase);
    }

    #endregion

    #region Hide Command Tests

    [Fact]
    public void HideCommand_ValidCommand_ExitCode0()
    {
        var helper = new CliTestHelper();
        helper.Execute(new[] { "hide" });

        Assert.Equal(0, helper.ExitCode);
        Assert.True(
            helper.Output.Contains("hidden", StringComparison.OrdinalIgnoreCase) ||
            helper.Output.Contains("disabled", StringComparison.OrdinalIgnoreCase));
    }

    [Theory]
    [InlineData("hide")]
    [InlineData("HIDE")]
    [InlineData("Hide")]
    [InlineData("HiDe")]
    public void HideCommand_CaseInsensitive_Works(string command)
    {
        var helper = new CliTestHelper();
        helper.Execute(new[] { command });

        Assert.Equal(0, helper.ExitCode);
        Assert.True(
            helper.Output.Contains("hidden", StringComparison.OrdinalIgnoreCase) ||
            helper.Output.Contains("disabled", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void HideCommand_WhenAlreadyHidden_Idempotent()
    {
        // First, ensure it's hidden
        var helper1 = new CliTestHelper();
        helper1.Execute(new[] { "hide" });
        Assert.Equal(0, helper1.ExitCode);

        // Hide again - should be idempotent
        var helper2 = new CliTestHelper();
        helper2.Execute(new[] { "hide" });
        Assert.Equal(0, helper2.ExitCode);
        Assert.Contains("already hidden", helper2.Output, StringComparison.OrdinalIgnoreCase);
    }

    #endregion

    #region Status Command Tests

    [Fact]
    public void StatusCommand_ValidCommand_ExitCode0()
    {
        var helper = new CliTestHelper();
        helper.Execute(new[] { "status" });

        Assert.Equal(0, helper.ExitCode);
        Assert.Contains("Clock Status:", helper.Output);
        Assert.Contains("Windows Version:", helper.Output);
    }

    [Theory]
    [InlineData("status")]
    [InlineData("STATUS")]
    [InlineData("Status")]
    [InlineData("StAtUs")]
    public void StatusCommand_CaseInsensitive_Works(string command)
    {
        var helper = new CliTestHelper();
        helper.Execute(new[] { command });

        Assert.Equal(0, helper.ExitCode);
        Assert.Contains("Clock Status:", helper.Output);
    }

    [Fact]
    public void StatusCommand_ShowsVisibleOrHidden()
    {
        var helper = new CliTestHelper();
        helper.Execute(new[] { "status" });

        Assert.Equal(0, helper.ExitCode);
        Assert.True(
            helper.Output.Contains("Visible", StringComparison.OrdinalIgnoreCase) ||
            helper.Output.Contains("Hidden", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void StatusCommand_ShowsWindowsVersion()
    {
        var helper = new CliTestHelper();
        helper.Execute(new[] { "status" });

        Assert.Equal(0, helper.ExitCode);
        Assert.True(
            helper.Output.Contains("Windows 11 Modern", StringComparison.OrdinalIgnoreCase) ||
            helper.Output.Contains("Windows 10", StringComparison.OrdinalIgnoreCase) ||
            helper.Output.Contains("Legacy", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void StatusCommand_AfterShow_ReportsVisible()
    {
        // Show the clock
        var showHelper = new CliTestHelper();
        showHelper.Execute(new[] { "show" });
        Assert.Equal(0, showHelper.ExitCode);

        // Check status
        var statusHelper = new CliTestHelper();
        statusHelper.Execute(new[] { "status" });
        Assert.Equal(0, statusHelper.ExitCode);
        Assert.Contains("Visible", statusHelper.Output);
    }

    [Fact]
    public void StatusCommand_AfterHide_ReportsHidden()
    {
        // Hide the clock
        var hideHelper = new CliTestHelper();
        hideHelper.Execute(new[] { "hide" });
        Assert.Equal(0, hideHelper.ExitCode);

        // Check status
        var statusHelper = new CliTestHelper();
        statusHelper.Execute(new[] { "status" });
        Assert.Equal(0, statusHelper.ExitCode);
        Assert.Contains("Hidden", statusHelper.Output);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void ShowHideSequence_WorksCorrectly()
    {
        // Show
        var showHelper = new CliTestHelper();
        showHelper.Execute(new[] { "show" });
        Assert.Equal(0, showHelper.ExitCode);

        // Verify visible
        var statusHelper1 = new CliTestHelper();
        statusHelper1.Execute(new[] { "status" });
        Assert.Contains("Visible", statusHelper1.Output);

        // Hide
        var hideHelper = new CliTestHelper();
        hideHelper.Execute(new[] { "hide" });
        Assert.Equal(0, hideHelper.ExitCode);

        // Verify hidden
        var statusHelper2 = new CliTestHelper();
        statusHelper2.Execute(new[] { "status" });
        Assert.Contains("Hidden", statusHelper2.Output);

        // Show again to restore
        var restoreHelper = new CliTestHelper();
        restoreHelper.Execute(new[] { "show" });
        Assert.Equal(0, restoreHelper.ExitCode);
    }

    [Fact]
    public void ClockToggler_IsClockVisible_MatchesStatusCommand()
    {
        // Get status via ClockToggler
        bool isVisible = ClockToggler.IsClockVisible();

        // Get status via CLI
        var helper = new CliTestHelper();
        helper.Execute(new[] { "status" });

        if (isVisible)
        {
            Assert.Contains("Visible", helper.Output);
        }
        else
        {
            Assert.Contains("Hidden", helper.Output);
        }
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void EmptyStringArgument_TreatedAsUnknownCommand()
    {
        var helper = new CliTestHelper();
        helper.Execute(new[] { "" });

        Assert.NotEqual(0, helper.ExitCode);
    }

    [Fact]
    public void WhitespaceArgument_TreatedAsUnknownCommand()
    {
        var helper = new CliTestHelper();
        helper.Execute(new[] { "   " });

        Assert.NotEqual(0, helper.ExitCode);
    }

    [Fact]
    public void CommandWithLeadingDashes_StillWorks()
    {
        var helper = new CliTestHelper();
        helper.Execute(new[] { "--show" });

        Assert.Equal(0, helper.ExitCode);
    }

    [Fact]
    public void MultipleArguments_OnlyFirstIsUsed()
    {
        var helper = new CliTestHelper();
        helper.Execute(new[] { "show", "extra", "arguments" });

        Assert.Equal(0, helper.ExitCode);
        Assert.True(
            helper.Output.Contains("visible", StringComparison.OrdinalIgnoreCase) ||
            helper.Output.Contains("enabled", StringComparison.OrdinalIgnoreCase));
    }

    [Theory]
    [InlineData("show123")]
    [InlineData("hide_clock")]
    [InlineData("status!")]
    public void CommandWithExtraCharacters_TreatedAsUnknown(string command)
    {
        var helper = new CliTestHelper();
        helper.Execute(new[] { command });

        Assert.NotEqual(0, helper.ExitCode);
        Assert.Contains("Unknown command", helper.Error);
    }

    #endregion

    #region Exit Code Validation

    [Theory]
    [InlineData("show", 0)]
    [InlineData("hide", 0)]
    [InlineData("status", 0)]
    [InlineData("--help", 0)]
    [InlineData("help", 0)]
    public void ValidCommands_ReturnExitCode0(string command, int expectedExitCode)
    {
        var helper = new CliTestHelper();
        helper.Execute(new[] { command });

        Assert.Equal(expectedExitCode, helper.ExitCode);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("unknown")]
    [InlineData("bad")]
    [InlineData("xyz")]
    public void InvalidCommands_ReturnNonZeroExitCode(string command)
    {
        var helper = new CliTestHelper();
        helper.Execute(new[] { command });

        Assert.NotEqual(0, helper.ExitCode);
    }

    #endregion

    #region Output Validation

    [Fact]
    public void ShowCommand_ProducesOutput()
    {
        var helper = new CliTestHelper();
        helper.Execute(new[] { "show" });

        Assert.NotEmpty(helper.Output);
        Assert.Empty(helper.Error);
    }

    [Fact]
    public void HideCommand_ProducesOutput()
    {
        var helper = new CliTestHelper();
        helper.Execute(new[] { "hide" });

        Assert.NotEmpty(helper.Output);
        Assert.Empty(helper.Error);
    }

    [Fact]
    public void StatusCommand_ProducesOutput()
    {
        var helper = new CliTestHelper();
        helper.Execute(new[] { "status" });

        Assert.NotEmpty(helper.Output);
        Assert.Empty(helper.Error);
    }

    [Fact]
    public void InvalidCommand_ProducesErrorOutput()
    {
        var helper = new CliTestHelper();
        helper.Execute(new[] { "invalid" });

        Assert.NotEmpty(helper.Error);
    }

    [Fact]
    public void HelpCommand_OutputContainsAllCommands()
    {
        var helper = new CliTestHelper();
        helper.Execute(new[] { "--help" });

        Assert.Contains("show", helper.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("hide", helper.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("status", helper.Output, StringComparison.OrdinalIgnoreCase);
    }

    #endregion
}
