using Microsoft.Extensions.FileProviders;
using webapi.Configuration;
using webapi.DB;
using webapi.Interfaces;
using webapi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
//builder.Services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));


builder.Services.AddSingleton<IDsakConfiguration, AppConfig>();
builder.Services.AddSingleton<IDsakService, DsakService>();
builder.Services.AddSingleton<ISqlHelper, SqlHelper>();
builder.Services.AddSingleton<IDsakDbRepo, DsakDbRepo>();

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("*")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
        });
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();    
    app.UseSwaggerUI(c =>
    {
        string swaggerJsonBasePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
        c.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v1/swagger.json", "Ticket Search API");
    });
}

app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);
//app.UseAuthorization();

app.MapControllers();

app.Run();
