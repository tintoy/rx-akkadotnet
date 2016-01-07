using Akka.Actor;
using JetBrains.Annotations;
using Serilog;
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
			: this(system, manager, Log.Logger)
		{
		}

		/// <summary>
		///		Create a new Rx-integration actor system extension.
		/// </summary>
		/// <param name="system">
		///		The actor system extended by the API.
		/// </param>
		/// <param name="manager">
		///		A reference to the root Rx-integration management actor.
		/// </param>
		/// <param name="logger">
		///		The top-level logger for the reactive API.
		/// </param>
		public ReactiveApi(ActorSystem system, IActorRef manager, ILogger logger)
		{
			if (system == null)
				throw new ArgumentNullException(nameof(system));

			if (manager == null)
				throw new ArgumentNullException(nameof(manager));

			if (logger == null)
				throw new ArgumentNullException(nameof(logger));

			System = system;
			Manager = manager;
			Logger = logger.ForContext("Component", nameof(ReactiveApi));
		}

		/// <summary>
		///		The actor system extended by the API.
		/// </summary>
		[NotNull]
		public ActorSystem System	{ get; }

		/// <summary>
		///		A reference to the root Rx-integration management actor.
		/// </summary>
		[NotNull]
		internal IActorRef Manager	{ get; }

		/// <summary>
		///		The top-level logger for the reactive API.
		/// </summary>
		[NotNull]
		internal ILogger Logger		{ get; }
	}
}
