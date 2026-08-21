---
title: "OK Google, water the garden!"
description: "Let's face it, my brain is just not compatible with any device that has a segment display or more than one button."
slug: ok-google-water-the-garden
date: 2020-07-16
category: technology
tags: [smart-home, rachio, water-irrigation, gardening]
canonical_url: https://medium.com/@PetrSvihlik/ok-google-water-the-garden-29827f29a1cb
draft: true # triage: drop — excluded from the site (issue #64)
---

![](/assets/img/posts/ok-google-water-the-garden/01.jpg)

Let’s face it, my brain is just not compatible with any device that has a segment display or more than one button. Whether it’s a scientific calculator, thermostat, or water irrigation controller, I’m just not patient enough to RTFM or to try to learn and understand all the pictograms.

The first time I ran out of patience was Christmas two years ago when my ABB thermostats went crazy and I was unable to set the temperature to anything lower than the melting point of mithril.

To maintain my mental health, I decided to have them all replaced with Google Nest. It was the best decision I could have made—I no longer have to care about temperature and it saved me plenty money so far.

The second time I almost had gone crazy was when I tried to set up the water irrigation system after winter. This is the bad guy who caused it: Hunter ELC-401i.

![](/assets/img/posts/ok-google-water-the-garden/02.jpg)

Hunter Eco Logic ELC-401i

When I was replacing the thermostats, I left the job to a professional. It turned out to be a wise decision given the amount of the extra wiring necessary.

But this time, I said to myself, I’m gonna be tough, I’m gonna do all by myself. As you can imagine, everything that could have gone wrong went wrong. Let me share my journey with you.

### 💡 Choosing the smart sprinkler controller

My good friend [Jaroslav](https://twitter.com/JaroslavKordula), a home-automation guru, recommended me this cool new controller everyone’s talking about — [Rachio 3rd Generation](https://rachio.com/rachio-3/). I watched the [video](https://rachio.com/) on Rachio’s homepage and I was sold. I couldn’t believe my eyes, it was **exactly** what I needed.

### 📫 Shipping to Europe

The only problem was, that Rachio doesn’t ship to Europe. Which is quite unfortunate, considering my current state of residence being Czechia.

In fact, their product is not prepared for usage outside North America (more on that later) and it can even be illegal to use it.

![](/assets/img/posts/ok-google-water-the-garden/03.png)

There is, however, an option — ordering it from [Amazon US](https://www.amazon.com/gp/product/B07CZ864Y9/) which is exactly what I did. The only downside is the import fees of $43.84 ($43.43 to be precise — I got a $0.41 refund 😂). Good that I take part in online surveys and always have a plenty of Amazon coupons at hand.

Not sure why, but it took almost a month for the controller to be delivered. Longer than most of the stuff I order on AliExpress. COVID-19 I guess.

### ⚡ Power supply

If you think that you’ll just buy a US->EU converter and you’re done, you’re wrong. The original Rachio 3 power supply only supports 110V input (no 230V).

![](/assets/img/posts/ok-google-water-the-garden/04.jpg)

Original Rachio 3 Power Supply

I solved that by ordering a [different one](https://www.onpira.cz/zbozi/napajeci-zdroj-0-5-1a--5-24v-ac/?variantId=12147) with the following specification:

- IN: 230 V AC
- OUT: 24 V AC, 1000mA (1A)

![](/assets/img/posts/ok-google-water-the-garden/05.png)

Noname adapter

And it worked:

![](/assets/img/posts/ok-google-water-the-garden/06.png)

Rachio 3 powered up!

*Note: I learned that AC transformers are not that common here in Europe. It’s much easier to find DC transformers online.*

### 📡 Connecting to Wi-Fi

As you can see, Rachio doesn’t have any display and has almost no controls. 🎉 The setup is done entirely via [the app](https://play.google.com/store/apps/details?id=com.rachio.iro) 😍. As with most of today’s smart devices, the configuration is all cloud-based so you first need to connect the controller to Wi-Fi.

As you can probably guess by now, there was no Wi-Fi signal near the controller. Long story short — I got myself a Wi-Fi repeater.

![](/assets/img/posts/ok-google-water-the-garden/07.jpg)

TP-Link RE205 (AC750 Wi-Fi Extender)

I was surprised how easy it is to set up a repeater nowadays. You just press the [WPS button](https://en.wikipedia.org/wiki/Wi-Fi_Protected_Setup) on your router and the target device and it’s done. I didn’t even know my router has a WPS button. 🤔

![](/assets/img/posts/ok-google-water-the-garden/08.png)

Rachio 3 + TP-Link RE205 + 24VAC Power

### 🔌 Wiring

Finally, something that I got right the first time. Rachio basically copies standard controller wiring. So I went from this:

![](/assets/img/posts/ok-google-water-the-garden/09.jpg)

Hunter ELC 4

To this:

![](/assets/img/posts/ok-google-water-the-garden/10.jpg)

Rachio 3rd Generation — 2 zones setup

- Yellow-green: Common
- Blue: Zone 1
- Brown: Zone 2
- S1/S2: Rain sensors
- Black: Power In

*Note: The rain sensors are not necessary in order for Rachio to work — it uses* [*PWSWeather*](https://pwsweather.com/) *online service for weather forecasting.*

### ⚙️ App configuration

The next step was to set up the zones in the app and create an irrigation schedule. This was a piece of cake.

![](/assets/img/posts/ok-google-water-the-garden/11.png)

Rachio Android App

The only problem was that the sprinklers never started spraying water.

### 🚰 Fixing the water supply

First, I thought there is a different output voltage required in order to start the watering valves. But that didn’t seem likely since there are several [videos](https://www.youtube.com/results?search_query=rachio+hunter) on YouTube demonstrating the transition from Hunter to Rachio.

Then, I thought the water pump is dead. I called a technician who recommended cleaning the filter and indeed, that was it!

I disassembled the pipes and the filter was full of sand.

![](/assets/img/posts/ok-google-water-the-garden/12.jpg)

Clean strainer filter

### 💦 OK Google, water the garden

It took some plumbing, electrical work, networking, but I can finally say “OK Google, water the garden”!

![](/assets/img/posts/ok-google-water-the-garden/13.png)

Achievement unlocked!

What command should I teach my Google Assistant next? 😁
