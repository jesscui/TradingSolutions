using Asp.Versioning;
using FluentValidation;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using TradingSolutions;
using TradingSolutions.Application.Processors;
using TradingSolutions.Application.Repositories;
using TradingSolutions.Application.Requests.Nfl;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddFluentValidationAutoValidation()
                .AddValidatorsFromAssemblyContaining<AddNflPlayerRequestValidator>();
builder.Services.AddTransient<INflChartProcessor, NflChartProcessor>();
builder.Services.AddSingleton<INflDepthChartRepository, NflDepthChartRepository>();
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
builder.Services.AddSwaggerGen()
                .AddFluentValidationRulesToSwagger();

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
