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

            Events.Execute("test");
            go.Execute();
            Console.WriteLine(master.ExportStates());
        }
    }
}
