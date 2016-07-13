using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataGen.Singleton
{
    public abstract class Singleton<T>
        where T : class
    {
        private static volatile T instance;

        private static readonly object syncObject = new object();

        protected Singleton() { }

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncObject)
                    {
                        if (instance == null)
                            instance = Activator.CreateInstance(typeof(T), true) as T;
                    }
                }

                return instance;
            }
        }
    }
}
