using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VeisTest
{
    class Program
    {
        private static Socket _externalProcessor;
        private static Thread _oThread;
        private static readonly UTF8Encoding Encoding = new UTF8Encoding();

        static void Main(string[] args)
        {
            _externalProcessor = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Console.WriteLine("Trying to connect to workflow provider");
            try
            {
                _externalProcessor.Connect("127.0.0.1", 4444);
                Console.WriteLine("Connected");
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not connect to YAWL Workflow service: " + e.Message);
            }

            _oThread = new Thread(ReadMessages);
            _oThread.Start();

            while (true)
            {
                ReadInput();
            }
        }

        private static void ReadMessages()
        {
            char[] sep = { ' ' };
            while (true)
            {
                byte[] bytes = new byte[16348];

                while (_externalProcessor.Available == 0)
                {
                    Thread.Sleep(16);
                }
                if (_externalProcessor.Available > 0)
                {
                    _externalProcessor.Receive(bytes);
                    //Console.WriteLine(Encoding.GetString(bytes).Replace(" ", ""));
                    //string[] strings = Encoding.GetString(bytes).Split('\n');
                    //foreach (string xi in strings)
                    //{
                    //    string x = xi.Replace("/r", "");
                    //    String action = x.Split(' ')[0];
                    //    int totalParams = x.Split(' ').Length;
                    //    Console.Write(x.Replace(" ", ""));

                    //}

                    Console.Write(Encoding.GetString(bytes).Split('\n')[0] + "\n");
                }
            }
        }

        private static void ReadInput()
        {
            string input = Console.ReadLine();
            if (input.Length > 1)
            {
                Send(input);
                if (input == "exit")
                {
                    Terminate();
                }
            }
            
        }

        private static void Send(string text)
        {
            byte[] bytes = Encoding.GetBytes(text + "\n");
            _externalProcessor.Send(bytes);
        }

        static void Terminate()
        {
            
            _externalProcessor.Shutdown(SocketShutdown.Both);
            _externalProcessor.Disconnect(false);
            Process.GetCurrentProcess().Kill();
        }
    }
}
