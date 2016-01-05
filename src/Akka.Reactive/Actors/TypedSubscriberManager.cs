using Akka.Actor;
using System;

namespace Akka.Reactive.Actors
{
	using Messages;

	/// <summary>
	///		Actor that manages subscriptions for the specified base message type.
	/// </summary>
	/// <typeparam name="TMessage">
	///		The base message type.
	/// </typeparam>
    public class TypedSubscriberManager<TMessage>
		: ReceiveActor
    {
		/// <summary>
		///		Create a new <see cref="TypedSubscriberManager{TMessage}"/> actor.
		/// </summary>
		public TypedSubscriberManager()
		{
			Receive<CreateSubscriber<TMessage>>(createSubscriber =>
			{
				IActorRef subscriber = CreateSubscriberActor(createSubscriber);

				Sender.Tell(
					new SubscriberCreated(subscriber)
				);
			});
		}

		/// <summary>
		///		Create an actor to act as a subscriber on behalf of the specified observer.
		/// </summary>
		/// <param name="createSubscriber">
		///		The <see cref="CreateSubscriber"/> request message.
		/// </param>
		/// <returns></returns>
		IActorRef CreateSubscriberActor(CreateSubscriber<TMessage> createSubscriber)
		{
			if (createSubscriber == null)
				throw new ArgumentNullException(nameof(createSubscriber));

			IObserver<TMessage> observer = createSubscriber.Observer;

			return Context.ActorOf(
				Props.Create<EventSubscriber<TMessage>>(observer)
					.WithSupervisorStrategy(
						new OneForOneStrategy(error => // Slightly hacky way of notifying observer of any errors encountered by the subscriber actor.
						{
							observer.OnError(error);

							return Directive.Stop;
						})
					)
			);
		}
	}
}
