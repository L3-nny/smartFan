using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Lenny API", Version = "v1"});
});

var app = builder.Build();


//Enable swagger
app.UseSwagger();
app.UseSwaggerUI();    


app.MapGet("/device_Condition", () => "ac_1: running " + "ac_2: failed");

//ambient condition
app.MapGet("/temperature", () => 
{
    var temp = new
    {
        Date = DateTime.Now.ToString("yyyy-MM-dd"),
        Temp = 25,
        Summary = "moderate"
    };

    return temp;

})
.WithName("GetTemp")
.WithOpenApi();

app.Run(); 