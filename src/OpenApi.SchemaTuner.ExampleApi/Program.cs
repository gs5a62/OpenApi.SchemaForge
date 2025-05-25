using OpenApi.SchemaTuner.Core.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi("public", options => { options.AddSchemaTunerTransformers(); });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => { options.WithTitle("Custom Api"); });
}


// Map Scalar API references to separate endpoints
app.MapScalarApiReference("/api-reference", options => options.AddDocument("public"));

app.MapGet("/", context =>
{
    context.Response.Redirect("/api-reference");
    return Task.CompletedTask;
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();