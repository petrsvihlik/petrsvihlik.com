---
title: "Lessons Learned: Migrating from AppVeyor to GitHub Actions"
description: "The third article about GitHub Actions in a row? One must be thinking I am up to something."
slug: lessons-learned-migrating-from-appveyor-to-github-actions
date: 2021-03-05
category: technology
tags: [github-actions, appveyor, linux, devops]
comments: true
---

![](/assets/img/posts/lessons-learned-migrating-from-appveyor-to-github-actions/01.svg)

The third article about GitHub Actions in a row? One must be thinking I am up to something. And they'd be right. Over the past week, I migrated more than 10 repositories from AppVeyor to GitHub Actions.

"Why?" you are asking? My team did extensive research on which CI provider to use next as we started hitting some limits with Travis and we weren't happy with how scattered our build procedures were over multiple CIs. We've been using a different CI provider for nearly every stack. Considering the number of SDKs that we provide (JavaScript, .NET, PHP, Java, Swift, Android, Ruby), it should come as no surprise that we wanted to reduce the maintenance required. And we decided that GitHub Actions are simply the best to fit our needs, closely followed by CircleCI.

Over the course of the migration, I ran into multiple issues. Most of them are pretty easy to solve (at least if you pay attention to the documentation) so the intention of this article is not to give you solutions to simple problems but rather to give you an overview of what types of problems you might face along the way and how much time (based on the knowledge of your codebase) is required to convert a repo to GH Actions.

## Linux vs. Windows
By default, all actions run on Linux by default. And if possible, it's a good idea to keep it that way as it's the least resource-demanding option (which you'll also learn if you check out [the minute multiplier](https://docs.github.com/en/github/setting-up-and-managing-billing-and-payments-on-github/about-billing-for-github-actions) on the pricing page).
Another reason to stick to Linux is that, if you use Windows for local development, you have verification of cross-platform compatibility. Obviously, you can also use [the strategy matrix](https://docs.github.com/en/actions/reference/workflow-syntax-for-github-actions#jobsjob_idstrategymatrix) to run multiple platforms at once.

*PRO TIP: For more complex build scripts, it's a good idea to take an iterative approach and first migrate to `windows-latest`, make sure everything works, and then switch to `ubuntu-latest`.*

### Case sensitivity
As you know, on Linux, the file system is case-sensitive. So beware of files like `Directory.Build.props`. Yeah, `Directory.build.props` is something completely different.

### Directory separator
Some of your code or unit tests may depend on the file system. In that case, make sure you assemble all paths using `Path.Combine()` or `Path.DirectorySeparatorChar`.

### Quotes
Some shell commands require different syntax than on Windows. A nice example is `dotnet nuget push` which requires the first parameter to be [enclosed in apostrophes](https://github.com/dotnet/docs/issues/7146#issuecomment-604057410) on Linux to properly parse the wildcard when publishing multiple NuGet packages.

### Running PowerShell
Yes, you can run PowerShell Core on Linux, which is absolutely brilliant if you're migrating from Windows and want to keep just one set of scripts!
The only thing you need to do is to [specify `pwsh`](https://docs.github.com/en/actions/reference/encrypted-secrets#example-using-powershell) as the type of your shell.
```yml
steps:
  - shell: pwsh
    env:
      SUPER_SECRET: ${{ secrets.SuperSecret }}
    run: |
      example-command "$env:SUPER_SECRET"
```
Just notice the different syntax for accessing environment variables - `$SUPER_SECRET` in Bash vs `$env:SUPER_SECRET` in PowerShell and don't mistake the `pwsh` moniker with `powershell` which is the non-Core version of PowerShell supported only on Windows.
See the shell platform compatibility matrix [here](https://docs.github.com/en/actions/reference/workflow-syntax-for-github-actions#using-a-specific-shell).

### WSL is your best friend
If you don't have the Windows Subsystem for Linux installed, or worse, you don't know what it is, I encourage you to head to the [docs](https://docs.microsoft.com/en-us/windows/wsl/install-win10) and be amazed. Being able to execute any Linux command on Windows is, in my opinion, one of the most brilliant engineering achievements of Microsoft in the past years.
Personally, I'm a fan of [openSUSE](https://www.microsoft.com/en-us/p/opensuse-leap-152/9mzd0n9z4m4h) but you can choose from many distros.

### HttpClient
The HTTP request and response objects look slightly different when you serialize them on Windows and Linux. If, for instance, your unit tests heavily depend on serialized responses, you might want to address this issue or choose to run the workflow on `windows-latest`.

## `.csproj` patching
AppVeyor offers features such as [`.csproj` files patching](https://www.appveyor.com/docs/build-configuration/#net-core-csproj-files-patching) and [`AssemblyInfo` patching](https://www.appveyor.com/docs/build-configuration/#assemblyinfo-patching).
These features are not available in GitHub Actions and the easiest way to overcome this is to use parameters that the .NET CLI offers [`dotnet build -p:Version=1.2.3`](https://github.com/Kentico/kontent-delivery-sdk-net/blob/ec9ad537199f88a2f88a7c6545a7aeec8d7cbe39/.github/workflows/release.yml#L24) and [`dotnet pack -p:PackageVersion=1.2.3`](https://github.com/Kentico/kontent-delivery-sdk-net/blob/ec9ad537199f88a2f88a7c6545a7aeec8d7cbe39/.github/workflows/release.yml#L30) or [`dotnet pack -p:NuspecProperties="Version=1.2.3"`](https://github.com/Kentico/kontent-boilerplate-net/blob/8afced3a3d537e39372e7e84f49921d3057f5a33/.github/workflows/release.yml#L30) if you use a `.nuspec` file.

This is how it can look like in a real workflow:
```yml
- name: Extract version from tag
  id: get_version
  uses: battila7/get-version-action@v2
- name: Build
  run: dotnet build /p:Version="${{ steps.get_version.outputs.version-without-v }}"
- name: Pack
  run: dotnet pack /p:PackageVersion="${{ steps.get_version.outputs.version-without-v }}"
```

## Symbols
If you want to enable [Source Link](https://github.com/dotnet/sourcelink) for the consumers of your NuGets, make sure you turn on deterministic builds using `dotnet build /p:ContinuousIntegrationBuild=true`.

## Using custom actions
- GitHub Actions use [Semantic Versioning](https://semver.org/) so you don't have to include the minor and patch version (`codecov/codecov-action@v1.2.3`) when referencing them. It's actually a good idea to use just the major version (`codecov/codecov-action@v1`) because you get any potential fixes free of charge.
- You can use actions that are not published on the Marketplace. For instance, you can create your own actions and refer the them by `gh_username/repo_with_action@version`.

## Creating custom actions
Two things I learned about creating custom GitHub Actions that you might find asking yourself too:
- It's impossible to nest GitHub Actions (create GitHub Actions containing other GitHub Actions). However, according to [this](https://github.com/actions/runner/issues/646#issuecomment-788373907), it seems the topic of templating is about to be addressed this year.
- Hosting multiple actions in a single repo IS [possible](https://stackoverflow.com/questions/59259783/how-to-combine-two-action-yml-files-in-one-repo). You then refer to such actions as  `gh_username/repo_with_actions/action1@version`. I'm not sure if it's possible to [publish](https://docs.github.com/en/actions/creating-actions/publishing-actions-in-github-marketplace) them to the marketplace in this setup though.

## Summary
And that's it. There we no other issues during the migration. That was a great surprise for me. Learning GitHub Actions and migrating most of the repos took me a single weekend. Everything was documented properly, I found many examples, and didn't run to any bugs!

Good job GitHub Actions!
