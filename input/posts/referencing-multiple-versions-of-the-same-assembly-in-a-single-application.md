---
title: "Referencing Multiple Versions of the Same Assembly in a Single Application"
description: "We've all been there — our application depends on a version of assembly that's incompatible with the version that's referenced by the NuGet package we've just installed."
slug: referencing-multiple-versions-of-the-same-assembly-in-a-single-application
date: 2016-05-24
category: technology
tags: [dot-net, dependency-hell]
canonical_url: https://devnet.kentico.com/articles/referencing-multiple-versions-of-the-same-assembly-in-a-single-application
comments: true
---

::: info
A .NET Framework-era article (2016): binding redirects, AppDomains, and strong-naming work as described there, while modern .NET replaced this whole toolbox with `AssemblyLoadContext`. Still relevant if you maintain a .NET Framework or Kentico Xperience 13 site. Originally published on [Kentico DevNet](https://devnet.kentico.com/articles/referencing-multiple-versions-of-the-same-assembly-in-a-single-application).
:::

We've all been there — our application depends on a version of assembly that's incompatible with the version that's referenced by the NuGet package we've just installed. The situation gets worse when the conflict occurs between two third-party NuGet packages. What can we do about it? What does Kentico do about it?

## The Problem Called "Dependency Hell"

![Referencing multiple versions of the same assembly](https://devnet.kentico.com/getattachment/Articles/2016-05/Referencing-Multiple-Versions-of-the-Same-Assembly/cms_dep_t.png?width=480&height=175 "Referencing multiple versions of the same assembly")

Sometimes, we get to a point where different parts of our application depend on different versions of the same DLL. And by "the same", we mean assemblies with the same [simple assembly name](https://msdn.microsoft.com/en-us/library/system.reflection.assemblyname.name(v=vs.110).aspx). In many cases, when there were no breaking changes between the versions, this situation doesn't pose a problem. We can easily instruct the app to use only one of the DLLs, even without rebuilding the app. We call this [assembly binding redirection](https://msdn.microsoft.com/en-us/library/7wd6ex19(v=vs.110).aspx) and, in fact, it has been a standard approach for [NuGet](http://docs.nuget.org/release-notes/nuget-1.2#user-content-automatically-add-binding-redirects) since version 1.2 (March 2011).

The real problem arises when the DLLs are incompatible — the public types and members of the API differ, etc.

If you're lucky, the developer of the third-party component used [semantic versioning](http://semver.org/) and you can tell whether the DLLs are backwards compatible or not just from looking at their version numbers. If you're not that lucky, you'll have to either compare the APIs manually or test your application for any runtime errors related to missing types or members.

Either way, it can happen that you need to reference both of the DLLs and, because they share the name, only one can end up in your **bin** folder. Renaming one of the DLLs wouldn't help as it would prevent the assembly binder from finding it.

```
System.IO.FileLoadException : Could not load file or assembly 'Newtonsoft.Json, Version=4.5.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed' or one of its dependencies. The located assembly's manifest definition does not match the assembly reference. (Exception from HRESULT: 0x80131040)
```

We don't want to end up with an exception like this, so what now?

## Possible Solutions

There are more ways of loading multiple versions of the same assembly into a single application domain. All of them require the assemblies to be [strong-named](https://msdn.microsoft.com/en-us/library/wd40t7ad(v=vs.110).aspx) to avoid identity conflicts.

- [Install Assembly to Global Assembly Cache](https://msdn.microsoft.com/en-us/library/dkkx7f79(v=vs.110).aspx) - an easy way, only to be recommended for production environments. We don't need to touch the application at all because it'll automatically look up the assemblies in the GAC. However, it's not really handy for development environments as the installation has to be repeated on every machine.
- [Perform Custom Resolution via AppDomain.AssemblyResolve event](https://msdn.microsoft.com/en-us/library/system.appdomain.assemblyresolve(v=vs.110).aspx) - the most powerful option, especially useful when the loading logic is to be more complex or parameterized.
- **[Specify code base](https://msdn.microsoft.com/en-us/library/efs781xb(v=vs.110).aspx)** - by editing a (web) application configuration file, we can easily point the application to the appropriate DLL versions even if they're not directly in the bin folder.

  `<configuration>
  <runtime>
  <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
  <dependentAssembly>
  <assemblyIdentity name="Some.API" publicKeyToken="12ab3cd4e5f6abcd"
  culture="neutral" />
  <codeBase version="1.0.0.0" href="v1\Some.API.dll" />
  <codeBase version="2.0.0.0" href="v2\Some.API.dll" />
  <!-- INFO: The version attribute represents an assembly
  version which doesn't always have to match the
  NuGet package version.
  The codebase attribute can be anywhere on
  the local intranet or the Internet. -->
  </dependentAssembly>
  </assemblyBinding>
  </runtime>
  </configuration>`

  **From MSDN**

  If you have multiple versions of an assembly in a directory and you want to reference a particular version of that assembly, you must use the [<codeBase>](https://msdn.microsoft.com/en-us/library/efs781xb(v=vs.110).aspx) element instead of the privatePath attribute of the [<probing>](https://msdn.microsoft.com/en-us/library/823z9h8w(v=vs.110).aspx) element. If you use the [<probing>](https://msdn.microsoft.com/en-us/library/823z9h8w(v=vs.110).aspx) element, the runtime stops probing the first time it finds an assembly that matches the simple assembly name referenced, whether it is a correct match or not. If it is a correct match, that assembly is used. If it is not a correct match, probing stops and binding fails.

## CMSDependencies Demystified

In terms of using third-party DLLs, Kentico is no exception. We use a lot of well-known libraries. That's also the reason why we introduced the concept of CMSDependencies several versions ago. We try not to conflict with the DLLs you may be using in your custom libraries and spare you the work that goes with using different versions of the same assemblies, so we pulled out these libraries from the bin folder and put them aside to a separate folder in the web project. From a technical perspective, as you have probably guessed, we're using the AppDomain.AssemblyResolve event approach, which gives us enough room for maneuver.

You may be asking whether the CMSDependencies will be available if you integrate with the Kentico API from an external application. The answer is [yes](https://docs.kentico.com/display/K9/Using+the+Kentico+API+externally#UsingtheKenticoAPIexternally-IntegratingKenticoAPIlibraries) if you use our official [Kentico.Libraries NuGet package](https://www.nuget.org/packages/Kentico.Libraries/).

## Conclusion

In this article, I tried to outline the main directions you can take when facing problems with conflicting assembly versions. There's much more to the loading logic, which I haven't covered in this article. For more information, I recommend the following MSDN reading:

- [How the Runtime Locates Assemblies](https://msdn.microsoft.com/en-us/library/yx7xezcf(v=vs.110).aspx)
- [Specifying an Assembly's Location](https://msdn.microsoft.com/en-us/library/4191fzwb(v=vs.110).aspx)

I'll be very glad if you share your experience in the comments. Have you tried any of the suggested solutions? Did it work for you?
