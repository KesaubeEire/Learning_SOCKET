using System;
using System.Text;
using System.Linq;//给于数组连接方法的关键库


namespace SOCKEY_TCP_Client
{
 /// <summary>
 /// 用来转换数据为带长度的数组的类
 /// </summary>
    public class Message
    {
        public static byte[] GetBytes(string data)
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            int dataLength = dataBytes.Length;
            byte[] lengthBytes = BitConverter.GetBytes(dataLength);
            byte[] newBytes = lengthBytes.Concat(dataBytes).ToArray();
            return newBytes;
        }
    }
}
