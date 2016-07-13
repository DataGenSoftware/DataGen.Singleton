using DataGen.Singleton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatGen.Singleton.Tests
{
    internal sealed class FakeSingleton : Singleton<FakeSingleton>
    {
        private FakeSingleton()
        {
            FakeSingleton.InstanceCount++;
        }

        public static int InstanceCount { get; set; }
    }
}
