using Akka.TestKit;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
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

		/// <summary>
		/// 	Verify that we can create an <see cref="ISubject{T}"/> and send it a string.
		/// </summary>
		[Fact]
		public async Task Can_receive_from_subject_of_string()
		{
			TestProbe target = CreateTestProbe("Probe1");
			ISubject<string> subject = await Sys.Reactive().CreateSubjectAsync<string>(target);
			
			List<string> responses = new List<string>();
			AutoResetEvent receivedResponse = new AutoResetEvent(false);
			subject.Subscribe(message =>
			{
				responses.Add(message);
				receivedResponse.Set();
			});

			subject.OnNext("Hello");

			Within(TimeSpan.FromSeconds(2), () =>
			{
				target.ExpectMsg<string>(message =>
				{
					Assert.Equal("Hello", message);
					target.Reply("World");
				});
			});

			Assert.True(
				receivedResponse.WaitOne(
					TimeSpan.FromSeconds(2)
				),
				"Timed out waiting for response message."
			);
			
			Assert.Equal(1, responses.Count);
			Assert.Equal("World", responses[0]);
		}
	}
}
