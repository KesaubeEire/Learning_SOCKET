using System;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace SOCKEY_TCP_HOST
{
    class MainClass
    {
        /// <summary>
        /// 服务器端的代码
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        public static void Main(string[] args)
        {
            //StartServerASync_to1();
            StartServerSync_to_a_lot();
            Console.ReadKey();
        }
        /// <summary>
        /// 同 步 方 式
        /// </summary>
        static void StartServerSync()
        {
            //建立socket，ipV4，流形式，TCP协议（其它暂时不懂）
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //绑定ip 局域网 192.168.31.255 是变动的，总是会变的
            //绑定ip 本机 127.0.0.1
            //IpAddress xxx.xx.xx.xx IPENdPOINT xxx.xx.xx.xx:port
            //IPAddress ipAddress = new IPAddress(new byte[]{192.168.31.255}); 老方法，不推荐使用
            IPAddress ip_Address = IPAddress.Parse("192.168.31.122");
            IPEndPoint ip_EndPoint = new IPEndPoint(ip_Address, 8081);

            serverSocket.Bind(ip_EndPoint);//绑定ip和端口号
            serverSocket.Listen(50);//开始监听，同时监听的端口数最大值，0为不限制
            Socket clientSocket = serverSocket.Accept();//接受一个客户端链接知道有一个接入

            //向客户段发送一条消息
            string msg = "Good Luck Guy, Say Something";

            ///发送时需要将信息转化为数组，接受时需要先定义一个数组存入消息

            byte[] data = System.Text.Encoding.UTF8.GetBytes(msg);//使用这种方式将输入的信息转换为字节数组，必须要这么做
            clientSocket.Send(data);//发送消息


            byte[] dataBuffer = new byte[1024];//1024是字符数量大小
            int count = clientSocket.Receive(dataBuffer);//receive函数返回接受数据的长度
            string msgReceive = System.Text.Encoding.UTF8.GetString(dataBuffer, 0, count);
            //这玩意儿返回到底有多少个字符是有意义的，前面的1024是给的固定的空间，一般信息是填不满的，多余的要删掉的吧……
            Console.WriteLine(msgReceive);
            Console.ReadKey();
            clientSocket.Close();//关闭和客户端的链接
            serverSocket.Close();//关闭和自身的链接

        }

        /// <summary>
        /// 异步方式一个客户端
        /// </summary>
        static void StartServerASync_to1()
        {
            //建立socket，ipV4，流形式，TCP协议（其它暂时不懂）
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //绑定ip 局域网 192.168.31.255 是变动的，总是会变的
            //绑定ip 本机 127.0.0.1
            //IpAddress xxx.xx.xx.xx IPENdPOINT xxx.xx.xx.xx:port
            //IPAddress ipAddress = new IPAddress(new byte[]{192.168.31.255}); 老方法，不推荐使用
            IPAddress ip_Address = IPAddress.Parse("192.168.31.122");
            IPEndPoint ip_EndPoint = new IPEndPoint(ip_Address, 8081);

            serverSocket.Bind(ip_EndPoint);//绑定ip和端口号
            serverSocket.Listen(50);//开始监听，同时监听的端口数最大值，0为不限制
            Socket clientSocket = serverSocket.Accept();//接受一个客户端链接知道有一个接入

            //向客户段发送一条消息
            string msg = "Good Luck Guy, Say Something";

            ///发送时需要将信息转化为数组，接受时需要先定义一个数组存入消息

            byte[] data = System.Text.Encoding.UTF8.GetBytes(msg);//使用这种方式将输入的信息转换为字节数组，必须要这么做
            clientSocket.Send(data);//发送消息

            //异步接受数据,very important
            clientSocket.BeginReceive(dataBuffer, 0, 1024, SocketFlags.None, ReceiveCallBack, clientSocket);

        }

        /// <summary>
        /// 异步方式多个客户端
        /// </summary>
        static void StartServerSync_to_a_lot()
        {
            //建立socket，ipV4，流形式，TCP协议（其它暂时不懂）
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //绑定ip 局域网 192.168.31.255 是变动的，总是会变的
            //绑定ip 本机 127.0.0.1
            //IpAddress xxx.xx.xx.xx IPENdPOINT xxx.xx.xx.xx:port
            //IPAddress ipAddress = new IPAddress(new byte[]{192.168.31.255}); 老方法，不推荐使用
            IPAddress ip_Address = IPAddress.Parse("192.168.31.122");
            IPEndPoint ip_EndPoint = new IPEndPoint(ip_Address, 8089);

            serverSocket.Bind(ip_EndPoint);//绑定ip和端口号
            serverSocket.Listen(50);//开始监听，同时监听的端口数最大值，0为不限制

            //异步接受多个客户端的数据
            serverSocket.BeginAccept(AcceptCallBack, serverSocket);
        }

        static byte[] dataBuffer = new byte[1024];//1024是字符数量大小

        static int repeatNumMax = 0;
        /// <summary>
        /// 一个回调函数，负责异步返回消息
        /// </summary>
        /// <param name="ar">Ar.</param>
        static void ReceiveCallBack(IAsyncResult ar)
        {
            Socket clientSocket = null;
            try
            {
                clientSocket = ar.AsyncState as Socket;
                int count = clientSocket.EndReceive(ar);
                //重复连续出现5次这种空数据判断为关闭了客户端
                if (count == 0)//接收到的尽是空数据
                    repeatNumMax++;
                else repeatNumMax = 0;
                if (repeatNumMax >= 5)
                {
                    Console.WriteLine("检测到某客户端断开");
                    clientSocket.Close(); 
                    return;
                }

                string msg = Encoding.UTF8.GetString(dataBuffer, 0, count);
                Console.WriteLine("从客户端异步接收到数据：" + "(" + count + ")" + "\t" + msg);
                clientSocket.BeginReceive(dataBuffer, 0, 1024, SocketFlags.None, ReceiveCallBack, clientSocket);
                //循环回调
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                if (clientSocket != null)
                {
                    clientSocket.Close();
                }
            }
        }

        /// <summary>
        /// 回调函数，负责异步接受信息
        /// </summary>
        /// <param name="ar">Ar.</param>
        static void AcceptCallBack(IAsyncResult ar)
        {
            //将那个异步的接受后结束
            Socket serverSocket = ar.AsyncState as Socket;
            Socket clientSocket = serverSocket.EndAccept(ar);
            //向客户段发送一条消息
            string msg = "Good Luck Guy, Say Something";

            ///发送时需要将信息转化为数组，接受时需要先定义一个数组存入消息

            byte[] data = System.Text.Encoding.UTF8.GetBytes(msg);//使用这种方式将输入的信息转换为字节数组，必须要这么做
            clientSocket.Send(data);//发送消息


            //异步接受数据,very important
            clientSocket.BeginReceive(dataBuffer, 0, 1024, SocketFlags.None, ReceiveCallBack, clientSocket);

            serverSocket.BeginAccept(AcceptCallBack, serverSocket);
            //循环回调等待下一个信息传来

        }
    }
}
