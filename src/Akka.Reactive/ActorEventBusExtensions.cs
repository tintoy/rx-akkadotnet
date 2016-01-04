using Akka.Actor;
using Akka.Event;
using System;
using System.Reactive.Linq;

namespace Akka.Reactive
{
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
					subscribe: observer => () =>
					{
						// AF: Might be better to have a single EventBus subscriber that then acts as a subject to which multiple Rx observers can be subscribed.
						// AF: Or, at the very least, have a root actor that creates child actors as subscribers and manages their lifetimes.

						// For example, IActorRef subscriberActor = system.Reactive().CreateSubscriberActor(observer);

						IActorRef subscriber = system.ActorOf(
							Props.Create<EventSubscriberActor<TEventMessage>>(observer)
								.WithSupervisorStrategy(
									new OneForOneStrategy(error => // Slightly hacky way of notifying observer of any errors encountered by the subscriber actor.
									{
										observer.OnError(error);
										
										return Directive.Stop;
									})
								)
						);

						eventBus.Subscribe(subscriber, subscriptionClassifier);
					}
				);
		}

		/// <summary>
		///		Actor that forwards received events to an <see cref="IObserver{T}"/>.
		/// </summary>
		/// <typeparam name="TEventMessage">
		///		The base message type used to represent events.
		/// </typeparam>
		class EventSubscriberActor<TEventMessage>
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
			public EventSubscriberActor(IObserver<TEventMessage> eventObserver)
			{
				if (eventObserver == null)
					throw new ArgumentNullException(nameof(eventObserver));

				_eventObserver = eventObserver;

				Receive<TEventMessage>(
					eventMessage => _eventObserver.OnNext(eventMessage)
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
}
