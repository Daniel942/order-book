using Microsoft.EntityFrameworkCore;
using order_book_backend.Data;
using order_book_backend.Services;

var builder = WebApplication.CreateBuilder(args);

var allowSpecificOrigins = "_allowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allowSpecificOrigins,
        builder =>
        {
            builder.WithOrigins("http://localhost:3000");
        });
});

builder.Services.AddScoped<IOrderBookService, OrderBookService>();

builder.Services.AddDbContext<OrderBookContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(allowSpecificOrigins);

WebSocketOptions webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};

app.UseWebSockets(webSocketOptions);

app.UseAuthorization();

app.MapControllers();

app.Run();
