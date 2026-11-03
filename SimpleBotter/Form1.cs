using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleBotter
{
    public partial class Form1 : Form
    {
        private TextBox txtAddress;
        private NumericUpDown numBots;
        private CheckBox chkUseProxy;
        private TextBox txtProxyList;
        private ComboBox cmbVersion;
        private Button btnStart;
        private Button btnStop;
        private List<BotClient> bots = new List<BotClient>();
        private CancellationTokenSource? cts;

        public Form1()
        {
            InitializeComponent();
            cmbVersion.Items.AddRange(new[] { "1.16.5", "1.17", "1.18", "1.19", "1.20" });
            cmbVersion.SelectedIndex = 0;
        }

        private void InitializeComponent()
        {
            this.txtAddress = new TextBox() { Location = new Point(12, 12), Size = new Size(150, 23), Text = "example.org" };
            this.numBots = new NumericUpDown() { Location = new Point(170, 12), Minimum = 1, Maximum = 5000, Value = 1030, Size = new Size(70, 23) };
            this.chkUseProxy = new CheckBox() { Location = new Point(250, 14), Text = "Use proxy", Size = new Size(80, 20) };
            this.txtProxyList = new TextBox() { Location = new Point(12, 45), Size = new Size(200, 60), Multiline = true, ScrollBars = ScrollBars.Vertical };
            this.cmbVersion = new ComboBox() { Location = new Point(220, 45), Size = new Size(80, 23) };
            this.btnStart = new Button() { Location = new Point(12, 120), Text = "Start", BackColor = Color.LightGreen, Size = new Size(75, 30) };
            this.btnStop = new Button() { Location = new Point(95, 120), Text = "Stop", BackColor = Color.LightCoral, Size = new Size(75, 30) };
            
            btnStart.Click += BtnStart_Click;
            btnStop.Click += BtnStop_Click;
            
            this.Controls.Add(txtAddress);
            this.Controls.Add(numBots);
            this.Controls.Add(chkUseProxy);
            this.Controls.Add(txtProxyList);
            this.Controls.Add(cmbVersion);
            this.Controls.Add(btnStart);
            this.Controls.Add(btnStop);
            this.Text = "SimpleBotter";
            this.Size = new Size(350, 200);
        }

        private async void BtnStart_Click(object? sender, EventArgs e)
        {
            if (cts != null) return;
            cts = new CancellationTokenSource();
            int botCount = (int)numBots.Value;
            string server = txtAddress.Text;
            string version = cmbVersion.SelectedItem?.ToString() ?? "1.16.5";
            List<IPEndPoint>? proxies = null;
            
            if (chkUseProxy.Checked && !string.IsNullOrWhiteSpace(txtProxyList.Text))
            {
                proxies = new List<IPEndPoint>();
                foreach (var line in txtProxyList.Lines)
                {
                    var parts = line.Trim().Split(':');
                    if (parts.Length == 2 && IPAddress.TryParse(parts[0], out IPAddress ip) && int.TryParse(parts[1], out int port))
                        proxies.Add(new IPEndPoint(ip, port));
                }
            }

            bots.Clear();
            var tasks = new List<Task>();
            for (int i = 0; i < botCount; i++)
            {
                var proxy = proxies != null && proxies.Count > 0 ? proxies[i % proxies.Count] : null;
                var bot = new BotClient(server, 25565, version, proxy);
                bots.Add(bot);
                tasks.Add(bot.ConnectAsync(cts.Token));
                await Task.Delay(10);
            }
        }

        private void BtnStop_Click(object? sender, EventArgs e)
        {
            cts?.Cancel();
            foreach (var bot in bots) bot.Disconnect();
            cts = null;
        }
    }
}