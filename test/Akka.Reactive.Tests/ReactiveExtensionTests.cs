using Akka.Actor;
using Xunit;

namespace Akka.Reactive.Tests
{
	using Xunit.Abstractions;

	/// <summary>
	///		Tests for the Rx integration extension for Akka actor systems.
	/// </summary>
    public class ReactiveExtensionTests
	{
		/// <summary>
		///		Create a new test suite for the Rx integration extension.
		/// </summary>
		/// <param name="output">
		///		The test output.
		/// </param>
		public ReactiveExtensionTests(ITestOutputHelper output)
		{
			if (output == null)
				throw new System.ArgumentNullException(nameof(output));

			Output = output;
		}

		/// <summary>
		///		Output for the current test run.
		/// </summary>
		public ITestOutputHelper Output
		{
			get;
		}

		/// <summary>
		///		Verify that the Rx integration extension can be retrieved for an Akka actor system.
		/// </summary>
		[Fact]
		public void Can_get_extension_from_actor_system()
		{
			using (ActorSystem system = ActorSystem.Create("Test", TestConfigurations.Default))
			{
				ReactiveApi reactiveApi = system.Reactive();
				Assert.NotNull(reactiveApi);

				system.Terminate();
				system.WhenTerminated.Wait();
			}
		}

		/// <summary>
		///		Verify that the Rx integration extension is a singleton if retrieved multiple times from the same actor system.
		/// </summary>
		[Fact]
		public void Extension_is_singleton_within_actor_system()
		{
			using (ActorSystem system = ActorSystem.Create("Test", TestConfigurations.Default))
			{
				ReactiveApi reactiveApi1 = system.Reactive();
				Assert.NotNull(reactiveApi1);

				ReactiveApi reactiveApi2 = system.Reactive();
				Assert.NotNull(reactiveApi2);

				Assert.Same(reactiveApi1, reactiveApi2);

				system.Terminate();
				system.WhenTerminated.Wait();
			}
		}

		/// <summary>
		///		Verify that the Rx integration extension is not a singleton across multiple Akka actor systems.
		/// </summary>
		[Fact]
		public void Extension_is_not_singleton_across_actor_systems()
		{
			using (ActorSystem system1 = ActorSystem.Create("Test1", TestConfigurations.Default))
			{
				ReactiveApi reactiveApi1 = system1.Reactive();
				Assert.NotNull(reactiveApi1);

				using (ActorSystem system2 = ActorSystem.Create("Test2", TestConfigurations.Default))
				{
					ReactiveApi reactiveApi2 = system2.Reactive();
					Assert.NotNull(reactiveApi2);

					Assert.NotSame(reactiveApi1, reactiveApi2);

					system2.Terminate();
					system2.WhenTerminated.Wait();
				}

				system1.Terminate();
				system1.WhenTerminated.Wait();
			}
		}
	}
}
