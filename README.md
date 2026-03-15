[![.NET Core](https://github.com/petrsvihlik/petrsvihlik.com/workflows/.NET%20Core/badge.svg)](https://github.com/petrsvihlik/petrsvihlik.com/actions)

# petrsvihlik.com

Source code for [petrsvihlik.com](https://petrsvihlik.com) — a static site built with [Statiq.Web](https://statiq.dev/web/) on .NET 10. Content is stored as Markdown files with YAML front matter.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (for Pagefind search indexing)

## Running locally

**Preview with live reload:**
```bash
dotnet run -- preview
```
Opens at `http://localhost:5080`. The site rebuilds automatically on file changes.

**One-off build** (output goes to `output/`):
```bash
dotnet run
```

> Note: Pagefind search index is built as part of the normal build but not during `preview`. Search won't work locally unless you run `npx pagefind --site output` manually after a full build.

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
