using System.Net;
using System.Net.Sockets;
using System.Media;

namespace Server
{
    public partial class Form1 : Form
    {
        const int PORT = 8088;
        const string IP = "127.0.0.1";

        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(IP), PORT);
        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public Form1()
        {
            InitializeComponent();
            AddFileToList();
            ReciveData();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null) 
            {              
                SoundPlayer simpleSound = new SoundPlayer(listBox1.SelectedItem.ToString());
                simpleSound.Play();
            }
        }
        //private void Listen()
        //{
        //    Task.Run(() =>
        //    {
        //        serverSocket.Bind(endPoint);
        //        serverSocket.Listen(10);
        //        while (true)
        //        {
        //            ReciveData(serverSocket.Accept());
        //        }
        //    });
        //}
        private void ReciveData()
        {
            Task.Run(() =>
            {
                try
                {
                    serverSocket.Bind(endPoint);
                    serverSocket.Listen(10);
                    Socket clientSocket = serverSocket.Accept();
                    int bytes = 0;
                    byte[] buffer = new byte[101024];
                    do
                    {
                        bytes = clientSocket.Receive(buffer);
                    } while (clientSocket.Available > 0);
                    File.WriteAllBytes($"{DateTime.Now.ToString().Replace(".", "_").Replace(":", "_")}.wav", buffer);
                    clientSocket.Close();
                    serverSocket.Shutdown(SocketShutdown.Both);
                    serverSocket.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
        }

        //private void AddToList()
        //{
        //    DirectoryInfo dir = new(@"E:\PROGRAMMING\MyProjects\ClientServerSocket\ServerSocket\bin\Debug\net6.0");
        //    FileInfo[] files = dir.GetFiles("*.wav");
        //    for (int i = 0; i < files.LongLength; i++)
        //    {
        //        foreach (FileInfo fi in files)
        //        {
        //            listBox1.Items.Add(fi.ToString());
        //            Invalidate();
        //        }
        //    }
        //}

        private void fileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            string meassage = $"файл добавлен \n{DateTime.Now.ToString().Replace(".", ":").Replace(":", ":")}";
            listBox1.Items.Add(e.Name);
            listBox2.Items.Add(meassage);
        }

        private void AddFileToList()
        {
            fileWatcher.Path = Directory.GetCurrentDirectory();
            fileWatcher.NotifyFilter = NotifyFilters.Attributes
                | NotifyFilters.CreationTime
                | NotifyFilters.FileName
                | NotifyFilters.DirectoryName
                | NotifyFilters.LastWrite
                | NotifyFilters.LastAccess
                | NotifyFilters.Size
                | NotifyFilters.Security;
            fileWatcher.Filter = "*.wav";
            fileWatcher.IncludeSubdirectories = true;
            fileWatcher.EnableRaisingEvents = true;

            foreach (var item in Directory.GetFiles(Directory.GetCurrentDirectory()))
            {
                if (Path.GetExtension(item).Equals(".wav"))
                {
                    listBox1.Items.Add(Path.GetFileName(item).Replace("\\", "/"));
                }
            }
        }
    }
}