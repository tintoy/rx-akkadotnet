namespace Akka.Reactive.Messages
{
    /// <summary>
    ///		Message requesting the subject's observable / observer interfaces.
    /// </summary>
    sealed class GetSubjectInterfaces
    {
        /// <summary>
        ///     The singleton instance of the <see cref="GetSubjectInterfaces"/> message.
        /// </summary>
        public static GetSubjectInterfaces Instance = new GetSubjectInterfaces();

        /// <summary>
        ///     Create a new <see cref="GetSubjectInterfaces"/> message.
        /// </summary>
        GetSubjectInterfaces()
        {
        }
    }
}