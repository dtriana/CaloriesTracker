using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();
var sqldb = sql.AddDatabase("calories-tracking-db");

var _ = builder.AddProject<CaloriesTracker>("web")
    .WaitFor(sqldb)
    .WithReference(sqldb);

builder.Build().Run();
