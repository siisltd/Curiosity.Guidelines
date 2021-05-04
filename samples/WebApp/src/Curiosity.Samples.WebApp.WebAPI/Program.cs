using System.Threading.Tasks;
using Curiosity.Hosting;
using Curiosity.Hosting.Web;
using Curiosity.Samples.WebApp.API.Configuration;

namespace Curiosity.Samples.WebApp.API
{
    public class CliArgs : CuriosityCLIArguments
    {
        public CliArgs() : base("Curiosity sample WebAPI")
        {
        }
    }
    
    class Program
    {
        public static Task<int> Main(string[] args)
        {
            var bootstrapper = new CuriosityWebAppBootstrapper<CliArgs, AppConfiguration, Startup.Startup>();
            return bootstrapper.RunAsync(args);
        }
    }
}