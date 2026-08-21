---
title: "The Rise of JAMstack"
description: "JAMstack has proven increasingly popular among developers over the past few years. But what exactly is JAMstack, and why should you care?"
slug: the-rise-of-jamstack
date: 2020-03-12
category: technology
tags: [jamstack]
canonical_url: https://kontent.ai/blog/the-rise-of-jamstack
comments: true
---

::: info
A 2020 snapshot of a movement at its peak: the term has since been retired by its own coiners, and the patterns dissolved into the mainstream (SSR, ISR, edge rendering). Archived as a period piece — including the capitalization. Originally published on the [Kontent.ai blog](https://kontent.ai/blog/the-rise-of-jamstack).
:::

## What is JAMstack, and how does it work?
The JAM in [JAMstack](https://kontent.ai/resources/state-of-jamstack-report/) stands for **J**avaScript, **A**PI, and **M**arkup. It is essentially a new way to construct websites and applications, with the key difference being that it runs almost completely in the front end, allowing the sites and apps to become served without classic web servers.

In a traditional [CMS workflow](https://kontent.ai/blog/content-management-workflow/), the front and back ends are coupled, which means generating data to present to the user may require going through a number of processes along the way—such as web request processing, interacting with databases and other services, rendering the output, and communicating between server and browser.

JAMstack, however, draws a strong line between the content delivery and all that needs to happen before in order to render the content. Static site generators (SSGs) make it possible to pre-generate the **M**arkup, and the static files are then served straight from the content delivery network (CDN) to the user. Any dynamic enhancements take place on the client-side thanks to **J**avaScript and services available via **A**PI.

## Why has JAMstack experienced such a rise?
As with all great ideas, the genius lies in its simplicity. JAMstack and static site generators in particular owe their success to modern JavaScript frameworks. While not all SSGs are based on JavaScript, and some of them existed long before the advent of React and Vue, a good portion of them took advantage of what already existed and what was hugely adopted by the broad developer community—React’s JSX and Vue.js templates.

Static site generators are frameworks built on top of those templating engines, derivatives that offer something really valuable—performance. And performance is what makes the difference in how well a website ranks on Google.

![](/assets/img/posts/the-rise-of-jamstack/01.png)

Considering [Google’s to push](https://developers.google.com/web/updates/2018/07/search-ads-speed) to speed up web sites—Lighthouse, PageSpeed Insights, AMP (Accelerated Mobile Pages)—and growing demand for API-driven content management, it looks like JAMstack is here to stay for the foreseeable future. But is it the right solution for everyone?

## What are the pros and cons of working with JAMstack?
Statically generated sites can be sourced with data from literally anywhere—locally-stored markdown files, Google Sheets, or, in theory, a traditional CMS. It just makes much more sense to do so with a headless CMS whose main responsibility is managing and delivering content.

Although headless CMSs were initially created to overcome the omnichannel gap, it seems that they create a perfect match with static site generators.

![](/assets/img/posts/the-rise-of-jamstack/02.svg)

Keeping the concerns separated lets you use a [best-of-breed solution](https://kontent.ai/integrations) for each problem while allowing the (micro)services to scale almost indefinitely. Speaking of scalability, serving static files requires almost zero infrastructure and removes the need for boosting your databases’ and web servers’ performance. There is practically no way of achieving better performance and scalability because serving static files is just a matter of transferring them over the wire. The page-load time is limited by the speed of your connection and the hard drive/memory serving the files.

Shrinking the infrastructure also helps cut down the maintenance costs and reduce the attack surface. You see, with static files, there is very little left to hack. In fact, only few things can go wrong in general, which makes the apps very stable.

So, improved performance, greater security, scalability, cost-effectiveness, and offering the same or better developer experience as modern JS frameworks are the advantages. What are the drawbacks?

For those of a less technical persuasion, for example, those working on content, it can be a complex system to master. At the same time, any dynamic features are likely to require the help of a third-party solution. On top of that, any significant alterations of the presentation layer would need the assistance of a developer. Lastly, JAMstack isn’t always appropriate for sites or apps that demand the need for frequent, let alone real-time updates or apps that are mostly dynamic.

## Which frameworks are suitable for building sites with JAMstack?
The beauty of JAMstack is that it doesn’t prescribe the technologies and frameworks to construct websites and apps with. While Gatsby, Vue, React, Hugo, and Jekyll are all featuring prominently, the final cocktail of ingredients is up to you. Mixing the services together might get even easier with initiatives such as [StackBit](https://www.stackbit.com/) and their [Sourcebit](https://github.com/stackbithq/sourcebit) framework that aspire to create a unified interface [between any headless CMS](https://kontent.ai/headless-cms-guide/) and any static site generator.

**So it seems that JAMstack’s rise is set to continue in 2020, but what’s next?**

There is one technology that could potentially disrupt the landscape, and that is WebAssembly. Also known as WASM, WebAssembly is efficient bytecode in a binary format, seen by some as a successor to JavaScript thanks to its native-like performance (1.5x up to 20x faster than JavaScript).

WebAssembly alone could be a notable challenger of the elevated performance standards set by JAMstack.

Still in its relative infancy, it remains to be seen how WASM will reshape the landscape. With the birth of first [WebAssembly front-end frameworks](https://www.webassemblyman.com/webassembly_front_end_web_development.html), it’s undeniable that the technology is getting some serious traction. But rather than replacing JavaScipt, it’s more likely that it’ll supplement it. WebAssembly in combination with JAMstack could give life to a deadly combo offering the best performance of both worlds—statically generated markup and dynamic parts powered by superfast bytecode. Will we start seeing **W**AMstack sites soon?
