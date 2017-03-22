using System;
using System.Collections.Generic;
using Jareel;

class TestContainer : StateObject
{
	[StateData("arg1")] public string Arg1 { get; set; }

	public TestContainer()
	{
		Arg1 = "Hello, World!";
	}
}

[StateContainer("testParent")]
class TestState1 : State
{
    [StateData("boolField")] public bool BoolField { get; set; }

	[StateData("testList")] public List<TestContainer> List { get; set; }

    public TestState1()
    {
        BoolField = true;
		List = new List<TestContainer>();
		List.Add(new TestContainer());
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