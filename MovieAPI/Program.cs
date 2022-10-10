using Amazon.S3;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MovieAPI.Data.DbConfig;
using MovieAPI.Services;
using MovieAPI.Services.SignalR;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//Add services log
//var logger = new LoggerConfiguration()
//    .ReadFrom.Configuration(builder.Configuration)
//    .CreateLogger();
//builder.Logging.ClearProviders();
//builder.Logging.AddSerilog(logger);
builder.Services.AddControllers();
//Add config database
AppSettings.ConnectionString = builder.Configuration.GetConnectionString("MovieAPIConnection");
builder.Services.AddDbContext<MovieAPIDbContext>(
    //options => options.UseSqlServer(AppSettings.ConnectionString)
);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
        .SetIsOriginAllowed(origin => true));
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
//Add services Momo
AppSettings.PartnerCode = builder.Configuration["MomoConnectionInformation:PartnerCode"];
AppSettings.MomoAccessKey = builder.Configuration["MomoConnectionInformation:AccessKey"];
AppSettings.MomoSerectkey = builder.Configuration["MomoConnectionInformation:Serectkey"];
AppSettings.Endpoint = builder.Configuration["MomoConnectionInformation:Endpoint"];
AppSettings.ReturnUrl = builder.Configuration["MomoConnectionInformation:ReturnUrl"];
AppSettings.NotifyUrl = builder.Configuration["MomoConnectionInformation:Notifyurl"];
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
    endpoints.MapControllers();
    endpoints.MapHub<ChatHub>("/chat");
    endpoints.MapHub<ReviewHub>("/review");
});
app.UseHttpsRedirection();

app.MapControllers();
app.Run();
