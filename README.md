# rx-akkadotnet
Reactive Extensions (Rx) integration for Akka.NET

This library was originally used to sketch out ideas, but is now being actively developed based on community feedback.
Contributions or contributors are welcome :)

Note that this targets .NET Core, and so if you want to work with it you'll need to add the [Akka.NET CoreCLR package feed](NuGet.config) to your NuGet.config.

Functionality currently includes:
* Wrap an `IActorRef` in an `IObserver`
   * Uses the actor's designated scheduler to deliver messages to it
   * Calling `IObserver::OnError` will deliver a special messsage to the actor containing the error.
      * Will consider making this behaviour customisable, depending on use case
         * E.g. Stop actor, somehow cause it to fault, or even specify a supervision policy.
   * Calling `IObserver::OnCompleted` will by default send the actor a `PoisonPill` message (but this will be configurable)
* Subject actor (and wrapper `ISubject` implementation for communicating with it)
   * Messages sent to `IObserver::OnNext` are sent to the actor to be published via `IObservable::Subscribe`
   * Calling `IObserver::OnError` will deliver a special messsage to the target actor containing the error.
   * Subject actor terminates once `IObservable::OnCompleted` is called
   * Target actor will (optionally?) be sent a `PoisonPill` when `IObservable::OnCompleted` is called
   * Subject's outgoing `OnCompleted` will not be called until the subject actor itself has terminated.
* Rx wrappers for for Akka `ActorEventBus`
   * `ISubject`
   * `IObservable`
   * `IObserver`
* Rx wrappers for Akka `EventBus`
   * These will require delegates to perform actual subscribe / unsubscribe / publish
   * Might also create standard wrapper (with default delegate implementations) for well-known `EventBus` patterns (such as channel / sub-channel classifiers).

Example usage for actor-to-`ISubject`:

```csharp
class ReverseActor
    : ReceiveActor
{
    public ReverseActor()
    {
        Receive<string>(message =>
        {
            char[] buffer = message.ToCharArray();
            Array.Reverse(buffer);
    
            Sender.Tell(
                new String(buffer)
            );
        });
    }
    
    protected override void PostStop()
    {
        Console.WriteLine("ReverseActor stopped.");
    }
}

IActorRef reverseActor = system.ActorOf(
    Props.Create<ReverseActor>()
);

ISubject<string> reverseSubject = await system.Reactive().CreateSubjectAsync<string>(reverseActor);

// Log to console
reverseSubject.Subscribe(
    reversed => Console.WriteLine(reversed),
    () => Console.WriteLine("Done.");
);

reverseSubject.OnNext("Hello");
reverseSubject.OnNext("World");
reverseSubject.OnCompleted();
```

Which should produce output:

```
olleH
dlroW
ReverseActor stopped.
Done.
```
