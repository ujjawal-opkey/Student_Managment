using Student_Management.Data;

var builder = WebApplication.CreateBuilder(args);

// Register your DataAccess class as a scoped service
builder.Services.AddScoped<DataAccess>();

// This is where your appsettings.json connection string is being accessed
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


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

app.UseAuthorization();

app.MapControllers();

app.Run();