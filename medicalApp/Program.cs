using Microsoft.EntityFrameworkCore;
using medicalApp.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using medicalApp.Services;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressMapClientErrors = true;
});

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTSettings:SecretKey"])),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JWTSettings:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JWTSettings:Audience"],
            ValidateLifetime = true
        };
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDataContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("backMedBase")));
builder.Services.AddSingleton(new AuthService(builder.Configuration["JWTSettings:SecretKey"]));

builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    opt.OperationFilter<AuthorizeCheckFilter>();
});


// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();