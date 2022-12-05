using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AsyncBasicTesting
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void Button1_ClickAsync(object sender, EventArgs e)
        {
            var client = new HttpClient();
            try
            {
               // var resultWithRetries = await DownloadStringWithRetries(client, "https://wwwogle.com");
                var resultWithTimeOut = await DownloadStringWithTimeout(client, "https://www.google.com");
                if (resultWithTimeOut == null)
                {
                    PrintData("Failed");
                }
                else
                {
                    PrintData("result:" + resultWithTimeOut);
                }
            }
            catch
            {

                PrintData("Failed");
            }
        }

        async Task<string> DownloadStringWithRetries(HttpClient client, string uri)
        {
            // Retry after 1 second, then after 2 seconds, then 4.
            TimeSpan nextDelay = TimeSpan.FromSeconds(1);
            for (int i = 0; i != 3; ++i)
            {
                try
                {
                    return await client.GetStringAsync(uri);
                }
                catch
                {
                }
                PrintData("Try" + nextDelay.ToString());
                await Task.Delay(nextDelay);
                nextDelay = nextDelay + nextDelay;
            }
            // Try one last time, allowing the error to propagate.
            return await client.GetStringAsync(uri);
        }

        async Task<string> DownloadStringWithTimeout(HttpClient client, string uri)
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            Task<string> downloadTask = client.GetStringAsync(uri);
            Task timeoutTask = Task.Delay(Timeout.InfiniteTimeSpan, cts.Token);
            Task completedTask = await Task.WhenAny(downloadTask, timeoutTask);
            if (completedTask == timeoutTask)
                return null;
            return await downloadTask;
        }

        public void PrintData(string value)
        {
            try
            {
                if (richTextBox1.InvokeRequired)
                {
                    string m = $"invoke result \t{value}\n";
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
            catch (Exception ex)
            {

            }

        }

        private async void Button2_Click(object sender, EventArgs e)
        {
            PrintData("ProcessTasksAsync");
          await  ProcessTasksAsync();

            PrintData(@"First solution - a higher-level async method that handles awaiting the task and processing its result:");
            await ProcessTasksAsync_FirstSoution();
            PrintData(@"Second solution - do the task processing concurrently:");
            await ProcessTasksAsync_SecondSolution();

            PrintData(@"SeThirscond solution -Nito.AsyncEx- OrderByCompletion :");
            await UseOrderByCompletionAsync();
        }
        async Task<int> DelayAndReturnAsync(int value)
        {
            await Task.Delay(TimeSpan.FromSeconds(value));
            return value;
        }
        // Currently, this method prints "2", "3", and "1".
        // The desired behavior is for this method to print "1", "2", and "3".
        async Task ProcessTasksAsync()
        {
            // Create a sequence of tasks.
            Task<int> taskA = DelayAndReturnAsync(2);
            Task<int> taskB = DelayAndReturnAsync(3);
            Task<int> taskC = DelayAndReturnAsync(1);
            Task<int>[] tasks = new[] { taskA, taskB, taskC };
            // Await each task in order.
            foreach (Task<int> task in tasks)
            {
                var result = await task;
                Trace.WriteLine(result);
                PrintData(result.ToString());
            }
        }

        async Task AwaitAndProcessAsync(Task<int> task)
        {
            int result = await task;
            Trace.WriteLine(result);
            PrintData(result.ToString());
        }
        // This method now prints "1", "2", and "3".
        async Task ProcessTasksAsync_FirstSoution()
        {
            // Create a sequence of tasks.
            Task<int> taskA = DelayAndReturnAsync(2);
            Task<int> taskB = DelayAndReturnAsync(3);
            Task<int> taskC = DelayAndReturnAsync(1);
            Task<int>[] tasks = new[] { taskA, taskB, taskC };
            IEnumerable<Task> taskQuery =
            from t in tasks select AwaitAndProcessAsync(t);
            Task[] processingTasks = taskQuery.ToArray();
            // Await all processing to complete
            await Task.WhenAll(processingTasks);
        }

        // This method now prints "1", "2", and "3".
        async Task ProcessTasksAsync_SecondSolution()
        {
            // Create a sequence of tasks.
            Task<int> taskA = DelayAndReturnAsync(2);
            Task<int> taskB = DelayAndReturnAsync(3);
            Task<int> taskC = DelayAndReturnAsync(1);
            Task<int>[] tasks = new[] { taskA, taskB, taskC };
            Task[] processingTasks = tasks.Select(async t =>
            {
                var result = await t;
                Trace.WriteLine(result);
                PrintData(result.ToString());
            }).ToArray();
            // Await all processing to complete
            await Task.WhenAll(processingTasks);
        }

        async Task UseOrderByCompletionAsync()
        {
            // Create a sequence of tasks.
            Task<int> taskA = DelayAndReturnAsync(2);
            Task<int> taskB = DelayAndReturnAsync(3);
            Task<int> taskC = DelayAndReturnAsync(1);
            Task<int>[] tasks = new[] { taskA, taskB, taskC };
            // Await each one as they complete.
            foreach (Task<int> task in tasks.OrderByCompletion())
            {
                int result = await task;
                Trace.WriteLine(result);
                PrintData(result.ToString());
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            LINQAsynchronousStreams();
        }
        public async Task LINQAsynchronousStreams()
        {
            IAsyncEnumerable<int> values = SlowRange().WhereAwait(
    async value =>
    {
    // Do some asynchronous work to determine
    // if this element should be included.
    await Task.Delay(10);
        return value % 2 == 0;
    });
            await foreach (int result in values)
            {
                PrintData(result.ToString());
            }
        }
    // Produce sequence that slows down as it progresses.
    async IAsyncEnumerable<int> SlowRange()
    {
        for (int i = 0; i != 10; ++i)
        {
            await Task.Delay(i * 100);
            yield return i;
        }
    }

        private void Button4_Click(object sender, EventArgs e)
        {
            double[] array = { 1.1, 1.2, 1.3, 1.4, 1.5, 1.6, 1.7, 1.8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30 };
            ProcessArray(array);
        }
        void ProcessArray(double[] array)
        {
            Parallel.Invoke(
            () => ProcessPartialArray(array, 0, array.Length / 2),
            () => ProcessPartialArray(array, array.Length / 2, array.Length)
            );
        }
        void ProcessPartialArray(double[] array, int begin, int end)
        {
            // CPU-intensive processing...
            for (var i = begin; i < end; i++)
                PrintData(array[i].ToString());
        }
    }
}
