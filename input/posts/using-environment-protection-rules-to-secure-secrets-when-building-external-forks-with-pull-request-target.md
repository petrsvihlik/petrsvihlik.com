---
title: "Using Environment Protection Rules to Secure Secrets When Building External Forks with pull_request_target"
description: "Building pull requests from forked repositories with GitHub Actions can be a bit tricky when it comes to secrets. A 2021 pattern — with a 2026 postscript on how it aged."
slug: using-environment-protection-rules-to-secure-secrets-when-building-external-forks-with-pull-request-target
date: 2021-03-05
updated: 2026-08-21
category: technology
tags: [github-actions, devops, security]
comments: true
---

::: warning
This article is from March 2021 and describes `pull_request_target` as it worked back then. GitHub changed the event's semantics in December 2025, and `actions/checkout` now refuses the exact checkout shown below by default. The pattern's core idea — a human approval gate in front of secrets — survived and is now semi-official practice, but don't copy the YAML verbatim. The [postscript](#postscript-august-2026-what-four-years-did-to-this-pattern) tells the whole story.
:::

![](/assets/img/posts/using-environment-protection-rules-to-secure-secrets-when-building-external-forks-with-pull-request-target/01.svg)

Building pull requests from forked repositories with GitHub Actions can be a bit tricky when it comes to secrets. As per the [documentation](https://docs.github.com/en/actions/reference/events-that-trigger-workflows#pull_request), with the exception of `GITHUB_TOKEN`, secrets are not passed to the runner when a workflow is triggered from a forked repository. This is to prevent the automatic execution of untrusted code that may be contained within the forked repo.
In other words, we can't use the `pull_request` trigger if there are secrets that need to be involved in the workflow.

Fortunately, `pull_request_target` [comes to rescue](https://github.blog/2020-08-03-github-actions-improvements-for-fork-and-pull-request-workflows/#improvements-for-public-repository-forks).

> ...the `pull_request_target` event behaves in an almost identical way to the pull_request event with the same set of filters and payload. However, instead of running against the workflow and code from the merge commit, the event runs against the workflow and code from the base of the pull request. This means the workflow is running from a trusted source...

Ok, so now we have access to secrets but we're building the wrong code.

Apparently, there are some people who try to overcome this problem with the following code:

```yml
#INSECURE
steps:
- uses: actions/checkout@v2
  with:
    ref: ${{ github.event.pull_request.head.sha }} # Check out the code of the PR
```
This is highly discouraged and rightfully so, as it's insecure if no other security measures are taken.

In GitHub's own article [Preventing pwn requests](https://securitylab.github.com/research/github-actions-preventing-pwn-requests), the author - [Jaroslav Lobačevski](https://blog.devsecurity.eu/) - suggests using the `pull_request_target` in combination with a condition checking whether the PR is labeled `safe to test`. Like this:

```yml
    jobs:
      build:
        name: Build and test
        runs-on: ubuntu-latest
        if: contains(github.event.pull_request.labels.*.name, 'safe to test')
```

This is a perfectly valid approach but I think I may have found a better and more convenient way of preventing unauthorized code execution during the build of forks.

## Environment protection rules
Just a couple of months ago, GitHub introduced the [Environment protection rules](https://github.blog/changelog/2020-12-15-github-actions-environments-environment-protection-rules-and-environment-secrets-beta/). The main intent of this feature is to protect environments during deployments by applying rules that will pause the execution of a workflow until given conditions are met - e.g. a human approval is given, the certain time elapsed, etc. But it can serve any general purpose. In our case, we'll use it to protect our repository secrets and to prevent the execution of untrusted code.

### Protecting the build
Let's start with adding a dummy environment called "Integrate Pull Request" that will require human approval.

![Integrate Pull Request Environment](/assets/img/posts/using-environment-protection-rules-to-secure-secrets-when-building-external-forks-with-pull-request-target/02.png)

Our main build procedure will be associated with this environment and preceded by a dummy workflow step `approve` that will kick off the workflow and inform the author of the pull request that a review is necessary before proceeding any further.

```yml
on:
  pull_request_target:
    branches: [ master ]

jobs:
  approve: # First step
    runs-on: ubuntu-latest

    steps:
    - name: Approve
      run: echo For security reasons, all pull requests need to be approved first before running any automated CI.

  build: # Second step
    runs-on: ubuntu-latest

    needs: [approve] # Require the first step to finish
    environment:
      name: Integrate Pull Request # Our dummy environment
    steps:
    - ...
```

This way the workflow won't proceed until someone reviews the submitted code and therefore, we can safely check out the `${{ github.event.pull_request.head.sha }}` in the next step and build it. So the build is executed using a trusted workflow from the base of the PR and the actual code of the PR.

### How it works in practice

1. Someone submits a pull request and a workflow is triggered and immediately paused
![Build is waiting for human approval](/assets/img/posts/using-environment-protection-rules-to-secure-secrets-when-building-external-forks-with-pull-request-target/03.png)

2. The reviewer or a group of reviewers receive an e-mail notification
![Email notification about a pending review](/assets/img/posts/using-environment-protection-rules-to-secure-secrets-when-building-external-forks-with-pull-request-target/04.png)

3. The reviewer clicks the link, navigates to the repo, verifies that the submitted PR doesn't contain any unwanted code, and finally gives an approval
![Approve the workflow step](/assets/img/posts/using-environment-protection-rules-to-secure-secrets-when-building-external-forks-with-pull-request-target/05.png)

4. The build proceeds
![Build is approved](/assets/img/posts/using-environment-protection-rules-to-secure-secrets-when-building-external-forks-with-pull-request-target/06.png)

5. All approvals are audited
![Approval audit log](/assets/img/posts/using-environment-protection-rules-to-secure-secrets-when-building-external-forks-with-pull-request-target/07.png)

## A few words on Codecov
While implementing this workflow, I ran into an issue where the Codecov action, similarly to the [GitHub Checkout Action](https://github.com/marketplace/actions/checkout), is by default [pointed to the PR's Base](https://github.com/codecov/codecov-action/issues/155) and needs to be overridden. This can be achieved by:

```yml
- name: Codecov
  uses: codecov/codecov-action@v1
  with:
    token: ${{ secrets.CODECOV_TOKEN }}
    override_pr: ${{ github.event.number }}
    override_commit: ${{ github.event.pull_request.head.sha }}
```

To get rid of the following warning message:

> Issue detecting commit SHA. Please run actions/checkout with fetch-depth > 1 or set to 0

make sure to also set `fetch-depth` of the Checkout action to 2.

```yml
steps:
- uses: actions/checkout@v2
  with:
    fetch-depth: 2
```

*Note: I found an alternative approach using a conditional [workflow step](https://github.com/Cookie-AutoDelete/Cookie-AutoDelete/actions/runs/274505391/workflow) and a [shell script](https://github.com/Cookie-AutoDelete/Cookie-AutoDelete/blob/3.X.X-Branch/.github/codecov_alt.sh). But overriding the commit SHA is far easier.*

## Summary
The advantage of this approach is that you can assign a group of reviewers who'll receive an email notification about the pending workflow and can review and approve it in a single click.
The process is, in my opinion, more transparent thanks to all events being logged and semantically more correct than using labels.

If you want to explore the whole workflow, feel free to check out my project [WopiHost](https://github.com/petrsvihlik/WopiHost/blob/master/.github/workflows/pull_request.yml).

To learn more about the specifics of `pull_request_target` head to the [documentation](https://docs.github.com/en/actions/reference/events-that-trigger-workflows#pull_request_target).

---

## Postscript (August 2026): what four years did to this pattern

I wrote everything above in March 2021, when environments were a few months old and `pull_request_target` was the shiny new answer to fork builds. Publishing security advice comes with an implicit contract: you should come back later and tell people how it aged. Here's my report, and it's a story in four acts.

### Act one: the pattern spreads

The human-approval-gate idea took off. Variants of "gate the secrets behind an environment with required reviewers" appeared in community guidance and eventually in [GitHub's own documentation on securely using `pull_request_target`](https://docs.github.com/en/actions/reference/security/secure-use). For a while it was fair to call it semi-official best practice, and I'll admit the 2021 me would have been pretty pleased with that.

### Act two: the bypass (February 2025)

Then researchers at QuantCo found a hole — not in my workflow specifically, but in the ground it stood on. In [Pull Requests Go Both Ways](https://tech.quantco.com/blog/github-actions-environments/), Yannik Tausch and Oliver Borchert showed that **any collaborator with push access to any branch could bypass an environment's deployment-branch restriction** and reach its protected secrets and OIDC tokens by leveraging `pull_request_target` — and, remarkably, a repository was exposed *even if none of its workflows used the trigger at all*, because the attacker could introduce a workflow that did. They reported it through GitHub's bug bounty program on HackerOne; GitHub fixed it in December 2025.

Read that again: the mechanism I recommended for protecting secrets was, for a window of time, itself a way around a related protection. The approval gate still required a human click, but the environment model underneath had a seam nobody had noticed for years.

### Act three: GitHub rewrites the event (December 2025)

GitHub's fix went deeper than patching the bypass. As of December 8, 2025, [the semantics of `pull_request_target` changed outright](https://github.blog/changelog/2025-11-07-actions-pull_request_target-and-environment-branch-protections-changes/):

- The workflow file and checkout now **always come from the repository's default branch**, no matter which branch the PR targets. `GITHUB_REF` resolves to the default branch and `GITHUB_SHA` to its latest commit. (Previously, any base branch could supply the workflow — which meant outdated, already-"fixed" vulnerable workflows on stale branches could still be executed. That class of bug is now dead.)
- Environment branch-protection rules are evaluated against the *executing* ref — for `pull_request_target` that's the default branch, for the `pull_request` family it's `refs/pull/N/merge` — closing the seam QuantCo found.

So the block quote near the top of this article — "the event runs against the workflow and code from the base of the pull request" — is no longer true. It describes an event that doesn't exist anymore.

### Act four: my checkout line gets refused (June 2026)

The final twist is my favorite. In June 2026, [`actions/checkout` v7 started refusing to fetch fork PR code in `pull_request_target` workflows by default](https://github.blog/changelog/2026-06-18-safer-pull_request_target-defaults-for-github-actions-checkout/) — and among the "insecure inputs" it now rejects is, verbatim, `ref: ${{ github.event.pull_request.head.sha }}`. The exact line this article is built around. The enforcement was even backported to older checkout versions.

There's an opt-out for workflows that check out fork code deliberately, with elevated trust — which is precisely what the approval-gate pattern does. It's called `allow-unsafe-pr-checkout`, and GitHub says the name is intentionally ugly so it stands out in code review. I find that genuinely elegant: the platform now forces the 2021 pattern to *declare itself* instead of blending into innocent-looking YAML.

### What I'd actually do today

The idea survived; the YAML didn't. If I were setting this up in 2026:

1. Keep the environment with **required reviewers** in front of any job that touches secrets — that part aged well, and it's why the opt-out exists at all.
2. Accept the new semantics: your workflow always runs from the default branch. That's a feature — fix a vulnerable workflow once, on the default branch, and stale branches can't resurrect it.
3. Check out the fork's code only in the gated job, with `allow-unsafe-pr-checkout` set — visibly, greppably, deliberately.
4. Scope the secrets to the bare minimum the fork build needs, and prefer OIDC over long-lived credentials.
5. Re-read [GitHub's hardening guidance](https://docs.github.com/en/actions/reference/security/secure-use) before trusting anything — including this post.

### The meta-lesson

Security patterns have a half-life, and it's shorter than the half-life of blog posts about them. This article ranked in searches for years after the ground truth underneath it had shifted — which is exactly how "semi-canonical practice" quietly becomes a liability. The 2021 idea was right: put a human between untrusted code and your secrets. Everything else — the event semantics, the environment model, even the checkout line — turned out to be rented ground.

I'd rather amend my old advice in public than have it silently rot in a search index. Consider this the amendment.
