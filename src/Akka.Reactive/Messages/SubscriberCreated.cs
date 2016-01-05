using Akka.Actor;
using System;

namespace Akka.Reactive.Messages
{
	/// <summary>
	///		Message supplying a newly-created subscriber.
	/// </summary>
	sealed class SubscriberCreated
	{
		/// <summary>
		///		Create a new <see cref="SubscriberCreated"/> message.
		/// </summary>
		/// <param name="subscriber">
		///		The newly-created subscriber.
		/// </param>
		public SubscriberCreated(IActorRef subscriber)
		{
			if (subscriber == null)
				throw new ArgumentNullException(nameof(subscriber));

			Subscriber = subscriber;
		}

		/// <summary>
		///		A reference to the newly-created subscriber.
		/// </summary>
		public IActorRef Subscriber { get; }
	}
}