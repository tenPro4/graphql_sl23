
using GraphQL_SL2023.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace GraphQL_SL2023
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();

            using (IServiceScope scope = host.Services.CreateScope())
            {
                IDbContextFactory<SchoolDbContext> contextFactory =
                    scope.ServiceProvider.GetRequiredService<IDbContextFactory<SchoolDbContext>>();

                using (SchoolDbContext context = contextFactory.CreateDbContext())
                {
                    context.Database.Migrate();
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
