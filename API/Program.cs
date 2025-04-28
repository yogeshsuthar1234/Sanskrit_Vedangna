using BCrypt.Net;
using Persistence.Context;
using Persistence.Repositories;
using Application.Services;
using MockUPIPaymentGateway.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddSingleton<MongoDbContext>(provider => 
{
    var config = provider.GetRequiredService<IConfiguration>();
    return new MongoDbContext(config);
});

// In Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// After app build

builder.Services.AddScoped<ProgressRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddSingleton<MockUPIPaymentService>();


builder.Services.AddScoped<ProgressService>();

builder.Services.AddControllers();
// Add to existing services

var app = builder.Build();

   // Configure the HTTP request pipeline
   if (app.Environment.IsDevelopment())
   {
       app.UseDeveloperExceptionPage();
   }
   
 app.UseStaticFiles(); 
app.UseHttpsRedirection();
app.MapControllers();

app.UseCors("AllowAll");
app.Run();