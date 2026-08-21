---
title: "Running .NET Core 3 XUnit Code Coverage in AppVeyor Using OpenCover and Codecov"
description: "I love AppVeyor. It's my go-to CI/CD service for .NET projects. I use it for both open-source and private projects."
slug: running-net-core-3-xunit-code-coverage-in-appveyor-using-opencover-and-codecov
date: 2019-12-01
category: technology
tags: [opencover, appveyor, codecov, dot-net]
canonical_url: https://dev.to/petrsvihlik/running-net-core-3-xunit-code-coverage-in-appveyor-using-opencover-and-codecov-1n7p
draft: true # triage: drop — excluded from the site (issue #64)
---

![](/assets/img/posts/running-net-core-3-xunit-code-coverage-in-appveyor-using-opencover-and-codecov/01.webp)

I love AppVeyor. It's my go-to CI/CD service for .NET projects. I use it for both [personal](https://github.com/petrsvihlik/WopiHost/) and [work](https://github.com/Kentico/kontent-delivery-sdk-net) open-source projects. I also love automation of all kinds so, among other things, I want my test coverage reports to be automatically generated with every build.

Recently, I started upgrading my project portfolio to .NET Core 3.0. With that, I also wanted to level-up my unit tests which are typically XUnit. I didn't want to introduce too much change to my code so, after some research, I decided to stick with the following setup: AppVeyor (which I've already been using) + Codecov (which we use for another project at work) + OpenCover.

### Custom PowerShell coverage script 📜
The goal was to create a minimalistic, reusable piece of code (independent of environment settings, tool versions, etc.) that I could use across multiple projects. Here it goes:

#### coverage.ps1
{% gist https://gist.github.com/petrsvihlik/857671b3d937d010ccc67676733625d7 file=coverage.ps1 %}

The script can run locally and in AppVeyor with slightly different configurations. The most tricky part is:

```
$register = if ($ENV:APPVEYOR -eq $true ) { '-register' } else { '-register:user' }
```

When I finally made the script run on my local machine, it started failing in AppVeyor. It took me a few hours of digging and debugging to figure out that I can't use `-register:user` in AppVeyor. Fortunately, debugging is quite easy in AppVeyor as you can [RDP to the build worker](https://www.appveyor.com/docs/how-to/rdp-to-build-worker/).

I highly recommend reading [OpenCover's documentation](https://github.com/opencover/opencover/wiki/Usage). It'll help you understand the syntax of [`targetArgs`](https://github.com/opencover/opencover/wiki/Usage#notes-on-spaces-in-arguments), [`filter` and `register`](https://github.com/opencover/opencover/wiki/Usage#optional-arguments) parameters.

### Hooking the script into the CI pipeline 🔗

AppVeyor supports [Chocolatey](https://chocolatey.org/) so I'm using `cinst` to install OpenCover and Codecov CLIs. Then I'm turning off the default test script and replacing it with my own which runs OpenCover to generate the coverage files, and finally, I'm calling Codecov CLI to upload the results.

Cool thing is that if you run `codecov -f coverage.xml` from AppVeyor you don't need an API key. It just works automagically ✨.

#### appveyor.yml
{% gist https://gist.github.com/petrsvihlik/857671b3d937d010ccc67676733625d7 file=appveyor.yml %}

### The result
I've got this nice sunburst chart indicating which files need attention:

![Codecov sunburst](/assets/img/posts/running-net-core-3-xunit-code-coverage-in-appveyor-using-opencover-and-codecov/02.png)

I've also got the Codecov badge with % coverage:
{% github Kentico/kontent-delivery-sdk-net %}

And last but not least, a nice ASCII art in the AppVeyor log here:
[![Alt Text](/assets/img/posts/running-net-core-3-xunit-code-coverage-in-appveyor-using-opencover-and-codecov/03.png)](https://ci.appveyor.com/project/kentico/deliver-net-sdk/branch/master#L310)

So far, I have successfully tested this approach in two repos so I hope it'll work for you too. Any improvements are welcome!
