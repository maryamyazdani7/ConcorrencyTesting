using System;
using System.Threading.Tasks;

namespace Concurrency
{
    class Program
    {
        static void Main(string[] args)
        {
            Deadlock deadlock = new Deadlock();
            deadlock.DeadlockMethod();
            Console.WriteLine("Hello World!");
        }
      
    }
}
