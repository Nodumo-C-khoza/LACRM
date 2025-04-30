using Microsoft.EntityFrameworkCore;
using Less_Annoying_CRM_Assessment.Entities;
using Less_Annoying_CRM_Assessment.ExtensionMethods;
using Less_Annoying_CRM_Assessment.Interfaces;
using Less_Annoying_CRM_Assessment.Logging;
using Less_Annoying_CRM_Assessment.Repositories;
using Less_Annoying_CRM_Assessment.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOptions<LacrmSettings>()
    .Bind(builder.Configuration.GetSection("Lacrm"))
    .Validate(settings => !string.IsNullOrWhiteSpace(settings.ApiKey), "LACRM API key is required");

builder.Services.Configure<LacrmSettings>(builder.Configuration.GetSection("Lacrm"));
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        //options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

builder.Services.AddHttpClient<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddSingleton<IRequestLogger, MemoryRequestLogger>();


builder.Services.AddMemoryCache();

builder.Services.AddCors(opt =>
{
    opt.AddPolicy(name: "CorsPolicy", builder =>
    {
        builder.WithOrigins("http://localhost:4200")
          .AllowAnyHeader()
          .AllowAnyMethod();
    });
});


var app = builder.Build();

// Apply CORS policy
app.UseCors("CorsPolicy");
app.UseRouting();
app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

