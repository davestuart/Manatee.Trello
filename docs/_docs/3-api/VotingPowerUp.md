---
title: VotingPowerUp
category: API
order: 252
---

Represents the Voting power-up.

**Assembly:** Manatee.Trello.dll

**Namespace:** Manatee.Trello

**Inheritance hierarchy:**

- Object
- VotingPowerUp

## Properties

### string Id { get; }

Gets an ID on which matching can be performed.

### bool? IsPublic { get; }

Gets whether the power-up is public. (Really, I don&#39;t know what this is, and Trello&#39;s not talking.)

### string Name { get; }

Gets the power-up name.

## Methods

### Task Refresh(CancellationToken ct)

Refreshes the power-up data.

**Parameter:** ct

(Optional) A cancellation token for async processing.
