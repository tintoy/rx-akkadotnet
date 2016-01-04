using Akka.Actor;
using System;

namespace Akka.Reactive
{
	/// <summary>
	///		The Rx-integration extension for Akka actor systems.
	/// </summary>
    sealed class ReactiveExtension
		: IExtension
    {
		/// <summary>
		///		Create a new Rx-integration actor system extension.
		/// </summary>
		/// <param name="manager">
		///		A reference to the root Rx-integration management actor.
		/// </param>
		public ReactiveExtension(IActorRef manager)
		{
			if (manager == null)
				throw new ArgumentNullException(nameof(manager));

			Manager = manager;
		}

		/// <summary>
		///		A reference to the root Rx-integration management actor.
		/// </summary>
		public IActorRef Manager { get; }
    }
}
