[![.NET Core](https://github.com/petrsvihlik/petrsvihlik.com/workflows/.NET%20Core/badge.svg)](https://github.com/petrsvihlik/petrsvihlik.com/actions)

# petrsvihlik.com

Source code for [petrsvihlik.com](https://petrsvihlik.com) — a static site built with a small bespoke generator on .NET 11 (preview): Markdown ([Markdig](https://github.com/xoofx/markdig)) + YAML front matter ([YamlDotNet](https://github.com/aaubry/YamlDotNet)) rendered through Razor components (`Components/`) via ASP.NET Core's `HtmlRenderer`. No site-generator framework, two NuGet dependencies total.

## Prerequisites

- [.NET 11 SDK (preview)](https://dotnet.microsoft.com/download)

## Running locally

**Build and preview:**
```bash
dotnet run -- preview          # published content only, as production renders it
dotnet run -- preview drafts   # also renders draft posts at their real URLs
```
Generates the site and serves it at `http://localhost:5080` (with GitHub Pages-style extensionless URLs). Use `dotnet watch run -- preview` to rebuild on file changes.

**One-off build** (output goes to `output/`):
```bash
dotnet run
```

**Start a new post:**
```bash
dotnet run -- new "My Post Title"
```
Scaffolds `input/posts/my-post-title.md` with front matter prefilled (slug derived from the title, diacritics folded; today's date) — born as `draft: true`, so it is previewable immediately and invisible in production until the draft line is removed. Refuses to overwrite an existing file.

## How it works

- `Generation/SiteBuilder.cs` loads content from `input/`, renders the components, and writes `output/` — pages, paginated archives (home, per-tag, per-category), RSS/Atom feeds, and `sitemap.xml`.
- Templates are plain Razor components in `Components/`; site metadata (title, author, contacts) lives in `SiteBuilder.CreateSiteMetadata()`.
- Email capture (`Components/Newsletter.razor`) renders on every post page, at the bottom of the homepage archive, and on pages that opt in — a plain HTML form handing the address to the provider's hosted subscribe flow. Provider specifics are five constants at the top of the component (currently Substack; Buttondown/Kit equivalents documented inline).
- Document lightbox: any link with `data-lightbox="doc"` opens its target in an iframe overlay (toolbar: download via `data-download`, open full page, close) instead of navigating — the hero's `[cv]` link uses it. The CV itself is a self-contained page + PDF under `input/assets/cv/`; a header contact becomes such a link via the `Lightbox`/`Download` properties on `Contact`. To refresh the PDF after editing the CV HTML, print it headless (Chromium `--print-to-pdf` honors the page's `@page` A4 rules).
- Settings: `TagManagerId` from `appsettings.json`; `Host` and `LinkRoot` (used by CI) come from environment variables and control absolute-link generation for feeds and the sitemap; `Drafts=true` (or the `drafts` CLI arg) includes draft posts in the build.

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
- `canonical_url` can be added for posts originally published elsewhere (adds a `<link rel="canonical">`); posts without it are canonicalized to their own URL
- Categories and tags are derived from slugs automatically (hyphens → spaces, title-cased)
- `draft: true` excludes the post from production builds entirely — no page, and it never appears in archives, feeds, or the sitemap. Include draft pages locally with `dotnet run -- preview drafts` (or the `Drafts=true` env var); they render at their real `/posts/<slug>` URL, `noindex`ed, still outside archives/feeds/sitemap
- `comments: true` renders a giscus comment thread (backed by a GitHub Discussion titled `posts/<slug>`; the [`Blog discussion` workflow](.github/workflows/blog-discussion.yml) can pre-create one)
- `updated: 2026-08-21` marks a substantive revision: `date` always stays the original publication date (the archive is chronological), while `updated` shows next to it on the post, feeds `article:modified_time`/`dateModified`, the sitemap's lastmod, and the Atom entry's `updated`

### Rich content

Markdown is rendered with Markdig's `advanced` pipeline; everything it supports is styled and demonstrated on the unlisted test page [`/pages/markup-demo`](input/pages/markup-demo.md) — tables, task lists, footnotes, definition lists, figures with captions, `::: tip|info|warning|danger` callouts, ==highlights==, and more. Fenced code blocks get build-flagged syntax highlighting (vendored [highlight.js](https://highlightjs.org), loaded only on pages that contain code) plus a copy button; images lazy-load and open in the lightbox; GitHub gists embed via their plain `<script>` snippet. `h2`/`h3` headings get hover anchor links.

Add `unlisted: true` to a page's front matter to render it while keeping it out of the sitemap and marked `noindex` (used by the markup demo).

### Illustrations (SVG design system)

Post illustrations are hand-authored SVGs in a shared "terminal panel" style: a dark window with traffic-light dots and a `$ command` title, drawn strictly from the site's design tokens (JetBrains Mono, the dark-theme palette, 6px-radius boxes, code-comment annotations). Dark chrome is deliberate — like a real terminal screenshot, the panels read native in dark mode and intentional in light mode, with no theme plumbing needed inside `<img>`-loaded SVGs.

To create one, copy [`input/assets/img/posts/_template.svg`](input/assets/img/posts/_template.svg) — it documents the palette, chrome, and conventions inline — save it as `input/assets/img/posts/<slug>/NN.svg`, and reference it like any image (add a caption with the `^^^` figure syntax). Keep photos, screenshots, and memes as raster; use the panels for concepts, diagrams, and headers.

### New page

Create a Markdown file in `input/pages/`:

```yaml
---
title: Page Title
---

Page content here.
```

Pages support the same `comments: true` opt-in as posts, plus `newsletter: true` to render the email-capture form under the content (posts get it automatically).

## Deployment

Pushing to `master` triggers the [`.NET Core` GitHub Actions workflow](.github/workflows/dotnet-core.yml), which builds the site and deploys it to GitHub Pages (`gh-pages` branch). Lighthouse CI runs automatically after a successful deploy. Pull requests against `master` get a [Surge preview deploy](.github/workflows/preview.yml), linked from a PR comment and torn down on close.
