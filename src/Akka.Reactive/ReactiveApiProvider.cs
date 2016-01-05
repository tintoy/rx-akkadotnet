using Akka.Actor;
using System;

namespace Akka.Reactive
{
	using Actors;

	/// <summary>
	///		Extension provider for Rx integration.
	/// </summary>
	class ReactiveApiProvider :
		ExtensionIdProvider<ReactiveApi>
	{
		/// <summary>
		///		The singleton instance of the Rx-integration extension provider.
		/// </summary>
		public static readonly ReactiveApiProvider Instance = new ReactiveApiProvider();

		/// <summary>
		///		Create a new Rx-integration extension provider.
		/// </summary>
		ReactiveApiProvider()
		{
		}

		/// <summary>
		///		Create an instance of the extension.
		/// </summary>
		/// <param name="system">
		///		The actor system being extended.
		/// </param>
		/// <returns>
		///		The extension.
		/// </returns>
		public override ReactiveApi CreateExtension(ExtendedActorSystem system)
		{
			if (system == null)
				throw new ArgumentNullException(nameof(system));
			
			IActorRef manager = system.ActorOf(
				Props.Create<ReactiveManager>(),
				name: ReactiveManager.ActorName
			);

			return new ReactiveApi(system, manager);
		}
	}
}