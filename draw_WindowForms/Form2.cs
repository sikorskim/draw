using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace draw_WindowForms
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            label1.Text = "0/100";
        }

        CancellationTokenSource cs = new CancellationTokenSource();

        void odliczanieStart()
        {
            cs = generujCts();
            zadanie(cs);
        }

        CancellationTokenSource generujCts()
        {
            return new CancellationTokenSource();
        }

        async void zadanie(CancellationTokenSource cts)
        {
            await Task.Factory.StartNew(() =>
            {
                for (int i = 0; i <= 100; i++)
                {
                    Invoke(new Action(() =>
                    {
                        progressBar1.Value = i;
                        label1.Text = i.ToString() + "/100";
                    }));
                    Thread.Sleep(100);

                    if (cts.IsCancellationRequested)
                    {
                        Invoke(new Action(() =>
                        {
                            label1.Text = "zadanie zatrzymane przez użytkownika";
                        }));
                        break;
                    }
                }
                Invoke(new Action(() =>
                {
                    label1.Text = "zadanie zakończone";
                }));
            });
        }

        void zabijProces()
        {
            cs.Cancel();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            odliczanieStart();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            zabijProces();
        }

    }
}
