using Scrabble.Server.Models;
using Scrabble.Server.Services;

var builder = WebApplication.CreateBuilder(args);

//builder.Configuration.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddSingleton<BoardService>();
builder.Services.AddSingleton<DictionaryLookupService>();
builder.Services.AddSingleton<IBoardValidator, BoardValidator>();
builder.Services.AddSingleton<GameSessionManager>();
builder.Services.Configure<BoardServiceConfiguration>(builder.Configuration.GetSection("Board"));
builder.Services.AddOptions<BoardServiceConfiguration>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder
            .AllowAnyHeader()
            .WithOrigins("http://localhost:4200")
            .AllowCredentials()
            .AllowAnyMethod();
    });
});

builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseRouting();
app.UseAuthorization();
app.MapHub<GameHub>("/game");

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();
//app.MapControllers();

app.Run();
