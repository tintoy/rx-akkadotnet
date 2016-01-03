# rx-akkadotnet
Reactive Extensions (Rx) integration for Akka.NET

Functionality includes:
* Wrap an `IActorRef` in an `IObserver`
  * Uses the actor's designated scheduler to deliver messages to it
* Subject actor (and wrapper `ISubject` implementation for communicating with it)
  * Messages sent to `IObserver::OnNext` are sent to the actor to be published via `IObservable::Subscribe`
  * Currently unsure of the best way to surface errors sent to `IObservable::OnError` (may wind up simply forwarding them to be published via `IObservable::Subscribe`)
  * Subject actor terminates once `IObservable::OnCompleted` is called
  * Subject's outgoing `OnCompleted` will not be called until the subject actor itself has terminated.
* `IObservable` wrapper for Akka `EventBus`

Example usage for actor-to-`ISubject`:

```csharp
class ReverseActor
 : ReceiveActor
{
 public ReverseActor()
 {
  Receive<string>(message => {
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

ISubject<string, string> reverseSubject = reverseActor.ToSubject<string, string>();

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
dlroW
olleH
ReverseActor stopped.
Done.
```
