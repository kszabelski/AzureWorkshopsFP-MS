using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

namespace PictureOptimizer
{
    public class Program
    {
        static void Main()
        {
            var config = new JobHostConfiguration();
            config.UseServiceBus();
            var host = new JobHost(config);
            host.RunAndBlock();
        }
    }
}
