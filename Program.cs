using System;
using System.ServiceProcess;

namespace ResetService
{
    static class Program
    {
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new ResetService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
