using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFCoreDeadlockRetryBug
{

    public class DemoContextFactory : IDesignTimeDbContextFactory<DemoContext>
    {
        public DemoContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DemoContext>();
            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=testEFCoreDeadlockRetryBug;Trusted_Connection=True;", o => o.EnableRetryOnFailure());

            return new DemoContext(optionsBuilder.Options);
        }

    }

    public class DemoContext: DbContext
    {
        public DemoContext(DbContextOptions<DemoContext> options) : base(options)
        {

        }

        public virtual DbSet<Child> Children { get; set; }
        public virtual DbSet<Parent> Parents { get; set; }
    }
}
