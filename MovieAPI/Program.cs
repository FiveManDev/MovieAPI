using Amazon.S3;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MovieAPI;
using MovieAPI.Data.DbConfig;
using MovieAPI.Services;
using MovieAPI.Services.SignalR;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//Add services log
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);
builder.Services.AddControllers().AddNewtonsoftJson();
//Add config database
AppSettings.ConnectionString = builder.Configuration.GetConnectionString("MovieAPIConnection");
builder.Services.AddDbContext<MovieAPIDbContext>(
    //options => options.UseSqlServer(AppSettings.ConnectionString)
);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//Add services version Api
builder.Services.AddApiVersioning();

//Add services token
AppSettings.SecretKey = builder.Configuration["AppSettings:SecretKey"];
var secretKeyByte = Encoding.UTF8.GetBytes(AppSettings.SecretKey);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(secretKeyByte),
            ClockSkew = TimeSpan.Zero
        };
    });
//Add services AWSS3
AppSettings.AWSS3BucketName = builder.Configuration["AWSS3Bucket:AWSS3BucketName"];
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonS3>();
// Add auto mapper services
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// Add services SignalR
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder => builder
        .AllowAnyMethod()
        .AllowAnyHeader()
        .SetIsOriginAllowed(origin => true)
        .AllowCredentials());
});
builder.Services.AddSignalR();
// Add services OData
builder.Services.AddControllers().AddOData(opt=>
                                  opt.Select().Filter().Expand().OrderBy().Count().SetMaxTop(100));
//Add services mail
AppSettings.Mail = builder.Configuration["MailConnectionInformation:Mail"];
AppSettings.MailTile = builder.Configuration["MailConnectionInformation:MailTile"];
AppSettings.MailAppPassword = builder.Configuration["MailConnectionInformation:MailAppPassword"];
AppSettings.Host = builder.Configuration["MailConnectionInformation:Host"];
AppSettings.Port = builder.Configuration["MailConnectionInformation:Port"];
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Version = "1", Title = "My API" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.Http
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});
builder.Services.AddHostedService<Worker>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
   
    app.UseSwagger();
    app.UseSwaggerUI();
    
}
// Config App Use SignalR
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("CorsPolicy");
app.UseEndpoints(endpoints => {
    endpoints.MapHub<ChatHub>("/chat");
    endpoints.MapHub<ReviewHub>("/review");
});
app.UseHttpsRedirection();

app.MapControllers();
app.Run();
