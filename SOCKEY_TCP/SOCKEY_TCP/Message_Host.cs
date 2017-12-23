using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

namespace SOCKEY_TCP
{
     class Message_Host
    {
        private byte[] data = new byte[1024];
        private int startIndex = 0;//代表我们存取了多少个字节的数据在数组里面

          public byte[] Data { get { return Data; } }
        public int StartIndex { get { return startIndex; } }
        public int RemainSize { get { return data.Length - startIndex; } }// 显示剩余的空间大小

        /// <summary>
        /// 更新索引方法
        /// </summary>
        /// <param name="count"></param>
        public void AddCount(int count)
        {
            startIndex += count;
        }

        /// <summary>
        /// 解析消息或读取数据
        /// 逻辑是，数据没有头数据的四个字节（包含数据长度信息的那4个字节）就不接收数据
        /// 其次数据不够长度信息里的长度也不接受数据
        /// 完事以后更新那个上面的长度，长度会发生变化的，这里我还没有完全搞懂
        /// </summary>
        public void ReadMessage()
        {
            while (true)
            {
                if (startIndex <= 4) break;
                int count = BitConverter.ToInt32(data, 0);
                if ((startIndex - 4) >= count)
                {
                    Console.WriteLine(startIndex);
                    Console.WriteLine(count);
                    string s = Encoding.UTF8.GetString(data, 4, count);
                    Console.WriteLine("解析出来一条数据：" + s);
                    Array.Copy(data, count + 4, data, 0, startIndex - 4 - count);
                    startIndex -= (count + 4);
                }
                else break;
            }

        }
    }
}
