using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SOCKEY_TCP_HOST;

namespace SOCKEY_TCP_Client
{
    class MainClass
    {
        //----------------------------
        //一些错误或提示预设
        static string port_exit = "正常退出";
        static string port_OnEntering = "加入服务器";
        ///其他预设
        //static string ipCommom = "192.168.31.122";

        //string iphost = mainhostlaunch.ipcommom;
        //int portHost = MainHostLaunch.portCommom;

        /// <summary>
        /// 客户端代码
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        public static void Main(string[] args)
        {

            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(new IPEndPoint(IPAddress.Parse("192.168.31.122"), 8218));
            //clientSocket.Connect(new IPEndPoint(IPAddress.Parse(ipHost), portHost));

            byte[] data = new byte[1024];
            int count = clientSocket.Receive(data);
            string msg = Encoding.UTF8.GetString(data, 0, count);
            Console.WriteLine(msg);
            clientSocket.Send(Encoding.UTF8.GetBytes(port_OnEntering));


            //不断更新数据
            while (true)
            {
                string s = Console.ReadLine();

                if (s == "exit")
                {
                    Console.WriteLine("按下任意键以取消，按下y退出");
                    if ("y" == Console.ReadLine())
                    {
                        clientSocket.Send(Encoding.UTF8.GetBytes(port_exit));
                        clientSocket.Close();
                        return;
                    }
                    else continue;
                }
                else if (s == "nian")//未解决粘包问题的常规方法
                {
                    Console.WriteLine("按下任意键以取消，按下y进行粘包测试");
                    if ("y" == Console.ReadLine())
                    {
                        for (int i = 0; i < 1000; i++)
                        {
                            string ii = "" + i;
                            clientSocket.Send(Encoding.UTF8.GetBytes(ii));
                        }
                        continue;
                    }
                    else continue;
                }
                /// 解决粘包问题方法                    
                /// 新建一个静态类Message，将字符串的长度信息放在了头结点，使粘包问题得以解决
                /// 每次只按找头结点储存的长度信息截取舒服，给避免粘包打了客户端的基础
                else if (s == "nians")
                {
                    Console.WriteLine("按下任意键以取消，按下y进行粘包问题解决测试");
                    if ("y" == Console.ReadLine())
                    {
                        for (int i = 0; i < 1000; i++)
                        {
                            string ii = "" + i;
                            clientSocket.Send(Message.GetBytes(ii.ToString()));
                        }
                        continue;
                    }
                    else continue;
                }
                else if (s == "fen")
                {
                    Console.WriteLine("按下任意键以取消，按下y进行分包测试");
                    if ("y" == Console.ReadLine())
                    {
                        string ii = "";

                        for (int i = 0; i < 1000; i++)
                        {
                            ii += i;
                        }
                        clientSocket.Send(Encoding.UTF8.GetBytes(ii));
                        continue;
                    }
                    else continue;
                }
                else
                {
                    Console.WriteLine(s);
                    Console.WriteLine("\n");
                    //clientSocket.Send(Encoding.UTF8.GetBytes(s));
                    clientSocket.Send(Message.GetBytes(s.ToString()));
                }
            }

            Console.ReadKey();
            clientSocket.Close();

        }
    }
}
