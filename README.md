# rx-akkadotnet
Reactive Extensions (Rx) integration for Akka.NET

Functionality includes:
* Wrap an `IActorRef` in an `IObserver`
  * Uses the actor's designated scheduler to deliver messages to it
* `ISubject` actor / base class
  * Messages sent to `IObserver::OnNext` are sent to the actor to be published via `IObservable::Subscribe`
  * Currently unsure of the best way to surface errors sent to `IObservable::OnError` (may wind up simply forwarding them to be published via `IObservable::Subscribe`)
  * Subject actor terminates once `IObservable::OnCompleted` is called
* `IObservable` wrapper for Akka `EventBus`
