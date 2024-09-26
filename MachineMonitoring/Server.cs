using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using Newtonsoft.Json;
using System.Net.NetworkInformation;

namespace MachineMonitoring
{
    class Server
    {
        Form1 value = new Form1();
        private Socket serverSocket;
        private bool isRunning = true;
        private string ipAddress;
        private int port;

        private Form1 _form1;
        public Server(Form1 form1)
        {
            _form1 = form1;
            this.ipAddress = GetLocalIPAddress2();
            this.port = 2323;
        }

        public void Start()
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(IPAddress.Parse(ipAddress), port));
            serverSocket.Listen(10);

            Console.WriteLine("Server started at "+ ipAddress +":"+ port);

            while (isRunning)
            {
                var clientSocket = serverSocket.Accept();
                ThreadPool.QueueUserWorkItem(HandleClient, clientSocket);
            }
        }

        public void HandleClient(object client)
        {
            var clientSocket = (Socket)client;
            byte[] buffer = new byte[1024];
            int receivedBytes = clientSocket.Receive(buffer);
            string request = Encoding.ASCII.GetString(buffer, 0, receivedBytes);

            var data = new object[]
            {
                new { SG3Wheelcoolant = _form1.SG3WheelCoolant1},
                new { SG3chiller = _form1.SG3Chiller1},
                new { SG5Wheelcoolant = _form1.SG5WheelCoolant1},
                new { SG5chiller = _form1.SG5Chiller1},
                new { SG6Wheelcoolant = _form1.SG6WheelCoolant1},
                new { SG6chiller = _form1.SG6Chiller1},
                new { SP3Turbopump1 = _form1.SP3TurboPump11},
                new { SP3Turbopump2 = _form1.SP3TurboPump21},
                new { SP3Turbopump3 = _form1.SP3TurboPump31},
                new { SP3top = _form1.SP3Top1},
                new { SP3bottom = _form1.SP3Bottom1},
                new { SP3Noise = _form1.SP3Noise1}
            };

            string responseData = JsonConvert.SerializeObject(data);
            byte[] responseBytes = Encoding.UTF8.GetBytes(
                "HTTP/1.1 200 OK\r\n" +
                "Content-Type: application/json\r\n" +
                "Content-Length: {responseBytes.Length}\r\n" +
                "\r\n" + 
                responseData);

            clientSocket.Send(responseBytes);
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }

        public void Stop()
        {
            isRunning = false;
            serverSocket.Close();
        }

        private string GetLocalIPAddress()
        {
            string ipAddress = "";
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                // Prioritaskan hanya Wi-Fi network interfaces
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 && ni.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip.Address))
                        {
                            ipAddress = ip.Address.ToString();
                            return ipAddress; // Mengembalikan IP address dari Wi-Fi
                        }
                    }
                }
            }

            // Jika tidak ada Wi-Fi, bisa mencoba Ethernet atau mengembalikan IP kosong
            return ipAddress;
        }

        // Method untuk mendapatkan IP address secara otomatis
        private string GetLocalIPAddress2()
        {
            string ipAddress = "";
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if ((ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet) && ni.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip.Address))
                        {
                            ipAddress = ip.Address.ToString();
                            break;
                        }
                    }
                }
            }

            return ipAddress;
        }
    }
}
