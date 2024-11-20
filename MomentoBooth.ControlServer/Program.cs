using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using dotenv.net;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Npgsql;

// TODO: Investigate how the start folder is resolved... See if we can improve this, the 10 is simply arbitrary but works for now.
DotEnv.Load(options: new DotEnvOptions(probeForEnv: true, ignoreExceptions: false, probeLevelsToSearch: 10));

// ////////////////// //
// Configure services //
// ////////////////// //

var builder = WebApplication.CreateBuilder(args);

// Add PostgreSQL data source
string? dbConnStr = builder.Configuration.GetValue<string>("Db:PostgresConnStr");
if (string.IsNullOrEmpty(dbConnStr)) {
    var connStrBuilder = new NpgsqlConnectionStringBuilder
    {
        Host = builder.Configuration.GetValue<string>("Db:PostgresHost"),
        Port = builder.Configuration.GetValue<int>("Db:PostgresPort"),
        Database = builder.Configuration.GetValue<string>("Db:PostgresDatabase"),
        Username = builder.Configuration.GetValue<string>("Db:PostgresUser"),
        Password = builder.Configuration.GetValue<string>("Db:PostgresPassword"),
        SslMode = SslMode.Require,
    };
    dbConnStr = connStrBuilder.ConnectionString;
}
builder.Services.AddNpgsqlDataSource(dbConnStr);

// Add FluentMigrator
builder.Services.AddFluentMigratorCore()
    .ConfigureRunner(rb => rb
        .AddPostgres()
        .WithGlobalConnectionString(dbConnStr)
        .ScanIn(Assembly.GetEntryAssembly()).For.Migrations());

var app = builder.Build();

// /////////////////////// //
// Configure HTTP pipeline //
// /////////////////////// //

app.UseHttpsRedirection();

// Run migrations
using var scope = app.Services.CreateScope();
var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
runner.MigrateUp();

app.Run();
