using Microsoft.EntityFrameworkCore;
using SingularSystemsAssessment.Entities;
using SingularSystemsAssessment.Interfaces;
using SingularSystemsAssessment.Repositories;
using SingularSystemsAssessment.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<SingularDbContext>(option => option.UseInMemoryDatabase("SingularSystemsDB"));

builder.Services.AddHttpClient<IExternalApiService,ExternalApiService>(client =>
{
    client.BaseAddress = new Uri("https://singularsystems-tech-assessment-sales-api2.azurewebsites.net");
});

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ISaleRepository, SaleRepository>();
builder.Services.AddScoped<IExternalApiService, ExternalApiService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ISaleService, SaleService>();


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
