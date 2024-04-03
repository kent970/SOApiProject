using SOApiProject.Data;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(
    builder =>
    {
        builder.AddConsole();
        builder.AddDebug();
    }
);
builder.Services.AddHttpClient();
builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDB"));
builder.Services.AddScoped<IApiService, ApiService>();
builder.Services.AddScoped<IMongoService, MongoService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDatabaseInitializer,DatabaseInitializer>();
builder.Services.AddHttpClient<IApiService,ApiService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["BaseUrl"]);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

using var scope = app.Services.CreateScope();
var initializer = scope.ServiceProvider.GetRequiredService<IMongoService>();
await initializer.FetchDataToDatabase();

app.Run();