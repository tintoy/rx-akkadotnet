using System;

namespace Akka.Reactive.Messages
{
	/// <summary>
	///		Message requesting the creation of a subscriber.
	/// </summary>
	abstract class CreateSubscriber
	{
		/// <summary>
		///		The base message type handled by the subscriber.
		/// </summary>
		/// <remarks>
		///		Used to route the request to a type-aware management actor.
		/// </remarks>
		public abstract Type BaseMessageType { get; }
	}

	/// <summary>
	///		Message requesting the creation of a subscriber.
	/// </summary>
	sealed class CreateSubscriber<TMessage>
		: CreateSubscriber
	{
		/// <summary>
		///		Create a new <see cref="CreateSubscriber{TMessage}"/> message.
		/// </summary>
		/// <param name="observer">
		///		The observer that the subscriber will represent.
		/// </param>
		public CreateSubscriber(IObserver<TMessage> observer)
		{
			if (observer == null)
				throw new ArgumentNullException(nameof(observer));

			Observer = observer;
		}

		/// <summary>
		///		The base message type handled by the subscriber.
		/// </summary>
		/// <remarks>
		///		Used to route the request to a type-aware management actor.
		/// </remarks>
		public override Type BaseMessageType => typeof(TMessage);

		/// <summary>
		///		The observer that the subscriber will represent.
		/// </summary>
		
		public IObserver<TMessage> Observer { get; }
	}
}