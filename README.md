# rx-akkadotnet
Reactive Extensions (Rx) integration for Akka.NET

Functionality includes:
* Wrap an `IActorRef` in an `IObserver`
   * Uses the actor's designated scheduler to deliver messages to it
   * Calling `IObserver::OnError` will deliver a special messsage to the actor containing the error.
      * Will consider making this behaviour customisable, depending on use case
         * E.g. Stop actor, somehow cause it to fault, or even specify a supervision policy.
   * Calling `IObserver::OnCompleted` will by default send the actor a `PoisonPill` message (but this will be configurable)
* Subject actor (and wrapper `ISubject` implementation for communicating with it)
   * Messages sent to `IObserver::OnNext` are sent to the actor to be published via `IObservable::Subscribe`
   * Currently unsure of the best way to surface errors sent to `IObservable::OnError` (may wind up simply forwarding them to be published via `IObservable::Subscribe`)
   * Subject actor terminates once `IObservable::OnCompleted` is called
   * Subject's outgoing `OnCompleted` will not be called until the subject actor itself has terminated.
* Rx wrappers for for Akka `ActorEventBus`
   * `ISubject`
   * `IObservable`
   * `IObserver`
* Rx wrappers for Akka `EventBus`
   * These will require delegates to perform actual subscribe / unsubscribe / publish
   * Might also create standard wrapper (with defauly delegate implementations) for well-known `EventBus` patterns (such as channel / sub-channel classifiers).

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
