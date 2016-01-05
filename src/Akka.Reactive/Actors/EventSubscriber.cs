using Akka.Actor;
using System;

namespace Akka.Reactive.Actors
{
	using Messages;

	/// <summary>
	///		Actor that forwards received events to an <see cref="IObserver{T}"/>.
	/// </summary>
	/// <typeparam name="TEventMessage">
	///		The base message type used to represent events.
	/// </typeparam>
	class EventSubscriber<TEventMessage>
		: ReceiveActor
	{
		/// <summary>
		///		The <see cref="IObserver{T}"/> to which events are forwarded.
		/// </summary>
		readonly IObserver<TEventMessage> _eventObserver;

		/// <summary>
		///		Create a new event-subscriber actor.
		/// </summary>
		/// <param name="eventObserver">
		///		The <see cref="IObserver{T}"/> to which events are forwarded.
		/// </param>
		public EventSubscriber(IObserver<TEventMessage> eventObserver)
		{
			if (eventObserver == null)
				throw new ArgumentNullException(nameof(eventObserver));

			_eventObserver = eventObserver;

			Receive<TEventMessage>(
				eventMessage => _eventObserver.OnNext(eventMessage)
			);
			Receive<ReactiveSequenceError>(
				sequenceError => _eventObserver.OnError(sequenceError.Error)
			);
			Receive<ReactiveSequenceCompleted>(
				sequenceCompleted => Context.Stop(Self)
			);
		}

		/// <summary>
		///		Called when the actor is stopped.
		/// </summary>
		protected override void PostStop()
		{
			_eventObserver.OnCompleted();

			base.PostStop();
		}
	}
}
