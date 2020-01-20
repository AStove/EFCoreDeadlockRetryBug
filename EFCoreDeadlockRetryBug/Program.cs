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
            //services.AddDbContext<DemoContext>(options => options.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=testEFCoreDeadlockRetryBug;Trusted_Connection=True;", o => o.EnableRetryOnFailure()));
            services.AddDbContext<DemoContext>(options => options.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=testEFCoreDeadlockRetryBug;Trusted_Connection=True;"));
            ServiceProvider provider = services.BuildServiceProvider();

            await InitializeDb(provider);
            var endlessTask1 = ChangeParents(provider);
            await NumberOfChildren(provider);
        }


        private static async Task InitializeDb(ServiceProvider provider)
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

        private static async Task ChangeParents(ServiceProvider provider)
        {
            while (true)
            {
                using (IServiceScope scope = provider.CreateScope())
                {
                    DemoContext demoContext = scope.ServiceProvider.GetRequiredService<DemoContext>();

                    //Create a new parent and delete the old one
                    var newParent = new Parent();
                    var oldParent = (await demoContext.Children.Include(c => c.Parent).FirstAsync()).Parent;
                    demoContext.Parents.Add(newParent);
                    demoContext.Parents.Remove(oldParent);

                    //assign the new parent to the children
                    await demoContext.Children.ForEachAsync(child => child.Parent = newParent);

                    //save
                    await demoContext.SaveChangesAsync();
                }
            }
        }

        private static async Task NumberOfChildren(ServiceProvider provider)
        {
            while (true)
            {
                var numberOfChildren = 0;

                using (IServiceScope scope = provider.CreateScope())
                {
                    DemoContext demoContext = scope.ServiceProvider.GetRequiredService<DemoContext>();

                    //Count the children
                    var parentWithChildren = await demoContext.Parents
                        .Include(p => p.Children)
                        .FirstAsync();

                    numberOfChildren = parentWithChildren.Children.Count();
                }

                Console.WriteLine(numberOfChildren);

                if (numberOfChildren != 6)
                {
                    return;
                }
            }
        }
    }
}
