using Akka.Actor;

namespace Akka.Reactive
{
	/// <summary>
	///		The actor that represents the top-level management API.
	/// </summary>
    sealed class ReactiveManager
		: ReceiveActor
	{
		/// <summary>
		///		The well-known name of the reactive management API actor.
		/// </summary>
		public static readonly string ActorName = "reactive-integration";

		/// <summary>
		///		Create a new RX management API actor.
		/// </summary>
		public ReactiveManager()
		{
		}
    }
}
