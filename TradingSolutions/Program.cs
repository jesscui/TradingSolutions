using Asp.Versioning;
using TradingSolutions;
using TradingSolutions.Application.Processors;
using TradingSolutions.Application.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddTransient<IPlayerProcessor, PlayerProcessor>(); //todo check this lifetime
builder.Services.AddSingleton<IDepthChartRepository, DepthChartRepository>();
builder.Services.AddControllers();
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
}).AddApiExplorer(options =>
{
    options.SubstituteApiVersionInUrl = true;
    options.GroupNameFormat = "'v'VVV";
});
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ExceptionMiddlewareFilter>();
app.UseAuthorization();

app.MapControllers();

app.Run();
