---
title: "Three Ways to Manage Data in Kentico Using PowerShell"
description: "PowerShell is the leading task automation platform for Windows. Developers and sysadmins alike use it to automate tasks and manage systems."
slug: three-ways-to-manage-data-in-kentico-using-powershell
date: 2016-08-09
category: technology
tags: [powershell, rest, reflection, compilation]
canonical_url: https://devnet.kentico.com/articles/three-ways-to-manage-data-in-kentico-using-powershell
draft: true # triage: drop — excluded from the site (issue #64)
---

PowerShell is the leading task automation platform for Windows. Developers and system administrators from all around the world use it for the most complex deployment and installation scenarios as well as for the simplest scripting. The facts that it's built on top of the .NET Framework opens up certain possibilities for integration with Kentico. Today, we'll have a look at what we can achieve with PowerShell in combination with the Kentico API and the Kentico REST API.

During the past few years, PowerShell has become very popular, and I dare to say it's the number one choice for automating stuff in the Microsoft ecosystem nowadays. The whole Microsoft stack supports it: we can install Windows features, configure network adapters, download NuGet packages, manage IIS, or deploy whole infrastructures to Azure using PowerShell. There are tons and tons of PowerShell modules and commandlets out there that make the scripting easier. I'd be very much surprised if there weren't any PowerShell scripts used somewhere in your organization.

In the web development universe, we use PowerShell mainly to perform tasks during automated deployments, and often we need to adjust the initial configuration or add some data from other systems. Some actions will require calling third-party APIs, such as Kentico's, and if we don't want to implement an EXE application (as I've described in [another article](http://devnet.kentico.com/articles/take-advantage-of-kentico%E2%80%99s-nuget-feed-and-build-your-own-apps)) for every little task, it may be a good idea to learn how to do it from PowerShell.

Let's consider the following task: we need to retrieve a user object from the Kentico database, change its nickname, and save the changes back to the DB. There are at least three ways we can achieve that via PowerShell.

## 1) PowerShell Calling Kentico REST API

If you need to perform simple CRUD operations upon Kentico objects, such as for the described task of reading and updating a user object, you'll probably want to utilize the [Kentico REST API](https://docs.kentico.com/display/K9/Kentico+REST+service). To do that, call the Invoke-RestMethod cmdlet and pass the appropriate URI, HTTP method, and HTTP headers. Basically, you only need to provide credentials using the Authorization header, but you can, for example, adjust the output format by setting the Content-Type header.

<script src="https://gist.github.com/petrsvihlik/204515a87a14217bd3c2a2b991814d40.js"></script>

<noscript><a href="https://gist.github.com/petrsvihlik/204515a87a14217bd3c2a2b991814d40">View the gist on GitHub</a></noscript>

## 2) PowerShell Calling Reflected Kentico API

Should you need to perform some more advanced tasks, such as manipulating workflows and marketing automation or doing some Smart Search index maintenance, using the full-blown Kentico API would be the preferred choice. The following script reads the web.config mainly to retrieve the connection string, loads all necessary assemblies and their dependencies, and directly calls Kentico methods.

<script src="https://gist.github.com/petrsvihlik/4727fa0944caf0988e60cb1cddd85938.js"></script>

<noscript><a href="https://gist.github.com/petrsvihlik/4727fa0944caf0988e60cb1cddd85938">View the gist on GitHub</a></noscript>

## 3) PowerShell Calling Kentico API through an In-Time Compiled C# Class

The last option is similar to the previous one with one difference: the actual Kentico-related code is written in C# and compiled by PowerShell during the script's runtime. This option may be more convenient for those who are used to writing C#.

<script src="https://gist.github.com/petrsvihlik/eae68ea160c9db788c39e5aa9eb774a1.js"></script>

<noscript><a href="https://gist.github.com/petrsvihlik/eae68ea160c9db788c39e5aa9eb774a1">View the gist on GitHub</a></noscript>

## Conclusion

I hope this article gave you a head start in writing your own modules. For Kentico developers, I see the potential of PowerShell, especially in CI scenarios and in intranets, but I'm sure you can think of other applications. Let us know down in the comments, and don't hesitate to share your creations with the community!
