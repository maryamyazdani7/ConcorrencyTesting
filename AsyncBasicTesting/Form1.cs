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
                    string m = $"result\t{value}\n";
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

    }
}
