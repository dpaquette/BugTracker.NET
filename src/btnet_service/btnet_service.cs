using System;
//compile like so:
//csc btnet_service.cs POP3Main.cs POP3Client.cs

//then run "installutil.exe"

using System.ComponentModel;
using System.Configuration.Install;

namespace btnet {

	///////////////////////////////////////////////////////////////////////
	public class service : System.ServiceProcess.ServiceBase
	{

		protected static POP3Main pop3;

		public static void Main (string[] args)
		{
			System.ServiceProcess.ServiceBase.Run(new service());
		}


		public service()
		{
		   this.ServiceName = "btnet_service";
		   this.CanStop = true;
		   this.CanPauseAndContinue = true;
		   this.AutoLog = true;
		}


		protected override void OnStart(string[] args) {
			bool verbose = false;
            // look in this exe's folder for the config, not the c:\ root folder.
            string this_exe = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
			pop3 = new POP3Main(System.IO.Path.GetDirectoryName(this_exe) + "\\btnet_service.exe.config", verbose);
			OnContinue();
		}
		protected override void OnStop() {
			pop3.stop();
		}
		protected override void OnPause() {
			pop3.pause();
		}
		protected override void OnContinue() {
			pop3.start();
		}

	}

	[RunInstaller(true)]
	public class ProjectInstaller : System.Configuration.Install.Installer
	{

		private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller1;
		private System.ServiceProcess.ServiceInstaller serviceInstaller1;
		private System.ServiceProcess.ServiceController serviceController1;

		private const string SERVICE_NAME = "btnet_service";

		public ProjectInstaller()
		{

			this.serviceProcessInstaller1 = new System.ServiceProcess.ServiceProcessInstaller();
			this.serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
			this.serviceProcessInstaller1.Password = null;
			this.serviceProcessInstaller1.Username = null;

			this.serviceInstaller1 = new System.ServiceProcess.ServiceInstaller();
			this.serviceInstaller1.AfterInstall += new InstallEventHandler(AfterInstallEventHandler);
			this.serviceInstaller1.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
			this.serviceInstaller1.ServiceName = SERVICE_NAME;
			this.serviceInstaller1.ServicesDependedOn = new string [] { "Tcpip" };

			this.Installers.AddRange(
				new System.Configuration.Install.Installer[] {
					this.serviceProcessInstaller1,
					this.serviceInstaller1}
				);
		}

		private void AfterInstallEventHandler(object sender, InstallEventArgs e)
		{
			serviceController1 = new System.ServiceProcess.ServiceController(SERVICE_NAME);
			serviceController1.Start();
			serviceController1.WaitForStatus(
				System.ServiceProcess.ServiceControllerStatus.Running,
				TimeSpan.FromMinutes(1));
			serviceController1.Close();
		}
	}
}