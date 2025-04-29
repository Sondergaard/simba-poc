using Azure.Core;
using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using SimbaNetApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseHttpsRedirection();

app.MapGet("/", () => Results.Redirect("/lineitems"));

app.MapGet("/lineitems", LineitemEndpoint.GetAllLineitems)    
    .WithOpenApi()
    .Produces(200, typeof(LineitemEndpoint.Lineitem[]))
    .Produces(500, typeof(ProblemDetails));

app.Run();
