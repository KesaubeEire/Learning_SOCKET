using System;
using System.Text;

namespace ChangeInputType
{
    class MainClass
    {
        public static void Main(string[] args)
        {

            //byte[] data = Encoding.UTF8.GetBytes("魏先");
            int a = 102498650;
            byte[] data = BitConverter.GetBytes(a);
            foreach (byte b in data)
            {
                Console.Write(b + ":");
            }
            Console.WriteLine();
            Console.ReadKey();
        }
    }
}
