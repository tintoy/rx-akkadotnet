using Akka.Actor;
using System;
using System.Threading.Tasks;

namespace Akka.Reactive
{
	using Messages;

	/// <summary>
	///		Extension methods for the <see cref="ReactiveApi">reactive API</see>.
	/// </summary>
    public static class ReactiveApiExtensions
    {
		/// <summary>
		///		Asynchronously create a subscriber for the specified <see cref="IObserver{T}">observer</see>.
		/// </summary>
		/// <typeparam name="TMessage">
		///		The base message type handled by the observer.
		/// </typeparam>
		/// <param name="api">
		///		The reactive API.
		/// </param>
		/// <param name="observer">
		///		The observer.
		/// </param>
		/// <param name="timeout">
		///		An optional time span (defaults to 30 seconds) to wait for the subscriber to be created.
		/// </param>
		/// <returns>
		///		A reference to the subscriber actor.
		/// </returns>
		public static async Task<IActorRef> CreateSubscriberAsync<TMessage>(this ReactiveApi api, IObserver<TMessage> observer, TimeSpan? timeout = null)
		{
			if (api == null)
				throw new ArgumentNullException(nameof(api));

			if (observer == null)
				throw new ArgumentNullException(nameof(observer));

			api.Logger.Verbose("Creating subscriber actor for messages of type {MessageType}...", typeof(TMessage).FullName);

			SubscriberCreated subscriberCreated = await api.Manager.Ask<SubscriberCreated>(
				new CreateSubscriber<TMessage>(observer),
				timeout ?? TimeSpan.FromSeconds(30)
			);

			api.Logger.Verbose("Created subscriber actor {ActorPath} for messages of type {MessageType}...", subscriberCreated.Subscriber.Path.ToStringWithoutAddress(), typeof(TMessage).FullName);

			return subscriberCreated.Subscriber;
		}
	}
}
