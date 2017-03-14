using System;
using Jareel;

class TestMaster : MasterController
{
    protected override void UseControllers()
    {
        Use<TestState1, TestController1>();
        Use<TestState2, TestController2>();
    }
}
