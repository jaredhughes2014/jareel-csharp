
## Unidirectional data flow model for C#
Jareel is a data-flow model for C# applications that features
high automation and a unique internalized event system. Jareel requires
minimal boilerplate, as connection to most components occurs automatically.

Jareel also allows one-call imports/exports of the application state into
JSON strings. Other data formats may be introduced later

### Getting Started
Implementing a basic Jareel system can be done using three classes. Once your
system is complete, you will need two more components to make sure your system
updates and to give you access to your State objects.


#### State
The State object. Jareel systems are best implemented when everything there is
to know about your application is contained in one or more State objects. These
objects must inherit from State class and must have a StateContainer attribute
on the State.

States can contain properties for any primitive value (including strings), any
data type that inherits from StateObject, or a generic List containing any of
those types (nested Lists not yet supported)

All properties used for state must have a StateData attribute. This attribute allows
you to specify the name of the data when exported to JSON. Another option is to specify
whether the data is persistent. If you provide true for that option, the name of the property
will be used as the export name as well. If no argument is provided or false is provided,
the property will still be usable in the state, but it will not be exported when using
a standard export statement.

It's important to note that all State must be marked by a StateData attribute, even if
you have no plans to export that data.
```
[StateContainer("<name>")]
public class State1 : State
{
    [StateData(true)] public string Data1 { get; set; }

    [StateData] public List<int> Data1 { get; set; }
}
```

#### StateController
State controllers are used to control every modification that can be performed on a
state object and to define how to copy a state object.

StateControllers may define methods to use as event listeners. The event system is explained
further below, but any compile-time constant (ints, strings, enums, etc.) may be used as events.
Event listeners are defined using the EventListener attribute, where the argument is the value
of the event that will execute the method. EventListeners may be private methods and no other
hookup is required.

StateControllers must define a CloneState function. This clone should be a <b><u>deep</u></b> copy.
Failing to make the copy a deep copy may result in unexepcted changes to your state.

The following example shows how the above State1 might be controller. All StateControllers have access
to the state object they control via the State property.
```
public class State1Controller : StateController<State1>
{
    [EventListener("a_string")]
    private void HandleEvent()
    {
        State.Data1 = "I'm changed now";
    }

    [EventListener(0)]
    private void HandleInteger()
    {
        State.Data2.Add(1);
    }

    public State1 CloneState()
    {
        return new State1() {
            Data1 = State.Data1,
            Data2 = State.Data2.Select(p => p).ToList()
        };
    }
}
```

#### MasterController
The master controller is the only source of boilerplate in a Jareel project. As you define
your States and StateControllers, you must make sure you `Use()` those values in the
master controller to integrate them into your system.

The MasterController is an abstract class with one abstract method: `UseControllers()`.
This is where you may execute your `Use()` calls to include the States you define and the
controllers that handle them.

Note that you may only have one controller per type of State.

```
public class YourMasterController : MasterController()
{
    protected override void UseControllers()
    {
        Use<State1, State1Controller>(); //State type first, Controller second
        Use<State1, AnotherState1Controller>(); //This will throw a runtime error
    }
}
```

#### Executors
Now that you have your first state, you will need a way to cause changes to your state to register.
These changesare registered via an Executor.

There are two kinds of Executors: Sequential and Asynchronous (Not yet supported). Each executor
operates essentially the same way, but asynchronous allow your state changes to operate on another
thread. More details will come for the asynchronous when it is supported.

To create an executor, simply construct one using an instance of your defined MasterController
```
var yourMaster = new YourMasterController();
SequentialExecutor executor = new SequentialExecutor(yourMaster);
```

Once you have your executor, simply call `Execute()` each time you want to
check for changes in your state.
```
executor.Execute();
```

State Adapters and Events (both explained below) will not take effect until the executor calls
`Execute()`.

Currently, only a sequential Executor is supported. Asynchronous executors are on the agenda to
allow your MasterController to operate on a separate thread.

#### State Subscriber
After you've implemented your states and controllers, you will need a way to access the state
you defined. StateSubscribers are simple objects which  can be spawned from a `MasterController`
that allow you to access your State.

```
StateSubscriber<MyState> subscriber = myMaster.SubscribeToState<MyState>();
```

Every time your State is updated via events or state adaptation (both explained below) and your
state is updated via an executor, all StateSubscribers are updated with a copy of the subscribed
states as defined in that state's controller(if implemented correctly this will be a deep copy).

StateSubscribers have an `Updated` property that is true every time the subscribed state has changed
and is false after the state has been accessed until it is updated again.

```
// This will be false if the state has not updated
if (subscriber.Updated) {
    Console.WriteLine("Should not print");
}
myMaster.Events.Execute("event_that_will_update_my_state");

//This will be true after something causes your state to change
if (subscriber.Updated) {
    Console.WriteLine("Should print");
}
```

StateSubscribers may subscribe to up to four states.

```
var sub1 = aMaster.SubscribeToState<State1>();
var sub2 = aMaster.SubscribeToState<State1, State2>();
var sub3 = aMaster.SubscribeToState<State1, State2, State3>();
var sub4 = aMaster.SubscribeToState<State1, State2, State3, State4>();
```

Remember to unsubscribe your subscribers when you are finished with them.

```
myMaster.DisconnectSubscriber(aSubscriber);
```

### Changing your application state
Once you've defined the state of your application, how your executor will run, and
your subscribers, you will need a way to change your state.

# TODO KEEP ON WRITING BOYZ
