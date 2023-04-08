using CliWrap;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using System.Text;

namespace ingress_nginx_validate_jwt_tests
{
    public class EndToEndBase : IDisposable
    {
        readonly IContainer container;

        readonly string ImageName = $"ingress-nginx-validate-jwt-tests:{Guid.NewGuid()}";

        public EndToEndBase()
        {
            string fullPath = Path.GetFullPath("../../../../ingress-nginx-validate-jwt");
            var stdErrBuffer = new StringBuilder();

            var result = Cli.Wrap("docker")
                .WithArguments(new[] { "build", "-t", ImageName, "-f", "Dockerfile", "." })
                .WithWorkingDirectory(fullPath)
                .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer))
                .WithValidation(CommandResultValidation.None)
                .ExecuteAsync().GetAwaiter().GetResult();

            Console.WriteLine(stdErrBuffer);

            container = new ContainerBuilder()
              //.WithImage("ingress-nginx-validate-jwt")
              .WithImage(ImageName)
              .WithEnvironment("OpenIdProviderConfigurationUrl", "https://login.microsoftonline.com/common/v2.0/.well-known/openid-configuration")
              .WithPortBinding(8080)
              .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(8080))
              .Build();

            container.StartAsync().Wait();
        }

        public void Dispose()
        {
            container.StopAsync().Wait();

            Thread.Sleep(TimeSpan.FromSeconds(10));

            Cli.Wrap("docker")
                .WithArguments(new[] { "image", "rm", ImageName })
                .ExecuteAsync().GetAwaiter().GetResult();
        }
    }
}
