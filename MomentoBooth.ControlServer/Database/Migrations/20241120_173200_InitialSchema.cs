using FluentMigrator;

namespace MomentoBooth.ControlServer.Database.Migrations
{
    [Migration(20241120_173200)]
    public class CreateUserTable : Migration
    {
        public override void Up()
        {
            Create.Table("Users");
        }

        public override void Down()
        {
            Delete.Table("Users");
        }
    }
}
