using Akka.Actor;
using System;
using System.Reactive;
using System.Reactive.Subjects;

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
		public SubjectManager()
		{
			// Route messages through this actor.
			IActorRef self = Context.Self;
			_input = Observer.Create<TMessage>(
				onNext: message => self.Tell(message),
				onError: error => self.Tell(error),
				onCompleted: () => Context.Stop(self)
			);

			Receive<TMessage>(message => _subject.OnNext(message));
			Receive<Exception>(error => _subject.OnError(error));
			Receive<GetSubjectInterfaces>(
				getSubjectInterfaces => new SubjectInterfaces(observer: _input, observable: _subject)
			);
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

		#region Messages

		/// <summary>
		///		Message requesting the subject's observable / observer interfaces.
		/// </summary>
		public sealed class GetSubjectInterfaces
		{
		}

		/// <summary>
		///		Message providing the subject's observable / observer interfaces.
		/// </summary>
		public sealed class SubjectInterfaces
		{
			/// <summary>
			///		Create a new <see cref="SubjectInterfaces"/> message.
			/// </summary>
			/// <param name="observer">
			///		An <see cref="IObserver{T}"/> representing the subject's observer interface.
			/// </param>
			/// <param name="observable">
			///		An <see cref="IObservable{T}"/> representing the subject's observable interface.
			/// </param>
			public SubjectInterfaces(IObserver<TMessage> observer, IObservable<TMessage> observable)
			{
				if (observer == null)
					throw new ArgumentNullException(nameof(observer));

				if (observable == null)
					throw new ArgumentNullException(nameof(observable));

				Observer = observer;
				Observable = observable;
			}

			/// <summary>
			///		An <see cref="IObserver{T}"/> representing the subject's observer interface.
			/// </summary>
			
			public IObserver<TMessage> Observer { get; }

			/// <summary>
			///		An <see cref="IObservable{T}"/> representing the subject's observable interface.
			/// </summary>
			
			public IObservable<TMessage> Observable { get; }
		}

		#endregion // Messages
	}
}
