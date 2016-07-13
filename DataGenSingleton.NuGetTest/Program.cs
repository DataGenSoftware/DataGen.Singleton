using DataGen.Singleton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGenSingleton.NuGetTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var singleton1 = SingletonTest.Instance;
            singleton1.Value = 13;

            var singleton2 = SingletonTest.Instance;
            singleton2.Value = 7;

            Console.WriteLine(singleton1.Value);

            Console.ReadKey();
        }
    }


    public class SingletonTest:Singleton<SingletonTest>
    {
        public int Value { get; set; }
    }
}
