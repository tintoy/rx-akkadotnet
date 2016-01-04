using System;

namespace Akka.Reactive
{
	/// <summary>
	///		Message used to notify an Akka actor that an Rx event source to which it is subscribed encountered an error.
	/// </summary>
    public sealed class ReactiveSubscriptionError
    {
		/// <summary>
		///		Create a new <see cref="ReactiveSubscriptionError"/>.
		/// </summary>
		/// <param name="error">
		///		An <see cref="Exception"/> representing the error.
		/// </param>
		public ReactiveSubscriptionError(Exception error)
		{
			if (error == null)
				throw new ArgumentNullException(nameof(error));

			Error = error;
		}

		/// <summary>
		///		An <see cref="Exception"/> representing the error encountered by the event source.
		/// </summary>
		public Exception Error { get; }
	}
}
