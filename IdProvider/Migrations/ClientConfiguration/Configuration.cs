using System.Data.Entity.Migrations;

namespace IdProvider.Migrations.ClientConfiguration
{
    internal sealed class Configuration : DbMigrationsConfiguration<IdentityServer3.EntityFramework.ClientConfigurationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"Migrations\ClientConfiguration";
        }

        protected override void Seed(IdentityServer3.EntityFramework.ClientConfigurationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
