// Code hiervoor en hierna laten staan.
// De code hieronder is toevoeging. Zorg dat je deze ook hebt.

builder.Services.AddLogging(builder =>
    builder.AddDebug()
        .AddConsole()
        .SetMinimumLevel(LogLevel.Information));

// Prefer environment variable for connection string if present
var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__{{JouwConnectieNaam}}")
    ?? builder.Configuration.GetConnectionString("{{JouwConnectieNaam}}");
Console.WriteLine($"Using connection string: {connectionString}");

builder.Services.AddDbContext<{{Jouw context hier}}>(options =>
  options.UseSqlServer(connectionString));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    // Bij het opstarten zal hij de database migraties uitvoeren.
    var db = scope.ServiceProvider.GetRequiredService<{{Jouw context hier}}>();
    db.Database.Migrate();
}

// Forwarded headers are required for running behind a reverse proxy
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
});
// app.UseHttpsRedirection(); // Disabled for proxy usage