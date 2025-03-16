using MilyUnaNochesService.Contracts;
using MilyUnaNochesService.Services;
using MilyUnaNochesService.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MilyUnaNochesServer
{
    public static class Program
    {
        static void Main(string[] args)
        {
            using (ServiceHost host = new ServiceHost(typeof(MilyUnaNochesService.Services.MilyUnaNochesService)))
            {
                host.Open();
                Console.WriteLine("Service connected");
                Console.ReadLine();
            }
        }

    }
}
