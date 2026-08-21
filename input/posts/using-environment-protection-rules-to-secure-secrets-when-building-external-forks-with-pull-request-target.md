---
title: "Using Environment Protection Rules to Secure Secrets When Building External Forks with pull_request_target"
description: "Building pull requests from forked repositories with GitHub Actions can be a bit tricky when it comes to secrets."
slug: using-environment-protection-rules-to-secure-secrets-when-building-external-forks-with-pull-request-target
date: 2021-03-05
category: technology
tags: [github-actions, devops]
canonical_url: https://dev.to/petrsvihlik/using-environment-protection-rules-to-secure-secrets-when-building-external-forks-with-pullrequesttarget-hci
draft: true # triage: rework — excluded until rewritten (issue #64)
---

![](/assets/img/posts/using-environment-protection-rules-to-secure-secrets-when-building-external-forks-with-pull-request-target/01.webp)

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
