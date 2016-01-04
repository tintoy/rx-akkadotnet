using Akka.Actor;
using System;
using System.Reactive;

namespace Akka.Reactive
{
	/// <summary>
	///		Extension methods for <see cref="IActorRef"/>.
	/// </summary>
    public static class ActorRefExtensions
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
				onCompleted: () => actorRef.Tell(PoisonPill.Instance),
				onError: error => actorRef.Tell(
					new ReactiveSubscriptionError(error)
				)
			);
		}
    }
}
