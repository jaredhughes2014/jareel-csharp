using System;
using Jareel;

class Program
{
	static void Main(string[] args)
	{
		var master = new TestMaster();
		var go = new SequentialExecutor(master);

		master.Events.Execute(TestEvents.MeFirst);
		master.Events.Execute(TestEvents.MeSecond);
		master.Events.Execute(TestEvents.MeThird);

		go.Execute();

		master.Events.Execute(TestEvents.MeSecond);
		master.Events.Execute(TestEvents.MeFirst);
		master.Events.Execute(TestEvents.MeThird);

		go.Execute();

		master.Events.Execute(TestEvents.MeThird);
		master.Events.Execute(TestEvents.MeSecond);
		master.Events.Execute(TestEvents.MeFirst);

		go.Execute();
	}
}
