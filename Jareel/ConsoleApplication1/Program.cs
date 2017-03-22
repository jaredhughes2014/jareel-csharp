using System;
using Jareel;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var master = new TestMaster();
            var go = new SequentialExecutor(master);

            master.Events.Execute("test");
            go.Execute();
			string state = master.ExportStates();
            Console.WriteLine(state);

			master.Events.Execute("reset");
			go.Execute();
			master.ImportState(state);
			state = master.ExportStates();
			Console.WriteLine(state);
        }
    }
}
