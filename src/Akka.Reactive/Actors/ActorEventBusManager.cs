using Akka.Actor;
using Akka.Event;
using System;

namespace Akka.Reactive.Actors
{
	/// <summary>
	///		Static implementation for the actor that manages Akka <see cref="ActorEventBus{TEvent,TClassifier}">actor-based event bus</see>es.
	/// </summary>
	static class ActorEventBusManager
	{
		/// <summary>
		///		The generic type definition for <see cref="ActorEventBus{TEvent,TClassifier}"/>.
		/// </summary>
		static readonly Type ActorEventBusTypeDef = typeof(ActorEventBus<,>);

		/// <summary>
		///		Create <see cref="Props"/> for an <see cref="ActorEventBusManager"/> to manage the specified <see cref="ActorEventBus{TEvent,TClassifier}">actor-based event bus</see>.
		/// </summary>
		/// <typeparam name="TEvent">
		///		The base event type used by the bus.
		/// </typeparam>
		/// <typeparam name="TClassifier">
		///		The type used by the bus to classify events.
		/// </typeparam>
		/// <param name="eventBus">
		///		The event bus to be managed by the actor.
		/// </param>
		/// <returns>
		///		The configured <see cref="Props"/>.
		/// </returns>
		public static Props Props<TEvent, TClassifier>(ActorEventBus<TEvent, TClassifier> eventBus)
		{
			if (eventBus == null)
				throw new ArgumentNullException(nameof(eventBus));

	  		Type eventBusType = MakeActorEventBusType<TEvent, TClassifier>();

			return Actor.Props.Create(eventBusType, eventBus);
		}

		#region Helpers

		/// <summary>
		///		Create a closed generic type representing an <see cref="ActorEventBus{TEvent,TClassifier}"/>.
		/// </summary>
		/// <typeparam name="TEvent">
		///		The base event type used by the bus.
		/// </typeparam>
		/// <typeparam name="TClassifier">
		///		The type used by the bus to classify events.
		/// </typeparam>
		/// <returns>
		///		A <see cref="Type"/> representing the event bus type.
		/// </returns>
		static Type MakeActorEventBusType<TEvent, TClassifier>()
		{
			return MakeActorEventBusType(typeof(TEvent), typeof(TClassifier));
			;
		}

		/// <summary>
		///		Create a closed generic type representing an <see cref="ActorEventBus{TEvent,TClassifier}"/>.
		/// </summary>
		/// <param name="eventType">
		///		The base event type used by the bus.
		/// </param>
		/// <param name="classifierType">
		///		The type used by the bus to classify events.
		/// </param>
		/// <returns>
		///		A <see cref="Type"/> representing the event bus type.
		/// </returns>
		static Type MakeActorEventBusType(Type eventType, Type classifierType)
		{
			if (eventType == null)
				throw new ArgumentNullException(nameof(eventType));

			if (classifierType == null)
				throw new ArgumentNullException(nameof(classifierType));

			return ActorEventBusTypeDef.MakeGenericType(eventType, classifierType);
		}

		#endregion // Helpers
	}

