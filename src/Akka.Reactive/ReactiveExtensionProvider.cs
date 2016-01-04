using Akka.Actor;
using System;

namespace Akka.Reactive
{
	/// <summary>
	///		Extension provider for Rx integration.
	/// </summary>
	class ReactiveExtensionProvider :
		ExtensionIdProvider<ReactiveExtension>
	{
		/// <summary>
		///		The singleton instance of the Rx-integration extension provider.
		/// </summary>
		public static readonly ReactiveExtensionProvider Instance = new ReactiveExtensionProvider();

		/// <summary>
		///		Create a new Rx-integration extension provider.
		/// </summary>
		ReactiveExtensionProvider()
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
		public override ReactiveExtension CreateExtension(ExtendedActorSystem system)
		{
			if (system == null)
				throw new ArgumentNullException(nameof(system));
			
			IActorRef manager = system.ActorOf(
				Props.Create<ReactiveManager>(),
				name: ReactiveManager.ActorName
			);

			return new ReactiveExtension(manager);
		}
	}
}