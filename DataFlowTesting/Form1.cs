using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows.Forms;

namespace DataFlowTesting
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            ttAsync().GetAwaiter();

            PrintData("DONE");
        }
        private async Task ttAsync()
        {
            try
            {
                var multiplyBlock = new TransformBlock<int, int>(item =>
                {
                    if (item == 1)
                        throw new InvalidOperationException("Blech.");
                    var value = item * 2;
                   PrintData("thid is multiplyBlock value:" + value);
                    return value;
                    
                });
                var subtractBlock = new TransformBlock<int, int>(item => {
                    var value = item - 2;
                    PrintData("thid is subtractBlock value:" + value);
                    return value;
                    });
                IDisposable link = multiplyBlock.LinkTo(subtractBlock,
                new DataflowLinkOptions { PropagateCompletion = true });
                multiplyBlock.Post(1);
                await subtractBlock.Completion;
               
            }
            catch (AggregateException exception)
            {
                AggregateException ex = exception.Flatten();
                Trace.WriteLine(ex.InnerException);

                PrintData("FAILED:" + ex.InnerException);
            }
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
