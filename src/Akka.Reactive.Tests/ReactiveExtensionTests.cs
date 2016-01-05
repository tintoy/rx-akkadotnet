using Akka.Actor;
using Xunit;

namespace Akka.Reactive.Tests
{
	/// <summary>
	///		Tests for the Rx integration extension for Akka actor systems.
	/// </summary>
    public class ReactiveExtensionTests
    {
		/// <summary>
		///		Verify that the Rx integration extension can be retrieved for an Akka actor system.
		/// </summary>
		[Fact]
		public void Can_get_extension_from_actor_system()
		{
			using (ActorSystem system = ActorSystem.Create("Test"))
			{
				ReactiveApi reactiveApi = system.Reactive();
				Assert.NotNull(reactiveApi);

				system.Shutdown();

				system.AwaitTermination();
			}
		}

		/// <summary>
		///		Verify that the Rx integration extension is a singleton if retrieved multiple times from the same actor system.
		/// </summary>
		[Fact]
		public void Extension_is_singleton_within_actor_system()
		{
			using (ActorSystem system = ActorSystem.Create("Test"))
			{
				ReactiveApi reactiveApi1 = system.Reactive();
				Assert.NotNull(reactiveApi1);

				ReactiveApi reactiveApi2 = system.Reactive();
				Assert.NotNull(reactiveApi2);

				Assert.Same(reactiveApi1, reactiveApi2);

				system.Shutdown();

				system.AwaitTermination();
			}
		}

		/// <summary>
		///		Verify that the Rx integration extension is not a singleton across multiple Akka actor systems.
		/// </summary>
		[Fact]
		public void Extension_is_not_singleton_across_actor_systems()
		{
			using (ActorSystem system1 = ActorSystem.Create("Test1"))
			{
				ReactiveApi reactiveApi1 = system1.Reactive();
				Assert.NotNull(reactiveApi1);

				using (ActorSystem system2 = ActorSystem.Create("Test2"))
				{
					ReactiveApi reactiveApi2 = system2.Reactive();
					Assert.NotNull(reactiveApi2);

					Assert.NotSame(reactiveApi1, reactiveApi2);

					system2.Shutdown();
					system2.AwaitTermination();
				}

				system1.Shutdown();

				system1.AwaitTermination();
			}
		}
	}
}
