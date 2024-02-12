using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Job: IJob
    {
        Action func;

        public Job(Action func) { 
            this.func = func;
        }

        public void Execute()
        {
            func();
        }
    }
}
