using SOApiProject.Data;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(
    builder =>
    {
        builder.AddConsole();
        builder.AddDebug();
    }
);

builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDB"));

builder.Services.AddScoped<IMongoService, MongoService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddTransient<IDatabaseInitializer,DatabaseInitializer>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

using var scope = app.Services.CreateScope();
var initializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
await initializer.InitDb();

app.Run();