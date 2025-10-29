using DicomMeasurementApi.Data;
using DicomMeasurementApi.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

//注册MVC服务
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        //使用小驼峰命名法并格式化输出
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
    }
    );

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}
else
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        var connectionString = builder.Configuration.GetConnectionString("MySqlConnection");
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    });
}

builder.Services.AddScoped<IMeasurementServer, MeasurementServer>();

//配置Cros策略(跨资源共享)
var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new[] { "*" };
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowNextCloud", policy =>
    {
        policy.WithOrigins(corsOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseCors("AllowNextCloud");
app.UseAuthorization();
app.MapControllers();

//确保数据库创建，之后再看情况加吧(不加，没有找到数据库和表直接报错了)

app.Run();
