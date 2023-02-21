
using Alpha.CloneAccount.Api.Extensions;
using Fanex.Customer.API.Modules.SecurityCode;
using Fanex.Data.Repository;
using MinimalApi.Endpoint.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IDynamicRepository, DynamicRepository>();
builder.Services.RegisterSecurityCodeModule();
builder.Services.AddEndpoints();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseFanexData(builder.Configuration, app.Environment);
app.MapEndpoints();
app.Run();