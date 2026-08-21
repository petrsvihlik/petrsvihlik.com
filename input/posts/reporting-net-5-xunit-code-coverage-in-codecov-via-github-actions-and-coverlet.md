---
title: "Reporting .NET 5 XUnit Code Coverage in Codecov via GitHub Actions and Coverlet"
description: "You might remember my earlier post where I described how to set up code coverage reporting in AppVeyor. This is the GitHub Actions version."
slug: reporting-net-5-xunit-code-coverage-in-codecov-via-github-actions-and-coverlet
date: 2021-03-05
updated: 2026-08-21
category: technology
tags: [codecov, dot-net, github-actions, coverlet, continuous-integration, devops]
canonical_url: https://dev.to/kontent_ai/reporting-net-5-xunit-code-coverage-in-codecov-via-github-actions-and-coverlet-4h5i
comments: true
---

::: warning
Written for .NET 5 in 2021 and archived as-is: the concepts hold, but the commands have moved on (coverlet's collector with `--collect:"XPlat Code Coverage"` is the current path, and the Codecov action now requires a token). Originally published on the [Kontent.ai dev.to blog](https://dev.to/kontent_ai/reporting-net-5-xunit-code-coverage-in-codecov-via-github-actions-and-coverlet-4h5i).
:::

![](/assets/img/posts/reporting-net-5-xunit-code-coverage-in-codecov-via-github-actions-and-coverlet/01.webp)

You might remember my [earlier post](https://dev.to/petrsvihlik/running-net-core-3-xunit-code-coverage-in-appveyor-using-opencover-and-codecov-1n7p) where I described how to set up code coverage reporting for .NET Core 3 using AppVeyor + OpenCover + Codecov.

Time has moved on, .NET 5 has arrived, and the best practices have changed, so let's see how to collect code coverage for .NET projects in 2021.

## The stack
For some time, I have been intrigued by the `coverlet.collector` that you can find in any new XUnit `.csproj`. What it is? How can I use it? Can I replace OpenCover with it?

Also, I started noticing people migrating to GitHub Actions and I didn't have any experience with it and kinda felt that I'm missing the train. This observation has been confirmed recently in the [2020 State of Open Source Code Coverage](https://about.codecov.io/resource/2020-state-of-open-source-code-coverage/) report where GitHub Actions scored the #1 fastest growing CI used with Codecov (and also the #1 in absolute numbers).

[![2020 State of Open Source Code Coverage](/assets/img/posts/reporting-net-5-xunit-code-coverage-in-codecov-via-github-actions-and-coverlet/02.jpg)](https://about.codecov.io/resource/2020-state-of-open-source-code-coverage/#sos-23)

I was thinking I could modernize the stack that I use for .NET projects both at work and for my personal projects. Of the three tools that I've been using previously, I'll be keeping just Codecov (because it's awesome ❤). Let's get to it.

## Generating coverage
I like to use the `dotnet` CLI for as many tasks as possible. The good news is that `coverlet` offers deep integration with `msbuild`.
This is how you can generate code coverage for the whole solution:

```powershell
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```
The command will generate `coverage.opencover.xml` files in all your test projects (if you run `dotnet test` on the solution level).

All you need to do to make the code coverage-related `/p:` switches work is to install also the [`coverlet.msbuild`](https://www.nuget.org/packages/coverlet.msbuild/) NuGet package. So your `.csproj`s will look something like this:

```xml
<PackageReference Include="coverlet.collector" Version="3.0.3">
  <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
  <PrivateAssets>all</PrivateAssets>
</PackageReference>
<PackageReference Include="coverlet.msbuild" Version="3.0.3">
  <PrivateAssets>all</PrivateAssets>
  <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
</PackageReference>
```

Make sure to leave in the `/p:CoverletOutputFormat=opencover` switch as this is the format that's accepted by Codecov.

For more advanced configuration, I recommend reading the documentation on [Coverlet's integration with MSBuild](https://github.com/coverlet-coverage/coverlet/blob/master/Documentation/MSBuildIntegration.md). This is especially useful when your solution or projects don't stick to naming conventions and you need to do some extra filtering, etc.

## Integrating into GitHub Actions
Plugging the `dotnet test` into the CI pipeline is just a matter of creating a new workflow step. No matter if you use the `ubuntu-latest` or `windows-latest` runner, it works flawlessly.

```yml
- name: Restore dependencies
  run: dotnet restore
- name: Build
  run: dotnet build --no-restore /p:ContinuousIntegrationBuild=true
- name: Test
  run: dotnet test --no-build --verbosity normal /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

## Collecting coverage by Codecov
The last step is to upload the test coverage to Codecov. You can find an official [Codecov GitHub Action](https://github.com/marketplace/actions/codecov) on the marketplace which makes the process super easy - no need to install the Codecov CLI, etc.

Just add the following lines to your workflow:
```yml
- name: Codecov
  uses: codecov/codecov-action@v1
```
Codecov will search through the folder structure of your project and discover your coverage reports based on conventions. You don't even have to include the `CODECOV_TOKEN` for public repos.

## Summary
As you can see, this approach is much more convenient than [the one I was using before](https://dev.to/petrsvihlik/running-net-core-3-xunit-code-coverage-in-appveyor-using-opencover-and-codecov-1n7p) - downloading and installing tools, running PowerShell scripts, etc.

If you stick to standard .NET naming and folder structure conventions, setting up code coverage is actually very easy nowadays.

To see the whole workflow in action, head to some of our [SDK repos](https://github.com/Kentico/kontent-delivery-sdk-net/actions/workflows/integrate.yml). You can also explore how we do releases using GitHub Actions. All code is in the [`workflows folder`](https://github.com/Kentico/kontent-delivery-sdk-net/tree/master/.github/workflows).

In my next article, I'll cover how to [secure secrets when building pull requests from forks](https://dev.to/petrsvihlik/using-environment-protection-rules-to-secure-secrets-when-building-external-forks-with-pullrequesttarget-hci).
