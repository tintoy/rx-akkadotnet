using Akka.Actor;
using System;

namespace Akka.Reactive
{
	/// <summary>
	///		The Rx-integration extension for Akka actor systems.
	/// </summary>
	public sealed class ReactiveApi
		: IExtension
	{
		/// <summary>
		///		Create a new Rx-integration actor system extension.
		/// </summary>
		/// <param name="system">
		///		The actor system extended by the API.
		/// </param>
		/// <param name="manager">
		///		A reference to the root Rx-integration management actor.
		/// </param>
		public ReactiveApi(ActorSystem system, IActorRef manager)
		{
			if (system == null)
				throw new ArgumentNullException(nameof(system));

			if (manager == null)
				throw new ArgumentNullException(nameof(manager));

			System = system;
			Manager = manager;
		}

		/// <summary>
		///		The actor system extended by the API.
		/// </summary>
		
		public ActorSystem System	{ get; }

		/// <summary>
		///		A reference to the root Rx-integration management actor.
		/// </summary>
		
		internal IActorRef Manager	{ get; }
	}
}
