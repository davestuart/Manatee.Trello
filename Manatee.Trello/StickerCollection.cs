﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Manatee.Trello.Exceptions;
using Manatee.Trello.Internal.DataAccess;
using Manatee.Trello.Internal.Validation;
using Manatee.Trello.Json;
using Manatee.Trello.Rest;

namespace Manatee.Trello
{
	/// <summary>
	/// A read-only collection of attachments.
	/// </summary>
	public class ReadOnlyStickerCollection : ReadOnlyCollection<ISticker>
	{
		internal ReadOnlyStickerCollection(Func<string> getOwnerId, TrelloAuthorization auth)
			: base(getOwnerId, auth) {}

		/// <summary>
		/// Implement to provide data to the collection.
		/// </summary>
		protected sealed override void Update()
		{
			var endpoint = EndpointFactory.Build(EntityRequestType.Card_Read_Stickers, new Dictionary<string, object> {{"_id", OwnerId}});
			var newData = JsonRepository.Execute<List<IJsonSticker>>(Auth, endpoint);

			Items.Clear();
			Items.AddRange(newData.Select(ja =>
				{
					var attachment = TrelloConfiguration.Cache.Find<Sticker>(a => a.Id == ja.Id) ?? new Sticker(ja, OwnerId, Auth);
					attachment.Json = ja;
					return attachment;
				}));
		}
	}

	/// <summary>
	/// A collection of <see cref="ISticker"/>s.
	/// </summary>
	public interface ICardStickerCollection
	{
		/// <summary>
		/// Adds a <see cref="ISticker"/> to a <see cref="Card"/>.
		/// </summary>
		/// <param name="name">The name of the sticker.</param>
		/// <param name="left">The position of the left edge.</param>
		/// <param name="top">The position of the top edge.</param>
		/// <param name="zIndex">The z-index. Default is 0.</param>
		/// <param name="rotation">The rotation. Default is 0.</param>
		/// <returns>The attachment generated by Trello.</returns>
		/// <exception cref="ValidationException{String}">Thrown when <paramref name="name"/> is null, empty, or whitespace.</exception>
		/// <exception cref="ValidationException{Int32}">Thrown when <paramref name="rotation"/> is less than 0 or greater than 359.</exception>
		ISticker Add(string name, double left, double top, int zIndex = 0, int rotation = 0);
	}

	/// <summary>
	/// A collection of <see cref="ISticker"/>s.
	/// </summary>
	public class CardStickerCollection : ReadOnlyStickerCollection, ICardStickerCollection
	{
		private static readonly NumericRule<int> _rotationRule = new NumericRule<int>{Min = 0, Max = 359};

		internal CardStickerCollection(Func<string> getOwnerId, TrelloAuthorization auth)
			: base(getOwnerId, auth) {}

		/// <summary>
		/// Adds a <see cref="ISticker"/> to a <see cref="Card"/>.
		/// </summary>
		/// <param name="name">The name of the sticker.</param>
		/// <param name="left">The position of the left edge.</param>
		/// <param name="top">The position of the top edge.</param>
		/// <param name="zIndex">The z-index. Default is 0.</param>
		/// <param name="rotation">The rotation. Default is 0.</param>
		/// <returns>The attachment generated by Trello.</returns>
		/// <exception cref="ValidationException{String}">Thrown when <paramref name="name"/> is null, empty, or whitespace.</exception>
		/// <exception cref="ValidationException{Int32}">Thrown when <paramref name="rotation"/> is less than 0 or greater than 359.</exception>
		public ISticker Add(string name, double left, double top, int zIndex = 0, int rotation = 0)
		{
			var error = NotNullOrWhiteSpaceRule.Instance.Validate(null, name);
			if (error != null)
				throw new ValidationException<string>(name, new[] {error});
			error = _rotationRule.Validate(null, rotation);
			if (error != null)
				throw new ValidationException<int>(rotation, new[] {error});

			var parameters = new Dictionary<string, object>
				{
					{"image", name},
					{"top", top},
					{"left", left},
					{"zIndex", zIndex},
					{"rotate", rotation},
				};
			var endpoint = EndpointFactory.Build(EntityRequestType.Card_Write_AddSticker, new Dictionary<string, object> {{"_id", OwnerId}});
			var newData = JsonRepository.Execute<IJsonSticker>(Auth, endpoint, parameters);

			return new Sticker(newData, OwnerId, Auth);
		}

		public void Remove(Sticker sticker)
		{
			var error = NotNullRule<Sticker>.Instance.Validate(null, sticker);
			if (error != null)
				throw new ValidationException<Sticker>(sticker, new[] {error});

			var endpoint = EndpointFactory.Build(EntityRequestType.Card_Write_RemoveSticker,
			                                     new Dictionary<string, object>
				                                     {
					                                     {"_id", OwnerId},
					                                     {"_stickerId", sticker.Id}
				                                     });
			JsonRepository.Execute(Auth, endpoint);
		}
	}

	/// <summary>
	/// A collection of <see cref="ISticker"/>s.
	/// </summary>
	public interface IMemberStickerCollection
	{
		/// <summary>
		/// Adds a <see cref="ISticker"/> to a <see cref="IMember"/>'s custom sticker set by uploading data.
		/// </summary>
		/// <param name="data">The byte data of the file to attach.</param>
		/// <param name="name">A name for the attachment.</param>
		/// <returns>The attachment generated by Trello.</returns>
		ISticker Add(byte[] data, string name);
	}

	/// <summary>
	/// A collection of <see cref="ISticker"/>s.
	/// </summary>
	public class MemberStickerCollection : ReadOnlyStickerCollection, IMemberStickerCollection
	{
		internal MemberStickerCollection(Func<string> getOwnerId, TrelloAuthorization auth)
			: base(getOwnerId, auth) { }

		/// <summary>
		/// Adds a <see cref="ISticker"/> to a <see cref="IMember"/>'s custom sticker set by uploading data.
		/// </summary>
		/// <param name="data">The byte data of the file to attach.</param>
		/// <param name="name">A name for the attachment.</param>
		/// <returns>The attachment generated by Trello.</returns>
		public ISticker Add(byte[] data, string name)
		{
			var parameters = new Dictionary<string, object> { { RestFile.ParameterKey, new RestFile { ContentBytes = data, FileName = name } } };
			var endpoint = EndpointFactory.Build(EntityRequestType.Card_Write_AddAttachment, new Dictionary<string, object> { { "_id", OwnerId } });
			var newData = JsonRepository.Execute<IJsonSticker>(Auth, endpoint, parameters);

			return new Sticker(newData, OwnerId, Auth);
		}
	}
}