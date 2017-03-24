
using System;
using Jareel;

public class TestMaster : MasterController
{
	protected override void UseControllers()
	{
		Use<TestState, TestController>();
	}
}