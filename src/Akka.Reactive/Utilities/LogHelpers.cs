using Akka.Actor;
using Akka.Event;
using Akka.Logger.Serilog;
using System;

namespace Akka.Reactive.Utilities
{
	/// <summary>
	///		Helper functions for logging.
	/// </summary>
    public static class LogHelpers
    {
		/// <summary>
		///		Get a Serilog-formatted <see cref="ILoggingAdapter">Akka logger</see> for the current actor.
		/// </summary>
		/// <param name="actorContext">
		///		The actor context.
		/// </param>
		/// <returns>
		///		An <see cref="ILoggingAdapter"/> that accepts messages in Serilog format (named format holes).
		/// </returns>
		public static ILoggingAdapter GetSerilogger(this IActorContext actorContext)
		{
			if (actorContext == null)
				throw new ArgumentNullException(nameof(actorContext));

			return actorContext.GetLogger(
				new SerilogLogMessageFormatter()
			);
		}
	}
}
