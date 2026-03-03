# Publishing ElBruno.ClockTray to NuGet

This document describes how to publish the **ElBruno.ClockTray** package to NuGet using GitHub Actions with OIDC-based Trusted Publishing.

## How It Works

```
GitHub Release (tag v0.1.0)
        │
        ▼
GitHub Actions workflow
        │
        ├─ Determine version (tag → input → csproj)
        ├─ dotnet restore / build / pack
        ├─ NuGet OIDC login (trusted publishing)
        └─ dotnet nuget push --skip-duplicate
```

## Prerequisites

### 1. NuGet Trusted Publishing Setup

1. Go to [nuget.org](https://www.nuget.org/) → Manage Packages → **ElBruno.ClockTray**
2. Navigate to **Trusted publishers** tab
3. Add a new trusted publisher:
   - **Repository owner:** `elbruno`
   - **Repository name:** `ElBruno.ClockTray`
   - **Workflow:** `publish.yml`
   - **Environment:** `release`

### 2. GitHub Environment Setup

1. Go to **Settings → Environments** in the `elbruno/ElBruno.ClockTray` repository
2. Create an environment named **`release`**
3. Optionally add protection rules (required reviewers, deployment branches)

## Publishing

### Via GitHub Release (recommended)

1. Go to **Releases → Draft a new release**
2. Create a tag like `v0.1.0`
3. Publish the release
4. The workflow triggers automatically and publishes to NuGet

### Via Manual Dispatch

1. Go to **Actions → Publish to NuGet**
2. Click **Run workflow**
3. Optionally enter a version (defaults to csproj version)

## Version Resolution Priority

| Priority | Source | Example |
|----------|--------|---------|
| 1 | Release tag | `v0.1.0` → `0.1.0` |
| 2 | Manual workflow input | `0.2.0-beta` |
| 3 | `<Version>` in csproj | `0.1.0` |

## Troubleshooting

| Issue | Cause | Fix |
|-------|-------|-----|
| `403 Forbidden` on push | Trusted publisher not configured | Add trusted publisher on nuget.org |
| `409 Conflict` | Package version already exists | `--skip-duplicate` handles this; bump version |
| OIDC token failure | Environment mismatch | Ensure workflow uses `environment: release` |
| Build failure | Missing SDK | Ensure `dotnet-version: 10.0.x` in workflow |

## References

- [NuGet Trusted Publishing](https://devblogs.microsoft.com/nuget/introducing-trusted-publishers/)
- [NuGet/login Action](https://github.com/NuGet/login)
- [GitHub OIDC](https://docs.github.com/en/actions/deployment/security-hardening-your-deployments/about-security-hardening-with-openid-connect)
- [NuGet Package: ElBruno.ClockTray](https://www.nuget.org/packages/ElBruno.ClockTray)
