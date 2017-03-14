using System;
using Jareel;


[StateContainer("testParent")]
class TestState1 : State
{
    [StateData("boolField")] public bool BoolField { get; set; }

    public TestState1()
    {
        BoolField = true;
    }
}


class TestController1 : StateController<TestState1>
{
    [EventListener("test")] public void DoSomething()
    {

    }

    public override TestState1 CloneState()
    {
        return new TestState1() {
            BoolField = State.BoolField
        };
    }
}