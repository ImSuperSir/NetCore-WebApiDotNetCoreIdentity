using System.IdentityModel.Tokens.Jwt;
using System.Text;
using ImSuperSir.Security.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims types

var connectionString = builder.Configuration.GetConnectionString("SecurityContext");

builder.Services.AddDbContext<SecurityContex>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SecurityContext")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        var lJWTKey = builder.Configuration["AspNetCoreIdentityJWTKet"] ?? throw new InvalidOperationException("Missing JWT Key");
                        var lJWTKeyBytes = Encoding.UTF8.GetBytes(lJWTKey);
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer =  "ImSuperSir.Security",// builder.Configuration["Jwt:Issuer"],
                            ValidAudience = "ImSuperSir.Security",//builder.Configuration["Jwt:Audience"],
                            IssuerSigningKey = new SymmetricSecurityKey(lJWTKeyBytes)
                        };
                    });

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<SecurityContex>()
    .AddDefaultTokenProviders();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen( options => 
{
    options.SwaggerDoc("v1", new() { Title = "ImSuperSir.Security", Version = "v1" });

    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            },
            new string[] {}
        }
    });
    
}
);
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
