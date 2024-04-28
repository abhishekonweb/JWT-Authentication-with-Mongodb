using AspNetCore.Identity.MongoDbCore.Extensions;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using Authenticator.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
BsonSerializer.RegisterSerializer(new GuidSerializer(MongoDB.Bson.BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeSerializer(MongoDB.Bson.BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(MongoDB.Bson.BsonType.String));

// Add mongoIdentityConfiguration

var mongoSetting = builder.Configuration.GetSection("").Get<MongoSetting>();
var mongoDbConfiguration = new MongoDbIdentityConfiguration
{
    MongoDbSettings = new MongoDbSettings
    {
        ConnectionString = mongoSetting.ConnectionString,
        DatabaseName = mongoSetting.DatabaseName
    },
    IdentityOptionsAction = option =>
    {
        option.Password.RequireDigit = false;
        option.Password.RequiredLength = 8;
        option.Password.RequireNonAlphanumeric = true;

        //LockOut

        option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
        option.Lockout.MaxFailedAccessAttempts = 5;

        option.User.RequireUniqueEmail = true;
    }
};

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = true;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = "https:localhost:5001/",
        ValidAudience = "https:localhost:5001/",
        IssuerSigningKey= new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("kjgckjewf")),
        ClockSkew = TimeSpan.Zero  
    };
});
//configure mongo identity

builder.Services.ConfigureMongoDbIdentity<ApplicationUser, ApplicationRoles, Guid>(mongoDbConfiguration)
    .AddUserManager<UserManager<ApplicationUser>>()
    .AddSignInManager<UserManager<ApplicationUser>>()
    .AddRoleManager<RoleManager<ApplicationRoles>>()
    .AddDefaultTokenProviders();

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

