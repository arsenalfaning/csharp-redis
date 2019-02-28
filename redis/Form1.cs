using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using StackExchange.Redis;

namespace redis
{
    public partial class Form1 : Form
    {
        private ConnectionMultiplexer redis;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ConfigurationOptions config = new ConfigurationOptions
            {
                EndPoints =
                {
                    { "192.168.150.128", 6379 },
                    { "192.168.150.133", 6379 },
                    { "192.168.150.134", 6379 }
                },
                CommandMap = CommandMap.Create(new HashSet<string>
                { // EXCLUDE a few commands
                    "CONFIG", "CLUSTER","ECHO", "CLIENT"
                }, available: false),
                KeepAlive = 60,
                DefaultDatabase = 0,
                ClientName = "CSharp"
            };
            config.ReconnectRetryPolicy = new LinearRetry(5000);
            redis = ConnectionMultiplexer.Connect(config);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(redis.GetStatus());
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            redis.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string[] strings = textBox1.Text.ToString().Split(' ');
            object[] ps = new object[strings.Length - 1];
            for (int i = 1; i < strings.Length; i ++)
            {
                ps[i - 1] = strings[i];
            }
            RedisResult redisResult = redis.GetDatabase().Execute(strings[0], ps);
            textBox2.Clear();
            textBox2.AppendText(redisResult.IsNull ? "nil" : redisResult.ToString());
        }
    }
}
