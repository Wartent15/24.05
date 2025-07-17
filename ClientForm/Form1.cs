using System;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace ClientForm
{
    public class ClientForm : Form
    {
        private TextBox ipTextBox;
        private TextBox portTextBox;
        private TextBox messagesTextBox;
        private Button connectButton;
        private TcpClient client;
        private NetworkStream stream;

        public ClientForm()
        {
            ipTextBox = new TextBox { Text = "IP адрес", Width = 200 };
            portTextBox = new TextBox { Text = "Порт", Width = 100 };
            messagesTextBox = new TextBox { Multiline = true, Width = 300, Height = 200 };
            connectButton = new Button { Text = "Подключиться" };

            connectButton.Click += ConnectButton_Click;

            Controls.Add(ipTextBox);
            Controls.Add(portTextBox);
            Controls.Add(messagesTextBox);
            Controls.Add(connectButton);
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            client = new TcpClient(ipTextBox.Text, int.Parse(portTextBox.Text));
            stream = client.GetStream();

            // Например, человек вводит сообщение
            string message = Console.ReadLine();
            while (message != "Bye")
            {
                byte[] data = Encoding.ASCII.GetBytes(message);
                stream.Write(data, 0, data.Length);
                messagesTextBox.AppendText("Вы: " + message + "\n");
                // Чтение ответа от сервера
                byte[] buffer = new byte[4096];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                messagesTextBox.AppendText("Сервер: " + response + "\n");
            }

            stream.Close();
            client.Close();
        }
        static void Main()
        {
            Application.Run(new ClientForm());
        }
    }
}
