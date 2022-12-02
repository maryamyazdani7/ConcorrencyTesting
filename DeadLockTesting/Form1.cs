using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DeadLockTesting
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            //Deadlock();
            //Deadlock_FirstSolution().GetAwaiter();
            Deadlock_SecondSolution();
        }
        async Task WaitAsync()
        {
            PrintData("call waitAsync \n");
            // This await will capture the current context ...
            await Task.Delay(TimeSpan.FromSeconds(1));
            // ... and will attempt to resume the method here in that context.
            PrintData("end call waitAsync \n");
        }
        void Deadlock()
        {

            PrintData("create task waitAsync \n");
            // Start the delay.
            Task task = WaitAsync();
            // Synchronously block, waiting for the async method to complete.

            PrintData("wait for task waitAsync \n");
            task.Wait();
            PrintData("end \n");
        }
        public async Task WaitAsync_FirstSolution()
        {
            PrintData("call waitAsync \n");
            // This await will capture the current context ...
            await Task.Delay(TimeSpan.FromSeconds(1));
            // ... and will attempt to resume the method here in that context.
            //   Trace.WriteLine("end call waitAsync \n");
            PrintData("end call waitAsync \n");
        }
        public async Task Deadlock_FirstSolution()
        {

            PrintData("wait for task waitAsync \n");
            // Start the delay.
            await WaitAsync_FirstSolution();
            // Synchronously block, waiting for the async method to complete.

            PrintData("end \n");
        }

        public async Task WaitAsync_SecondSolution()
        {
            PrintData("call waitAsync \n");
            // This await will capture the current context ...
           // await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
            await Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < 10000; i++)
                {
                    //ignore
                }
                Trace.WriteLine("end loop \n");
            },CancellationToken.None,
TaskCreationOptions.None,TaskScheduler.Current).ConfigureAwait(false);

            // ... and will attempt to resume the method here in that context.
            PrintData("end call waitAsync \n");
        }
        public void Deadlock_SecondSolution()
        {

            PrintData("create task waitAsync \n");
            // Start the delay.
            Task task = WaitAsync_SecondSolution();
            // Synchronously block, waiting for the async method to complete.
            PrintData("wait for task waitAsync \n");
            task.Wait();

            PrintData("end \n");
        }

        public void PrintData(string value)
        {
//            Delegate.Invoke: Executes synchronously, on the same thread.
//Delegate.BeginInvoke: Executes asynchronously, on a threadpool thread.
//Control.Invoke: Executes on the UI thread, but calling thread waits for completion before continuing.
//Control.BeginInvoke: Executes on the UI thread, and calling thread doesn't wait for completion.


            try
            {
                if (richTextBox1.InvokeRequired)
                {
                    string m = $"{DateTime.Now.ToString("H:mm:ss.fffff")}\t{value}\n";
                    richTextBox1.BeginInvoke((MethodInvoker)delegate ()
                    {
                        richTextBox1.AppendText(m);
                        richTextBox1.ScrollToCaret();
                    });
                }
                else
                {
                    richTextBox1.AppendText(value + Environment.NewLine);
                }

            }
            catch(Exception ex)
            {

            }         

        }


    }

    
}
