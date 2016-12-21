using Akka.Actor;
using System;
using System.Reactive.Subjects;
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

			SubscriberCreated subscriberCreated = await api.Manager.Ask<SubscriberCreated>(
				new CreateSubscriber<TMessage>(observer),
				timeout ?? TimeSpan.FromSeconds(30)
			);

			return subscriberCreated.Subscriber;
		}

		/// <summary>
		/// 	Create a <see cref="Subject{T}"/> to represent the specified target actor.
		/// </summary>
		/// <typeparam name="T">
		///		The type of element handled by the subject.
		/// </typeparam>
		/// <param name="api">
		///		The reactive API.
		/// </param>
		/// <param name="target">
		///		The actor to be represented by the subject.
		/// </param>
		/// <returns>
		///		The <see cref="Subject{T}"/>.
		/// </returns>
		public static async Task<ISubject<T>> CreateSubjectAsync<T>(this ReactiveApi api, IActorRef target)
		{
			if (api == null)
				throw new ArgumentNullException(nameof(api));

			if (target == null)
				throw new ArgumentNullException(nameof(target));

			var subjectManager = await
				api.Manager.Ask<SubjectManagerRef>(
					new CreateSubject(target, typeof(T))
				);
			var subjectInterfaces = await
				subjectManager.ManagerRef.Ask<SubjectInterfaces<T>>(
					GetSubjectInterfaces.Instance
				);

			return Subject.Create<T>(subjectInterfaces.Observer, subjectInterfaces.Observable);
		}
	}
}
