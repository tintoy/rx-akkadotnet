namespace Akka.Reactive.Messages
{
	/// <summary>
	///		Message used to notify an Akka subscriber actor that an Rx sequence to which it is subscribed has completed.
	/// </summary>
    public sealed class ReactiveSequenceCompleted
	{
		/// <summary>
		///		The singleton instance of the <see cref="ReactiveSequenceCompleted"/> message.
		/// </summary>
		public static ReactiveSequenceCompleted Instance = new ReactiveSequenceCompleted();

		/// <summary>
		///		Create a new <see cref="ReactiveSequenceError"/>.
		/// </summary>
		ReactiveSequenceCompleted()
		{
		}
	}
}
