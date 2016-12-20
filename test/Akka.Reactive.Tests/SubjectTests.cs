using Akka.TestKit;
using System;
using System.Threading.Tasks;
using System.Reactive.Subjects;
using Xunit;
using Xunit.Abstractions;

using TestKitBase = Akka.TestKit.Xunit2.TestKit;

namespace Akka.Reactive.Tests
{
	/// <summary>
	///		Tests for the Rx subject support.
	/// </summary>
    public class SubjectTests
        : TestKitBase
	{
		/// <summary>
		///		Create a new test suite for Rx subjects.
		/// </summary>
		/// <param name="output">
		///		The test output.
		/// </param>
		public SubjectTests(ITestOutputHelper output)
            : base(TestConfigurations.Default.WithFallback(DefaultConfig))
		{
			if (output == null)
				throw new ArgumentNullException(nameof(output));

			Output = output;
		}

		/// <summary>
		///		Output for the current test run.
		/// </summary>
		public ITestOutputHelper Output { get; }

		/// <summary>
        /// 	Verify that we can create an <see cref="ISubject{T}"/> and send it a string.
        /// </summary>
		[Fact]
		public async Task Can_send_to_subject_of_string()
		{
			TestProbe target = CreateTestProbe();

            ISubject<string> subject = await Sys.Reactive().CreateSubjectAsync<string>(target);
            subject.OnNext("Hello");

            target.ExpectMsg("Hello",
                timeout: TimeSpan.FromSeconds(2)
            );
		}
	}
}
