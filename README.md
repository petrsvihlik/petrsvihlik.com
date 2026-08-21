[![.NET Core](https://github.com/petrsvihlik/petrsvihlik.com/workflows/.NET%20Core/badge.svg)](https://github.com/petrsvihlik/petrsvihlik.com/actions)

# petrsvihlik.com

Source code for [petrsvihlik.com](https://petrsvihlik.com) — a static site built with a small bespoke generator on .NET 11 (preview): Markdown ([Markdig](https://github.com/xoofx/markdig)) + YAML front matter ([YamlDotNet](https://github.com/aaubry/YamlDotNet)) rendered through Razor components (`Components/`) via ASP.NET Core's `HtmlRenderer`. No site-generator framework, two NuGet dependencies total.

## Prerequisites

- [.NET 11 SDK (preview)](https://dotnet.microsoft.com/download)

## Running locally

**Build and preview:**
```bash
dotnet run -- preview
```
Generates the site and serves it at `http://localhost:5080` (with GitHub Pages-style extensionless URLs). Use `dotnet watch run -- preview` to rebuild on file changes.

**One-off build** (output goes to `output/`):
```bash
dotnet run
```

## How it works

- `Generation/SiteBuilder.cs` loads content from `input/`, renders the components, and writes `output/` — pages, paginated archives (home, per-tag, per-category), RSS/Atom feeds, and `sitemap.xml`.
- Templates are plain Razor components in `Components/`; site metadata (title, author, contacts) lives in `SiteBuilder.CreateSiteMetadata()`.
- Settings: `TagManagerId` from `appsettings.json`; `Host` and `LinkRoot` (used by CI) come from environment variables and control absolute-link generation for feeds and the sitemap.

## Adding content

### New blog post

Create a file in `input/posts/` with the naming pattern `my-post-slug.md`:

```yaml
---
title: My Post Title
date: 2026-01-15
slug: my-post-slug
description: A short summary shown on the article list.
category: category-slug
tags:
  - tag-one
  - tag-two
---

Post content in Markdown goes here.
```

- `slug` is optional — defaults to the filename without extension
- `canonical_url` can be added for posts originally published elsewhere (adds a `<link rel="canonical">`)
- Categories and tags are derived from slugs automatically (hyphens → spaces, title-cased)

### New page

Create a Markdown file in `input/pages/`:

```yaml
---
title: Page Title
---

Page content here.
```

## Deployment

Pushing to `master` triggers the [`.NET Core` GitHub Actions workflow](.github/workflows/dotnet-core.yml), which builds the site and deploys it to GitHub Pages (`gh-pages` branch). Lighthouse CI runs automatically after a successful deploy.
