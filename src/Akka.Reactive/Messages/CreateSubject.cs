using Akka.Actor;
using System;

namespace Akka.Reactive.Messages
{
    /// <summary>
    ///     Message requesting the creation of a subject manager.
    /// </summary>
    sealed class CreateSubject
    {
        /// <summary>
        ///     Create a new <see cref="CreateSubject"/> message.
        /// </summary>
        /// <param name="target">
        ///     The target actor represented by the subject.
        /// </param>
        /// <param name="baseMessageType">
        ///     The target actor represented by the subject.
        /// </param>
        public CreateSubject(IActorRef target, Type baseMessageType)
        {
            Target = target;
            BaseMessageType = baseMessageType;
        }

        /// <summary>
        ///     The target actor represented by the subject.
        /// </summary>
        public IActorRef Target { get; }

        /// <summary>
		///		The base message type handled by the subject.
		/// </summary>
		public Type BaseMessageType { get; }
    }
}