using System.Text.Json;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using ServerlessAPI.Entities.Implementations;
using ServerlessAPI.Entities.Interface;
using ServerlessAPI.Repositories.Implementations.DynamoDB;
using ServerlessAPI.Repositories.Implementations.SQLServer;
using ServerlessAPI.Repositories.Interface;
using System.Data.SqlClient;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using ServerlessAPI.Utilities;
using System.Collections;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

//Logger
builder.Logging
        .ClearProviders()
        .AddJsonConsole();

// Add services to the container.
builder.Services
        .AddControllers()        
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

builder.Services.AddSingleton<IErrorMessages, ErrorMessages>();

if (true)
{
    string region = Environment.GetEnvironmentVariable("AWS_REGION") ?? RegionEndpoint.USEast1.SystemName;
    builder.Services
            .AddSingleton<IAmazonDynamoDB>(new AmazonDynamoDBClient(RegionEndpoint.GetBySystemName(region)))
            .AddScoped<IDynamoDBContext, DynamoDBContext>()
            .AddScoped<IUserEntity, UserEntity>()
            .AddScoped<IRepository, UserRepositoryDynamoDB>();
} else {
    string connectionString =
        "Data Source=localhost;" +
        "Initial Catalog=ServiceData;" +
        "Integrated Security=SSPI;" +
        "TrustServerCertificate=True;";

    builder.Services
        .AddScoped<IUserEntity, UserEntity>()
        .AddScoped<IRepository>(x =>
            new UserRepositorySQLServer(
                    connectionString,
                    x.GetRequiredService<ILogger<UserRepositorySQLServer>>(),
                    x.GetRequiredService<IErrorMessages>()
            )
        );

}
/*
string region = Environment.GetEnvironmentVariable("AWS_REGION") ?? RegionEndpoint.USEast1.SystemName;
builder.Services
        .AddSingleton<IAmazonDynamoDB>(new AmazonDynamoDBClient(RegionEndpoint.GetBySystemName(region)))
        .AddScoped<IDynamoDBContext, DynamoDBContext>()
        .AddScoped<IUserEntity, UserEntity>()
        .AddScoped<IUserRepository, UserRepositoryDynamoDB>();
*/
// Register UserRepositorySQLServer with connection string and logger





// Add AWS Lambda support. When running the application as an AWS Serverless application, Kestrel is replaced
// with a Lambda function contained in the Amazon.Lambda.AspNetCoreServer package, which marshals the request into the ASP.NET Core hosting framework.
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);


var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => "Welcome to running ASP.NET Core Minimal API on AWS Lambda");

app.Run();
