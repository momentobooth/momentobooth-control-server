using FluentMigrator;

namespace MomentoBooth.ControlServer.Database.Migrations
{
    [Migration(20241120_173200)]
    public class CreateUserTable : Migration
    {
        public override void Up()
        {
            Execute.Sql("CREATE EXTENSION IF NOT EXISTS \"citext\";");
            Execute.Sql("CREATE EXTENSION IF NOT EXISTS \"uuid-ossp\";");

            Create.Table("organizations")
                .WithColumn("id").AsCustom("uuid").PrimaryKey("organizations_pk").NotNullable().WithDefault(SystemMethods.NewGuid)
                .WithColumn("name").AsString(256)
                .WithColumn("created_at").AsCustom("timestamptz").NotNullable().WithDefault(SystemMethods.CurrentDateTimeOffset);

            Create.Table("users")
                .WithColumn("id").AsCustom("uuid").PrimaryKey("users_pk").NotNullable().WithDefault(SystemMethods.NewGuid)
                .WithColumn("name").AsString(256).NotNullable()
                .WithColumn("email").AsCustom("citext").NotNullable().Indexed("user_emails_index")
                .WithColumn("password_hash").AsBinary(256).NotNullable()
                .WithColumn("last_time_logged_in_at").AsCustom("timestamptz").Nullable()
                .WithColumn("created_at").AsCustom("timestamptz").NotNullable().WithDefault(SystemMethods.CurrentDateTimeOffset);

            Create.Table("organizations_users")
                .WithColumn("organization_id").AsCustom("uuid").NotNullable().ForeignKey("organizations", "id").OnDeleteOrUpdate(System.Data.Rule.Cascade)
                .WithColumn("user_id").AsCustom("uuid").NotNullable().ForeignKey("users", "id").OnDeleteOrUpdate(System.Data.Rule.Cascade)
                .WithColumn("created_at").AsCustom("timestamptz").NotNullable().WithDefault(SystemMethods.CurrentDateTimeOffset);

            Create.PrimaryKey("organizations_users_pk").OnTable("organizations_users").Columns(["organization_id", "user_id"]);

            Create.Table("photobooth_instances")
                .WithColumn("id").AsCustom("uuid").PrimaryKey("photobooth_instances_pk").NotNullable().WithDefault(SystemMethods.NewGuid)
                .WithColumn("friendly_name").AsString(256).NotNullable()
                .WithColumn("system_name").AsString(256).Nullable()
                .WithColumn("connect_key").AsCustom("uuid").NotNullable().WithDefault(SystemMethods.NewGuid)
                .WithColumn("last_time_connected_at").AsCustom("timestamptz").Nullable()
                .WithColumn("created_at").AsCustom("timestamptz").NotNullable().WithDefault(SystemMethods.CurrentDateTimeOffset);

            Create.UniqueConstraint("organizations_users_records_unique").OnTable("organizations_users").Columns(["organization_id", "user_id"]);
        }

        public override void Down()
        {
            Delete.UniqueConstraint("organizations_users_records_unique");

            Delete.Table("photobooth_instances");

            Delete.PrimaryKey("organizations_users_pk");

            Delete.Table("organizations_users");
            Delete.Table("users");
            Delete.Table("organizations");

            Execute.Sql("DROP EXTENSION \"uuid-ossp\";");
            Execute.Sql("DROP EXTENSION \"citext\";");
        }
    }
}
