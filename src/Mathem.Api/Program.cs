using System.Text.Json.Serialization;
using Mathem.Api;
using Microsoft.AspNetCore.Http.Json;
using MvcJsonOptions = Microsoft.AspNetCore.Mvc.JsonOptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<JsonOptions>(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.Configure<MvcJsonOptions>(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddScoped<IGetDeliveryDatesService, GetDeliveryDatesService>();
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/postalcode/{postalCode}", (int postalCode, List<Product> products, IGetDeliveryDatesService getDeliveryDatesService) =>
{
    return getDeliveryDatesService.Handle(
        postalCode,
        products);
});

app.Run();
