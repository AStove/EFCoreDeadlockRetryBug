using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreDeadlockRetryBug
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ServiceCollection services = new ServiceCollection();
            services.AddDbContext<DemoContext>(options => options.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=testEFCoreDeadlockRetryBug;Trusted_Connection=True;", o => o.EnableRetryOnFailure()));
            ServiceProvider provider = services.BuildServiceProvider();

            await InitializeDb(provider);

        }


        static private async Task InitializeDb(ServiceProvider provider)
        {
            using (IServiceScope scope = provider.CreateScope())
            {
                DemoContext demoContext = scope.ServiceProvider.GetRequiredService<DemoContext>();

                //Clear the table
                demoContext.Parents.RemoveRange(demoContext.Parents.ToArray());
                demoContext.Children.RemoveRange(demoContext.Children.ToArray());

                //Create parent with a number of children
                var ParentWithChildren = new Parent();
                demoContext.Parents.Add(ParentWithChildren);

                for (int i = 0; i <= 5; i++)
                {
                    demoContext.Children.Add(new Child { Parent = ParentWithChildren });
                }

                //Save it to the database
                await demoContext.SaveChangesAsync();
            }
        }

        private async Task ChangeParents()
        {

            while (true)
            {
                using (IServiceScope scope = provider.CreateScope())
                {
                    //Create a new parent and move the children to this parent
                    demoContext.Parents.Add(new Parent());
                }
            }
        }
    }
}