	/// <summary>
	///		Actor that manages an <see cref="ActorEventBus{TEvent,TClassifier}">actor-based event bus</see>.
	/// </summary>
	/// <typeparam name="TEvent">
	///		The base event type used by the bus.
	/// </typeparam>
	/// <typeparam name="TClassifier">
	///		The type used by the bus to classify events.
	/// </typeparam>
	sealed class ActorEventBusManager<TEvent, TClassifier>
		: ReceiveActor
	{
		/// <summary>
		///		The event bus being managed by the <see cref="ActorEventBusManager{TEvent,TClassifier}"/>.
		/// </summary>
		readonly ActorEventBus<TEvent, TClassifier> _eventBus;

		/// <summary>
		///		Create a new <see cref="ActorEventBusManager{TEvent,TClassifier}"/>.
		/// </summary>
		/// <param name="eventBus">
		///		The event bus being managed by the <see cref="ActorEventBusManager{TEvent,TClassifier}"/>.
		/// </param>
		public ActorEventBusManager(ActorEventBus<TEvent, TClassifier> eventBus)
		{
			if (eventBus == null)
				throw new ArgumentNullException(nameof(eventBus));

			_eventBus = eventBus;

			Receive<Publish>(publish =>
			{
				_eventBus.Publish(publish.EventMessage);
			});
			Receive<Subscribe>(subscribe =>
			{
				_eventBus.Subscribe(subscribe.Subscriber, subscribe.Classifier);
				Context.Watch(subscribe.Subscriber); // We want to terminate their subscriptions if they are terminated.
			});
			Receive<Unsubscribe>(unsubscribe =>
			{
				_eventBus.Unsubscribe(unsubscribe.Subscriber, unsubscribe.Classifier);

				// If all subscriptions for subscriber are being terminated (not just for a specific classifier, then we're no longer interested in their lifecycle
				if (Equals(unsubscribe.Classifier, default(TClassifier)))
					Context.Unwatch(unsubscribe.Subscriber);
			});
			Receive<Terminated>(actorTerminated =>
			{
				_eventBus.Unsubscribe(actorTerminated.ActorRef);
				Context.Unwatch(actorTerminated.ActorRef);
			});
		}

		#region Messages

		/// <summary>
		///		Marker interface for messages used by the <see cref="ActorEventBusManager{TEvent,TClassifier}"/>.
		/// </summary>
		interface Message
		{
		}

		/// <summary>
		///		Message that requests the subscription of an actor to an event bus.
		/// </summary>
		public sealed class Subscribe
			: Message
		{
			/// <summary>
			///		Create a new <see cref="Subscribe"/> message.
			/// </summary>
			/// <param name="subscriber">
			///		An <see cref="IActorRef"/> representing the actor to subscribe.
			/// </param>
			/// <param name="classifier">
			///		An optional classifier used to filter the events that the actor will be notified about.
			/// </param>
			public Subscribe(IActorRef subscriber, TClassifier classifier = default(TClassifier))
			{
				if (subscriber == null)
					throw new ArgumentNullException(nameof(subscriber));

				Subscriber = subscriber;
				Classifier = classifier;
			}

			/// <summary>
			///		The actor to subscribe to the bus.
			/// </summary>
			public IActorRef Subscriber { get; }

			/// <summary>
			///		An optional classifier used to filter the events that the subscriber will be notified about.
			/// </summary>
			public TClassifier Classifier { get; }
		}

		/// <summary>
		///		Message that requests the termination of an actor's subscription to an event bus.
		/// </summary>
		public sealed class Unsubscribe
			: Message
		{
			/// <summary>
			///		Create a new <see cref="Unsubscribe"/> message.
			/// </summary>
			/// <param name="subscriber">
			///		An <see cref="IActorRef"/> representing the actor to unsubscribe.
			/// </param>
			/// <param name="classifier">
			///		If specified, then the actor will only be unsubscribed from events that match the specified classifier; otherwise, the actor will be unsubscribed from all events.
			/// </param>
			public Unsubscribe(IActorRef subscriber, TClassifier classifier = default(TClassifier))
			{
				if (subscriber == null)
					throw new ArgumentNullException(nameof(subscriber));

				Subscriber = subscriber;
				Classifier = classifier;
			}

			/// <summary>
			///		The actor to subscribe to the bus.
			/// </summary>
			public IActorRef Subscriber { get; }

			/// <summary>
			///		An optional classifier used to filter the events that the subscriber will be notified about.
			/// </summary>
			public TClassifier Classifier { get; }
		}

		/// <summary>
		///		Publish an event on the bus.
		/// </summary>
		public sealed class Publish
			: Message
		{
			/// <summary>
			///		Create a new <see cref="Publish"/> message.
			/// </summary>
			/// <param name="eventMessage">
			///		The event message to publish.
			/// </param>
			public Publish(TEvent eventMessage)
			{
				if (eventMessage == null)
					throw new ArgumentNullException(nameof(eventMessage));

				EventMessage = eventMessage;
			}

			/// <summary>
			///		The event message to publish.
			/// </summary>
			public TEvent EventMessage { get; }
		}

		#endregion // Messages
	}
}
