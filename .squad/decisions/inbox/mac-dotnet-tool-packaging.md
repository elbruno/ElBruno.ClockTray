# Decision: Packaging ClockTray as a .NET Global Tool

**Date:** 2026-03-03  
**Agent:** Mac  
**Status:** Implemented ✅

## Context

Bruno requested ClockTray be installable as a dotnet global tool so users can run `dotnet tool install --global ElBruno.ClockTray` and then launch it with `clocktray` from any terminal.

## Problem

The .NET SDK throws **NETSDK1146** error when `PackAsTool=true` is combined with `UseWindowsForms=true`:

```
error NETSDK1146: PackAsTool does not support TargetPlatformIdentifier being set. 
For example, TargetFramework cannot be net5.0-windows, only net5.0. 
PackAsTool also does not support UseWPF or UseWindowsForms when targeting .NET 5 and higher.
```

This is a hard error thrown from `Microsoft.NET.PackTool.targets` line 294 in the `_PackToolValidation` target. The SDK explicitly blocks packaging WinForms/WPF apps as tools.

## Solution

**Override the SDK validation** using explicit SDK imports and target redefinition:

1. **Change from implicit to explicit SDK imports** in `.csproj`:
   ```xml
   <Project>
     <Import Project="Sdk.props" Sdk="Microsoft.NET.Sdk" />
     <!-- properties and items here -->
     <Import Project="Sdk.targets" Sdk="Microsoft.NET.Sdk" />
     <!-- override targets AFTER SDK targets -->
   </Project>
   ```

2. **Override `_PackToolValidation` target** after SDK imports:
   ```xml
   <Target Name="_PackToolValidation" Condition=" '$(PackAsTool)' == 'true' ">
     <PropertyGroup>
       <_ToolPackShortTargetFrameworkName>net10.0</_ToolPackShortTargetFrameworkName>
       <_ToolPackShortTargetFrameworkName Condition="'$(SelfContained)' == 'true'">any</_ToolPackShortTargetFrameworkName>
     </PropertyGroup>
     <!-- Validation errors removed to allow PackAsTool + UseWindowsForms -->
   </Target>
   ```

3. **Change OutputType** from `WinExe` to `Exe` (required for dotnet tools):
   ```xml
   <OutputType>Exe</OutputType>
   ```

4. **Set tool properties**:
   ```xml
   <PackAsTool>true</PackAsTool>
   <ToolCommandName>clocktray</ToolCommandName>
   ```

## Key Technical Details

- **Why explicit imports work**: The SDK imports happen implicitly at the start/end of `<Project Sdk="...">`. By using explicit `<Import>`, we control the order and can define targets AFTER the SDK's targets, allowing our override to win.
  
- **Why set `_ToolPackShortTargetFrameworkName` directly**: The SDK calculates this via `GetNuGetShortFolderName` task which returns `net10.0-windows7.0` for `net10.0-windows` TFM. Tools require a simple TFM folder structure (`tools/net10.0/any/`) not a platform-specific one. Setting it to `net10.0` directly ensures correct package layout.

- **Console window**: Changing from `WinExe` to `Exe` means a console window briefly appears on startup. This is a trade-off for tool packaging. Could be suppressed with additional Win32 calls if needed.

## Verification

✅ `dotnet pack -c Release` succeeds  
✅ Package created: `ElBruno.ClockTray.0.5.5.nupkg`  
✅ Tool installs: `dotnet tool install --global ElBruno.ClockTray`  
✅ Tool runs: `clocktray` command launches the system tray app  
✅ Build still works: `dotnet build` succeeds

## Files Modified

- `ClockTray.csproj` — explicit SDK imports, PackAsTool config, target override, version 0.5.5
- `README.md` — installation instructions updated to use `dotnet tool install`

## Future Considerations

- Monitor .NET SDK changes — the validation might be relaxed in future versions
- Consider hiding console window with `FreeConsole()` P/Invoke if the brief flash is problematic
- Alternative: create a separate console wrapper project that references the WinForms app, but this approach is simpler

## References

- SDK source: `Microsoft.NET.PackTool.targets` (line 273-294 in .NET SDK 10.0.200)
- Related issue: https://github.com/dotnet/sdk/issues/10346
