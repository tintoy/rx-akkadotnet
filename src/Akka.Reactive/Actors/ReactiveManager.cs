using Akka.Actor;
using System;
using System.Collections.Generic;

namespace Akka.Reactive.Actors
{
	using Messages;

	/// <summary>
	///		The actor that represents the top-level management API.
	/// </summary>
    sealed class ReactiveManager
		: ReceiveActor
	{
		/// <summary>
		///		The well-known name of the reactive management API actor.
		/// </summary>
		public static readonly string ActorName = "reactive-integration";

		/// <summary>
		///		The open generic type for <see cref="TypedSubscriberManager{TMessage}"/>.
		/// </summary>
		static readonly Type SubscriberManagerTypeDef = typeof(TypedSubscriberManager<>);

		/// <summary>
		///		The open generic type for <see cref="SubjectManager{TMessage}"/>.
		/// </summary>
		static readonly Type SubjectManagerTypeDef = typeof(SubjectManager<>);

		/// <summary>
		///		Actors that manage subscriptions for a given base message type, keyed by message type.
		/// </summary>
		readonly Dictionary<Type, IActorRef> _subscriberManagers				= new Dictionary<Type, IActorRef>();

		/// <summary>
		///		Reverse lookup for subscriber managers required to handle actor termination messages.
		/// </summary>
		readonly Dictionary<IActorRef, Type> _subscriberManagerReverseLookup	= new Dictionary<IActorRef, Type>();

		/// <summary>
		///		Create a new RX management API actor.
		/// </summary>
		public ReactiveManager()
		{
			Receive<CreateSubscriber>(createSubscriber => CreateSubscriberActor(createSubscriber));
			Receive<CreateSubject>(createSubjectManager =>
			{
				IActorRef subjectManager = CreateSubjectManager(createSubjectManager);

				Sender.Tell(new SubjectManagerRef(subjectManager));
			});
			Receive<Terminated>(terminated =>
			{
				Type baseMessageType;
				if (!_subscriberManagerReverseLookup.TryGetValue(terminated.ActorRef, out baseMessageType))
					return;

				// Actor has terminated; remove all references to it.
				_subscriberManagers.Remove(baseMessageType);
				_subscriberManagerReverseLookup.Remove(terminated.ActorRef);
			});
		}

		/// <summary>
		///		Create an actor to act as a subscriber on behalf of the specified observer.
		/// </summary>
		/// <param name="createSubscriber">
		///		The <see cref="CreateSubscriber"/> request that specifies the observer to use.
		/// </param>
		/// <returns>
		///		A <see cref="SubscriberCreated"/> message representing the newly-created subscriber.
		/// </returns>
		void CreateSubscriberActor(CreateSubscriber createSubscriber)
		{
			if (createSubscriber == null)
				throw new ArgumentNullException(nameof(createSubscriber));

			IActorRef subscriberManager;
			if (!_subscriberManagers.TryGetValue(createSubscriber.BaseMessageType, out subscriberManager))
			{
				subscriberManager = Context.ActorOf(
					SubscriberManagerProps(createSubscriber.BaseMessageType)
				);
				_subscriberManagers.Add(createSubscriber.BaseMessageType, subscriberManager);
				_subscriberManagerReverseLookup.Add(subscriberManager, createSubscriber.BaseMessageType);

				Context.Watch(subscriberManager);
			}

			// Forward the message to the typed subscriber manager; they'll know what to do with it.
			subscriberManager.Forward(createSubscriber);
		}

		/// <summary>
        /// 	Create a <see cref="SubjectManager{TMessage}"/>.
        /// </summary>
        /// <param name="target">
		///		The target actor represented by the subject.
		/// </param>
        /// <param name="baseMessageType"></param>
        /// <returns>
		///		An <see cref="IActorRef" /> representing the subject manager.
		/// </returns>
		IActorRef CreateSubjectManager(CreateSubject createSubjectManager)
		{
			Type subjectManagerType = SubjectManagerTypeDef.MakeGenericType(createSubjectManager.BaseMessageType);
			
			return Context.ActorOf(
				 Props.Create(subjectManagerType, createSubjectManager.Target)
			);
		}
		
		/// <summary>
		///		Create <see cref="Props"/> for a <see cref="TypedSubscriberManager{TMessage}"/> whose subscribers handle messages of the specified type.
		/// </summary>
		/// <param name="baseMessageType">
		///		The base message type to be used by the subscriber.
		/// </param>
		/// <param name="constructorParameters">
		///		Constructor parameters (if any) for the subscriber manager.
		/// </param>
		/// <returns>
		///		The configured <see cref="Props"/>.
		/// </returns>
		Props SubscriberManagerProps(Type baseMessageType, params object[] constructorParameters)
		{
			if (baseMessageType == null)
				throw new ArgumentNullException(nameof(baseMessageType));

			if (constructorParameters == null)
				throw new ArgumentNullException(nameof(constructorParameters));

			Type subscriberManagerType = SubscriberManagerTypeDef.MakeGenericType(baseMessageType);

			return Props.Create(subscriberManagerType, constructorParameters);
		}
    }
}
