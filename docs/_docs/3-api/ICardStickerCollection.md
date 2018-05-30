---
title: ICardStickerCollection
category: API
order: 71
---

A collection of Manatee.Trello.IStickers.

**Assembly:** Manatee.Trello.dll

**Namespace:** Manatee.Trello

**Inheritance hierarchy:**

- ICardStickerCollection

## Methods

### Task&lt;[ISticker](../ISticker#isticker)&gt; Add(string name, double left, double top, int zIndex, int rotation, CancellationToken ct)

Adds a Manatee.Trello.ISticker to a Manatee.Trello.ICard.

**Parameter:** name

The name of the sticker.

**Parameter:** left

The position of the left edge.

**Parameter:** top

The position of the top edge.

**Parameter:** zIndex

The z-index. Default is 0.

**Parameter:** rotation

The rotation. Default is 0.

**Parameter:** ct

(Optional) A cancellation token for async processing.

**Returns:** The attachment generated by Trello.

**Exception:** Manatee.Trello.ValidationException`1

Thrown when *name* is null, empty, or whitespace.

**Exception:** Manatee.Trello.ValidationException`1

Thrown when *rotation* is less than 0 or greater than 359.

### Task Remove(Sticker sticker, CancellationToken ct)

Removes a Manatee.Trello.ISticker from a Manatee.Trello.ICard.

**Parameter:** sticker

The sticker to remove.

**Parameter:** ct

(Optional) A cancellation token for async processing.
