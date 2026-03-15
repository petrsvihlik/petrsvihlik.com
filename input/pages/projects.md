---
title: "Projects"
slug: projects
description: "Petr's hobby projects"
show_in_navigation: true
---

## WopiHost

<img src="/assets/img/wopihost-logo.png" alt="WopiHost logo" class="project-logo">

[WopiHost](https://github.com/petrsvihlik/WopiHost) is the most advanced open-source .NET implementation of the [MS-WOPI protocol](https://learn.microsoft.com/en-us/microsoft-365/cloud-storage-partner-program/rest/), enabling seamless integration with WOPI clients such as **Office Online Server** and **Microsoft 365**.

It ships as a set of focused NuGet packages so you only pull in what you need:

| Package | Purpose |
|---|---|
| `WopiHost.Core` | ASP.NET Core middleware & WOPI endpoints |
| `WopiHost.Abstractions` | Interfaces for custom storage & lock providers |
| `WopiHost.FileSystemProvider` | Ready-to-use filesystem storage provider |
| `WopiHost.MemoryLockProvider` | In-memory lock provider for development |
| `WopiHost.Discovery` | WOPI Discovery with caching |
| `WopiHost.Url` | WOPI URL builder |

### Highlights

- 🔌 **Pluggable storage** — implement `IWopiStorageProvider` to back WopiHost with any data source: cloud blob storage, a database, or a custom API
- ☁️ **.NET Aspire integration** — first-class cloud-native support with OpenTelemetry observability built in
- 🔐 **Enterprise security** — WOPI proof key validation, origin checking, extensible JWT-based auth/authz
- 📄 **Full WOPI compliance** — file operations, container operations, `PutRelativeFile`, `PutUserInfo`, rename, create, and OneNote for the web folder endpoints
- 🏥 **Production ready** — health checks, in-memory caching, Docker/container support
- 🎯 **Multi-targeting** — .NET 8, 9, and 10

The latest **v5.0** release was a major milestone: migration from Autofac to `Microsoft.Extensions.DependencyInjection`, full nullable reference type annotations, a dedicated `ILockService` abstraction, and Docker support — the most complete release in the project's history.

[View on GitHub](https://github.com/petrsvihlik/WopiHost) · [NuGet packages](https://www.nuget.org/packages?q=WopiHost) · [Releases](https://github.com/petrsvihlik/WopiHost/releases)
