//using System.ComponentModel;
//using System.ServiceProcess;

//namespace ResetService
//{
//    [RunInstaller(true)]
//    public partial class ProjectInstaller : System.Configuration.Install.Installer
//    {
//        private ServiceProcessInstaller serviceProcessInstaller;
//        private ServiceInstaller serviceInstaller;

//        public ProjectInstaller()
//        {
//            serviceProcessInstaller = new ServiceProcessInstaller();
//            serviceInstaller = new ServiceInstaller();

//            // Here you can set properties on serviceProcessInstaller or register event handlers
//            serviceProcessInstaller.Account = ServiceAccount.LocalSystem;

//            serviceInstaller.ServiceName = "ResetService";
//            serviceInstaller.DisplayName = "Reset Service";
//            serviceInstaller.Description = "A service to reset programs based on time.";
//            serviceInstaller.StartType = ServiceStartMode.Automatic;

//            Installers.AddRange(new Installer[] {
//                serviceProcessInstaller,
//                serviceInstaller
//            });
//        }
//    }
//}
