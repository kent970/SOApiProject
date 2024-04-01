
using SOApiProject.Data;


var builder = WebApplication.CreateBuilder(args);

//todo napewno trasient i singleton?? a nie scoped?
builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDB"));
builder.Services.AddSingleton<IMongoService,MongoService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddTransient<DatabaseInitializer>();
var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

var initializer = app.Services.GetRequiredService<DatabaseInitializer>();
await initializer.InitDb();
    
app.Run();
