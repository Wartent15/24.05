using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace _24._05
{

    public class ServerForm : Form
    {
        private Button startButton;
        private TextBox messagesTextBox;
        private TcpListener listener;
        private Thread listenThread;

        public ServerForm()
        {
            startButton = new Button { Text = "Запустить сервер" };
            messagesTextBox = new TextBox { Multiline = true, Width = 400, Height = 300 };

            startButton.Click += StartButton_Click;

            Controls.Add(startButton);
            Controls.Add(messagesTextBox);

            Load += (s, e) => messagesTextBox.AppendText("Сервер запущен.\n");
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            listener = new TcpListener(IPAddress.Any, 8888);
            listener.Start();

            listenThread = new Thread(ListenForClients);
            listenThread.Start();
        }

        private void ListenForClients()
        {
            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Thread clientThread = new Thread(HandleClient);
                clientThread.Start(client);
            }
        }

        private void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[4096];
            int bytesRead;

            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                if (message.Trim() == "Bye")
                {
                    break;
                }
                Invoke(new Action(() => messagesTextBox.AppendText("Клиент: " + message + "\n")));
                // Ответ компьютера — случайная фраза
                string response = "Ответ компьютера: " + GetRandomResponse();
                byte[] responseBytes = Encoding.ASCII.GetBytes(response);
                stream.Write(responseBytes, 0, responseBytes.Length);
            }

            client.Close();
        }

        private string GetRandomResponse()
        {
            string[] responses = { "Привет!", "Как дела?", "Что нового?", "До свидания!" };
            Random rand = new Random();
            return responses[rand.Next(responses.Length)];
        }

        [STAThread]
        static void Main()
        {
            Application.Run(new ServerForm());
        }
    }
}
