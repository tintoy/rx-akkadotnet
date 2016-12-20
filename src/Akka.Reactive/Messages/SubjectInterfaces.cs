using System;

namespace Akka.Reactive.Messages
{
    /// <summary>
    ///		Message providing a subject's observable / observer interfaces.
    /// </summary>
    public sealed class SubjectInterfaces<TMessage>
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
}