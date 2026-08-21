---
title: "AI Is Not a Tooling Conversation. It's an Organizational Stress Test."
description: "Takeaways from the Pragmatic Summit in San Francisco on AI's organizational impact on engineering teams."
slug: ai-tooling-organizational-stress-test
date: 2026-02-20
category: leadership
tags: [ai, engineering, leadership]
comments: true
---

*Takeaways from the Pragmatic Summit in San Francisco.*

Last week, I was in San Francisco for the [Pragmatic Summit](https://www.pragmaticsummit.com/), organized by Gergely Orosz. A room full of engineering leaders — from scale-ups to Big Tech — all there to talk about what AI is actually doing to how we build software.

If you expected a talk about tools, you'd be wrong. What we actually talked about was something much harder: what happens to organizations, teams, and people when the ground shifts this fast under their feet.

Here's what I took away.

---

## Nobody Knows Anything

Let's start with the uncomfortable truth that Kent Beck articulated better than anyone: "Nobody knows any answer to anything. Devs don't feel safe."

He's right. And not just devs — nobody in a leadership seat feels safe either, even if they won't admit it.

The pace of change is [unlike anything we've experienced](https://www.youtube.com/shorts/vGKC9LpGnOQ), and the desired answer we're seeking is changing every week. The big AI players don't close contracts longer than three months with their suppliers and vendors. Three months. That's how unpredictable the near future is. You can make an order-of-magnitude prediction about what AI will look like six months from now. What happens in two years? Nobody even tries.

What worked last week literally doesn't work next week. One of the more striking trends discussed: teams are already migrating from MCPs to CLIs — MCPs eat up the context window, CLIs are more deterministic, can be piped into sequences, and are easier to reason about. This shift has been happening in weeks, not years. The tools are evolving so fast that the top engineers at OpenAI are burning through roughly 100 billion tokens per week, often running multiple agents in parallel, with overnight agentic runs becoming increasingly common as models improve at long-running sessions.

And then there's the money: one software company discussed plans to spend $15,000 per month per developer on AI tooling — $3M over the next few years. This isn't experimentation anymore. These are serious bets placed under radical uncertainty.

Virtually everyone at the summit agreed: AI is the biggest paradigm shift ever. There's AI, then a long gap, and then second comes the internet, the microprocessor, everything else. Whether or not you share that conviction, the room did — and these are people who've built and run engineering orgs at scale.

---

## The Amplification Thesis

Here's the insight that hit me hardest. During Laura Tacho's talk, she shared data that some companies saw incidents double after adopting AI in engineering. Others saw them cut in half.

Same tools. Opposite outcomes.

The pattern? AI didn't change the organization. It amplified it. Healthy teams got leverage. Dysfunctional teams got chaos — just faster.

This reframing matters. I don't see AI as a tooling conversation or a personal productivity hack anymore. It's an organizational maturity test. And if we don't address structural and cultural issues first, AI will simply help us execute dysfunction more efficiently and fail at scale.

AI is the catalyst that makes some existing problems more visible and more prevalent. And here's the part that might not be popular: the recommendation from multiple leaders in the room was to use AI as an excuse to fix problems you've wanted to fix for years but never got the budget or priority for. That re-org you've been putting off? The flaky test suite? The lack of clear ownership? Now you have a reason. AI will expose it anyway — better to address it on your terms.

Companies are mostly using a soft mandate approach — encouraging AI adoption rather than forcing it. But even encouragement requires a healthy foundation to land on.

---

## The Ground Is Shifting Under Every Role

What we've known as DX (Developer Experience) is turning into AX — Agentic Experience. Martin Fowler quipped that the Venn diagram of DX and AX is a circle, and he's not far off. But there's one key addition: institutional knowledge needs to be accessible in the code, in place, for both agents and people. Your architecture decisions, your API conventions, your patterns — if they only exist in someone's head or buried in a Confluence page nobody reads, your agents are flying blind.

The role boundaries are dissolving fast. The consensus was that PMs and engineers are converging. The open question is: who gets there first? Will PMs become builders faster than engineers gain business acumen? It almost doesn't matter — the advice was to enable everyone. If a PM wants to build a feature, let them. If an engineer wants to own the product problem, even better. The primary customer of Platform Engineering used to be the software engineer; now it should be the "builder" — PMs, engineers, UX designers, anyone creating.

We'll see the rise of EngOps — Engineering Operations teams — that focus on enabling this broader set of builders. And we'll still need specialists, a smaller number of people who influence the center of gravity: defining guardrails, patterns, API guidelines, evals, and quality review techniques. Not everyone becomes a generalist, but the generalist surface area grows.

Code reviews are changing fundamentally. The blunt reality: people don't like reviewing other people's AI slop. And the volume is about to explode. The emerging thinking is that we won't review code the way we used to. We'll review inputs and high-level outputs. If we focus on tests that represent important user scenarios, we almost don't need to look at the implementation. Tools like the recently announced [entire.io](http://entire.io) are early signals of this shift.

AI-authored code — code written without human intervention — is rising fast. We need to get comfortable with that.

---

## Cognitive Debt Is the New Tech Debt

Here's something that didn't get enough airtime at the summit, but I keep thinking about.

We're trading tech debt for cognitive debt.

Running multiple agents can get a lot of jobs done. But it's draining for the humans to stay in charge — orchestrating, reviewing, context-switching, keeping track of what each agent is doing and whether it's going off the rails. The throughput goes up, but so does the mental load.

There's a possibility that in the future, people will work just a couple of hours a day but get 15x the output. That sounds utopian until you realize those couple of hours might be the most cognitively intense hours you've ever worked.

Virtually all engineering leaders at the summit agreed that building is more fun today — "coding is fun again" was a common refrain, and the gratification cycle is shorter. But they also acknowledged the flip side: for the hard-core developers who found deep joy in the craft of writing code, something has shifted. As one speaker put it, "Making the one thing right doesn't make any difference anymore. The joy is gone; you need to look for the joy elsewhere."

And the security and guardrail gaps are real. One company shared a cautionary tale: a BDR built an AI-powered app to help manage customer contacts. They proudly showed it to a technical colleague, who discovered the app was public-internet-facing — effectively exposing their entire lead and customer list to the world. Company secrets, out in the open. This is what happens when the barrier to building drops, but the awareness of what you're exposing doesn't keep up.

---

## Metrics Won't Save You. Follow the Friction.

Nobody has figured out AI metrics. There are various frameworks, various approaches, but none of them is a silver bullet. The more practical recommendation from the room: focus on friction points, not metrics.

AI is causing friction points and bottlenecks, but they keep moving. Once you solve one, another appears downstream. The progression is remarkably consistent: first, you tackle how to adopt the AI tooling, then how to go fully agentic, then how to deal with the volume of code generated, then how to handle the flood of pull requests and code reviews, and then suddenly product managers become the bottleneck — they need to become 10x more productive too.

To go faster, we need higher quality. We invest in features, and we need to invest in "futures." We often have incentives for features, but it's really hard to measure futures. The companies that figure this out will compound their advantage.

---

## How to Survive

So what do you actually do?

Balance skepticism and curiosity. Be skeptical about your own skepticism. When something looks like bullshit, ask yourself: "How can I probe to see if it's actually true?" Allow yourself to say "I don't know." That's not weakness — in a landscape this uncertain, it's the most honest position available.

Experiment with setups. Try pair programming with an agent. Try mob programming — two people and one agent, two people and multiple agents, multiple people and multiple agents. The combinations matter, and nobody knows which ones work best for which problems yet.

Force yourself not to look at the code. It sounds counterintuitive, but it was common in the past to say "I don't trust C#, I want to see the assembly code it generated." AI is yet another layer of abstraction. We need to learn to trust the layer, verify through tests and outcomes, and let go of the need to read every line.

Invest in onboarding. One of the precursors for high productivity is fast onboarding. Research shows that if you create an engaging, quick onboarding experience, the enthusiasm is maintained for the next two years. In a world of rapidly evolving tools and approaches, how fast people get up to speed is a competitive advantage.

Bet on the next generation. Cloudflare announced plans to [hire 1,111 interns](https://blog.cloudflare.com/cloudflare-1111-intern-program/) — even as the industry narrative says junior roles are being eliminated. Their rationale is compelling: only the next generation will be truly "AI native." Not in the marketing sense that older companies use to sound modern, but in the real sense — they grew up with AI in their hands. They're willing to drop workflows they learned three months ago and replace them with new ones, because they have no attachment to how things were done before.

I recommend reading "[Something Big Is Happening](https://shumer.dev/something-big-is-happening)" by Matt Shumer for more actionable survival advice.

---

## The Question Behind the Question

The question isn't "How do we use these tools effectively?"

The question is "How do we figure out how to use the tools effectively?" — and keep figuring it out, over and over, as the answer changes beneath our feet.

A fun aside from the summit: one of the recommended ways to engage with AI on hard technical topics is to ask it to tell you a fairy tale — "Hey AI, tell me a story about this API" or "Tell me a bedtime story about this GPU algorithm." It sounds silly. It works. Sometimes the best way to understand something deeply is to let the abstraction become a narrative.

That, in a way, captures the spirit of the whole summit. We're all telling ourselves stories about where this goes. The honest ones admit they don't know the ending. The smart ones are building organizations that can adapt to whichever story turns out to be true.
