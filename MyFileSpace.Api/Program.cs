using Ardalis.ListStartupServices;
using MyFileSpace.Api;
using MyFileSpace.Api.Middlewares;
using MyFileSpace.Infrastructure;
using MyFileSpace.SharedKernel.Providers;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureKeyVault();

// Add services to the container.
builder.Host.UseSerilog((_, config) => config.ReadFrom.Configuration(builder.Configuration));

builder.Services.AddModulesConfiguration(builder.Environment, builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.Services.Initialize();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseShowAllServicesMiddleware();
    // Enable middleware to serve generated Swagger as a JSON endpoint.
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler(Constants.ERROR_PATH);
    app.UseHsts();
}

app.UserCorsConfiguration();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseMiddleware<CustomExceptionHandlerMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// Migrate and seed Database
app.Services.SetDbInstance();

app.Run();
