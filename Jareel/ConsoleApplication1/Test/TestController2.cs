using System;
using Jareel;


[StateContainer("testChild")]
class TestState2 : State
{
    [StateData("stringField")]
    public string StringField { get; set; }

    public TestState2()
    {
        StringField = "Not Set";
    }
}


class TestController2 : StateController<TestState2>
{
    [StateAdapter] private void OneToTwo(TestState1 one)
    {
        State.StringField = one.BoolField ? "Success" : "Failure";
    }

    public override TestState2 CloneState()
    {
        return new TestState2() {
            StringField = State.StringField
        };
    }
}