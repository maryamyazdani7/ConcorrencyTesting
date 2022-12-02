using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Concurrency
{
    class Deadlock
    {
       public  async Task WaitAsync()
        {

            Console.WriteLine("call waitAsync");
            // This await will capture the current context ...
            await Task.Delay(TimeSpan.FromSeconds(1));
            // ... and will attempt to resume the method here in that context.
            Console.WriteLine("end call waitAsync");
        }
       public async void DeadlockMethod()
        {

            Console.WriteLine("create task waitAsync");
            // Start the delay.
            Task task = WaitAsync();
            // Synchronously block, waiting for the async method to complete.

            Console.WriteLine("wait for task waitAsync");
            task.Wait();


            Console.WriteLine("end");
        }
    }
}
