namespace ServiceInsightInstaller.Managed
{
    using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;

    public class InstallerBootstrapper : BootstrapperApplication
    {
        protected override void Run()
        {
            Engine.Log(LogLevel.Verbose, "Starting the SC installer.");

            var app = new App();
            new CaliburnMicroBootstrapper(this);
            app.Run();

            Engine.Log(LogLevel.Verbose, "Finished the SC installer.");

            Engine.Quit(0);
        }
    }
}