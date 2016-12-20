using Akka.Actor;

namespace Akka.Reactive.Messages
{
    /// <summary>
    ///		Message providing a reference to a <see cref="SubjectManager{TMessage}"/>.
    /// </summary>
    sealed class SubjectManagerRef
    {
        /// <summary>
        ///     Create a new <see cref="SubjectManagerRef"/> message.
        /// </summary>
        /// <param name="managerRef">
        ///     A reference to the <see cref="SubjectManager{TMessage}"/> actor.
        /// </param>
        public SubjectManagerRef(IActorRef managerRef)
        {
            ManagerRef = managerRef;
        }

        /// <summary>
        ///     A reference to the <see cref="SubjectManager{TMessage}"/> actor.
        /// </summary>
        public IActorRef ManagerRef { get; }
    }
}