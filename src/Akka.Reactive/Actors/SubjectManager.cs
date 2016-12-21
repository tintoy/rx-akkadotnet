using Akka.Actor;
using System;
using System.Reactive;
using System.Reactive.Subjects;
using Akka.Reactive.Messages;

namespace Akka.Reactive.Actors
{
	/// <summary>
	///		Actor that manages the underlying <see cref="ISubject{TSource,TResult}"/> used to provide pub-sub semantics.
	/// </summary>
	/// <typeparam name="TMessage">
	///		The base message type handled by the subject.
	/// </typeparam>
    sealed class SubjectManager<TMessage>
		: ReceiveActor
	{
		/// <summary>
		///		The input observer used to route messages to this actor.
		/// </summary>
		readonly IObserver<TMessage>	_input;

		/// <summary>
		///		The underlying Rx subject.
		/// </summary>
		readonly Subject<TMessage>      _subject = new Subject<TMessage>();

		/// <summary>
		///		Create a new <see cref="ISubject{TSource,TResult}"/> manager.
		/// </summary>
	  	/// <param name="target">
		///		The target actor represented by the subject.
		/// </param>
		public SubjectManager(IActorRef target)
		{
			if (target == null)
				throw new ArgumentNullException(nameof(target));

			// Route messages through this actor.
			IActorRef self = Context.Self;
			// TODO: Consider using target.ToSubject().
			_input = Observer.Create<TMessage>(
				onNext: message => target.Tell(message),
				onError: error => target.Tell(new ReactiveSequenceError(error)),
				onCompleted: () =>
				{
					// Sequence complete; stop target.
					target.Tell(PoisonPill.Instance);

					Context.Stop(self);
				}
			);

			Receive<TMessage>(message => _subject.OnNext(message));
			Receive<Exception>(error => _subject.OnError(error));
			Receive<GetSubjectInterfaces>(getSubjectInterfaces =>
			{
				Sender.Tell(new SubjectInterfaces<TMessage>(
					observer: _input, observable: _subject
				));
			});
		}

		/// <summary>
		///		Called when the actor is stopped.
		/// </summary>
		protected override void PostStop()
		{
			using (_subject)
			{
				_subject.OnCompleted();
			}

			base.PostStop();
		}
	}
}
