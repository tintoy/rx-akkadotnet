using System;

namespace Akka.Reactive.Messages
{
	/// <summary>
	///		Message used to notify an Akka subscriber actor that an Rx sequence to which it is subscribed has encountered an error.
	/// </summary>
	public sealed class ReactiveSequenceError
    {
		/// <summary>
		///		Create a new <see cref="ReactiveSequenceError"/>.
		/// </summary>
		/// <param name="error">
		///		An <see cref="Exception"/> representing the error encountered by the sequence's source.
		/// </param>
		public ReactiveSequenceError(Exception error)
		{
			if (error == null)
				throw new ArgumentNullException(nameof(error));

			Error = error;
		}

		/// <summary>
		///		An <see cref="Exception"/> representing the error encountered by the sequence's source.
		/// </summary>
		public Exception Error { get; }
	}
}
