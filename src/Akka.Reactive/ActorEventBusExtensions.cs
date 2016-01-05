using Akka.Actor;
using Akka.Event;
using System;
using System.Reactive.Linq;

namespace Akka.Reactive
{
	using Messages;

	/// <summary>
	///		Rx-related extension methods for Akka's <see cref="ActorEventBus{TEvent,TClassifier}"/>.
	/// </summary>
	/// <remarks>
	///		AF: This is a work-in-progress. I'm in the progress of refactoring some of the supporting infrastructure as an actor system extension (well-known end point for tracking actor lifetime and calling OnCompleted / OnError as appropriate).
	/// </remarks>
	public static class ActorEventBusExtensions
    {
		/// <summary>
		///		Create a new <see cref="IObservable{T}">observable</see> that forwards messages from an <see cref="ActorEventBus{TEvent,TClassifier}">actor-based event bus</see> subscription to subscribed <see cref="IObserver{T}">observer</see>s.
		/// </summary>
		/// <typeparam name="TEventMessage">
		///		The base message type used to represent events from the bus.
		/// </typeparam>
		/// <typeparam name="TEventClassifier">
		///		The type used to classify events on the bus (i.e. select the "channel" or "audience" for which a given event is intented).
		/// </typeparam>
		/// <param name="eventBus">
		///		The actor-based event bus.
		/// </param>
		/// <param name="system">
		///		The actor system that will host the subscribing actor (usually, this is the actor system that hosts the event bus).
		/// </param>
		/// <param name="subscriptionClassifier">
		///		The classifier value that is passed to the event bus when subscribing.
		/// </param>
		/// <returns>
		///		The <see cref="IObservable{T}">observable</see>.
		/// </returns>
		public static IObservable<TEventMessage> ToObservable<TEventMessage, TEventClassifier>(this ActorEventBus<TEventMessage, TEventClassifier> eventBus, ActorSystem system, TEventClassifier subscriptionClassifier)
		{
			if (eventBus == null)
				throw new ArgumentNullException(nameof(eventBus));

			if (system == null)
				throw new ArgumentNullException(nameof(system));

			// When an observer subscribes, create an actor to represent them and subscribe it to the bus.
			return
				Observable.Create<TEventMessage>(
					async observer =>
					{
						// TODO: Push this down to the subscriber manager and have it unsubscribe when the subscribed actor stops (looks like this behaviour - ManagedActorClassification - is missing from CLR Akka).

						SubscriberCreated subscriberCreated = await system.Reactive().Manager.Ask<SubscriberCreated>( // TODO: Make this an extension method for ReactiveExtension.
							new CreateSubscriber<TEventMessage>(observer),
							timeout: TimeSpan.FromSeconds(30) // TODO: Make this configurable.
						);

						eventBus.Subscribe(subscriberCreated.Subscriber, subscriptionClassifier);
					}
				);
		}
    }
}
