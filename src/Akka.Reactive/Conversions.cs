using Akka.Actor;
using Akka.Event;
using System;
using System.Reactive;
using System.Reactive.Linq;

namespace Akka.Reactive
{
	using Messages;

	/// <summary>
	///		Extension methods for converting between Akka types (e.g. <see cref="IActorRef"/>) and Rx types (e.g. <see cref="IObserver{T}"/> / <see cref="IObservable{T}"/>).
	/// </summary>
    public static class Conversions
    {
		/// <summary>
		///		Create an <see cref="IObserver{T}">observer</see> that can be used to send messages to the specified actor.
		/// </summary>
		/// <typeparam name="TMessage">
		///		The base message type that the observer accepts.
		/// </typeparam>
		/// <param name="actorRef">
		///		A reference to the actor that observed messages will be forwarded to.
		/// </param>
		/// <returns>
		///		The new observer.
		/// </returns>
		public static IObserver<TMessage> ToObserver<TMessage>(this IActorRef actorRef)
		{
			return actorRef.ToObserver<TMessage>(stopOnCompletion: false);
		}
		
		/// <summary>
		///		Create an <see cref="IObserver{T}">observer</see> that can be used to send messages to the specified actor, optionally stopping it when <see cref="IObserver{T}.OnCompleted"/> is called.
		/// </summary>
		/// <typeparam name="TMessage">
		///		The base message type that the observer accepts.
		/// </typeparam>
		/// <param name="actorRef">
		///		A reference to the actor that observed messages will be forwarded to.
		/// </param>
		/// <param name="stopOnCompletion">
		///		Tell the actor to stop (by sending it a <see cref="PoisonPill"/> message) when <see cref="IObserver{T}.OnCompleted"/> is called?
		/// </param>
		/// <returns>
		///		The new observer.
		/// </returns>
		public static IObserver<TMessage> ToObserver<TMessage>(this IActorRef actorRef, bool stopOnCompletion)
		{
			if (actorRef == null)
				throw new ArgumentNullException(nameof(actorRef));

			return Observer.Create<TMessage>(
				onNext: message => actorRef.Tell(message),
				onCompleted: () =>
				{
					if (stopOnCompletion)
						actorRef.Tell(PoisonPill.Instance);
				},
				onError: error => actorRef.Tell(
					new ReactiveSequenceError(error)
				)
			);
		}

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
			return Observable.Create<TEventMessage>(async observer =>
			{
				// TODO: Push both subscriber creation and subscription to event bus down to the subscriber manager and have it unsubscribe when the subscribed actor stops (looks like this behaviour - ManagedActorClassification - is missing from CLR Akka).
				// TODO: Consider having an actor to manage all interaction with the event bus.
				IActorRef subscriber = await system.Reactive().CreateSubscriberAsync(observer);
				eventBus.Subscribe(subscriber, subscriptionClassifier);
			});
		}
	}
}
