using Akka.Actor;
using Serilog;
using System;

namespace Akka.Reactive
{
	using System.Threading.Tasks;
	using JetBrains.Annotations;
	using Messages;

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
		[NotNull]
		internal ActorSystem System	{ get; }

		/// <summary>
		///		A reference to the root Rx-integration management actor.
		/// </summary>
		[NotNull]
		internal IActorRef Manager	{ get; }

		/// <summary>
		///		The top-level logger for the reactive API.
		/// </summary>
		[NotNull]
		internal ILogger Logger		{ get; } = Log.ForContext<ReactiveApi>();
	}
}
