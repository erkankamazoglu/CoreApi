using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment;

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CoreApi", Version = "v1" });
});


var app = builder.Build();

if (env.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CoreApi v1"));
}

app.UseHttpsRedirection();
app.UseRouting(); 
app.UseAuthorization(); 
app.MapControllers(); 