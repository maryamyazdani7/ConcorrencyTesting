using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReactiveProgrammingTesting
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            IObservable<DateTimeOffset> timestamps =
 Observable.Interval(TimeSpan.FromSeconds(1))
 .Timestamp()
 .Where(x => x.Value % 2 == 0)
 .Select(x => x.Timestamp);
            timestamps.Subscribe(x => PrintData(x.ToString()));
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

        private void RichTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }

}
