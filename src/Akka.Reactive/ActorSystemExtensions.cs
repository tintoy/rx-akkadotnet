using Akka.Actor;
using System;

namespace Akka.Reactive
{
	/// <summary>
	///		Extension methods for <see cref="ActorSystem"/>.
	/// </summary>
	public static class ActorSystemExtensions
	{
		/// <summary>
		///		Get the Rx integration API for the actor system.
		/// </summary>
		/// <param name="system">
		///		The actor system.
		/// </param>
		/// <returns>
		///		The Rx integration API.
		/// </returns>
		public static ReactiveApi Reactive(this ActorSystem system)
		{
			if (system == null)
				throw new ArgumentNullException(nameof(system));

			return ReactiveApiProvider.Instance.Apply(system);
		}
	}
}
