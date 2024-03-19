using Ardalis.ListStartupServices;
using MyFileSpace.Api;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.UseSerilog((_, config) => config.ReadFrom.Configuration(builder.Configuration));

builder.Services.AddModulesConfiguration(builder.Environment, builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerConfiguration();

builder.Services.AddServiceDescriptorConfiguration();

builder.Services.AddHttpContextAccessor();

builder.Services.AddDistributedMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseShowAllServicesMiddleware();
}
else
{
    app.UseExceptionHandler(Constants.ERROR_PATH);
    app.UseHsts();
}

app.UserCorsConfiguration();

app.UseHttpsRedirection();

app.UseStaticFiles();

// Enable middleware to serve generated Swagger as a JSON endpoint.
app.UseSwagger();

app.UseSwaggerUIConfiguration();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// Migrate and seed Database
//app.Services.SetDbInstance();

app.Run();
