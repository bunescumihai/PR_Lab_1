using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class ThreadsHandler
    {
        List<Thread> threads = new List<Thread>();


        public void Add(Thread thread)
        {
            threads.Add(thread);
            thread.Start();
        }

        public void Remove(Thread thread) {
            removeThreedFromList(thread);
        }

        public void Remove(int index) {
            removeThreedFromList(threads.ElementAt(index));
        }

        private void removeThreedFromList(Thread thread)
        {
            thread.Abort();
            threads.Remove(thread);
        }
    }
}
