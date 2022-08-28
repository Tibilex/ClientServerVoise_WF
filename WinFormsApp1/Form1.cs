using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace WinFormsApp1
{   
    public partial class Form1 : Form
        {
            [DllImport("winmm.dll", EntryPoint = "mciSendString", CharSet = CharSet.Auto)]
            public static extern int mciSendString(
                string lpstrCommand,
                string lpstrReturnString,
                int uReturnLength,
                int hwndCallback
        );
        const int PORT = 8088;
        const string IP = "127.0.0.1";
        IPEndPoint iPEnd = new IPEndPoint(IPAddress.Parse(IP), PORT);
        Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.BackColor = Color.Red;
            mciSendString("open new type WAVEAudio alias recsound", "", 0, 0);
            mciSendString("record recsound", "", 0, 0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.BackColor = Color.Blue;
            mciSendString("stop recsound", "", 0, 0);
            mciSendString($"save recsound temp.wav", "", 0, 0); ;
            mciSendString("close recsound", "", 0, 0);
            this.BackColor = Color.Green;

            Connect();
        }

        private void Connect()
        {
            const string IP = "127.0.0.1";
            IPEndPoint iPEnd = new IPEndPoint(IPAddress.Parse(IP), 8088);
            try
            {
                Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientSocket.Connect(iPEnd);    
                clientSocket.SendFile("temp.wav");
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
                DeleteFile();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DeleteFile()
        {
            foreach (var item in Directory.GetFiles(Directory.GetCurrentDirectory()))
            {
                if (Path.GetExtension(item).Equals(".wav"))
                {
                    File.Delete(Path.GetFileName(item).Replace("\\", "/"));
                }
            }
        }
    }
}
