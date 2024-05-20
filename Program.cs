using desafio.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();
builder.Configuration.AddJsonFile("appsettings.json");
var keySecret = builder.Configuration["AppSettings:KeySecret"];
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keySecret));

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "IoT Device (Desafio) API",
        Version = "v1",
        Description = "Plataforma colaborativa para compartilhamento de acesso a dados de dispositivos IoT."
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = key,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();
app.UseCors(policy =>
{
    policy.AllowAnyOrigin()
          .AllowAnyHeader()
          .AllowAnyMethod();
});
app.UseAuthentication();
app.UseAuthorization();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Desafio API v1");
        //c.RoutePrefix = string.Empty;
        c.RoutePrefix = "swagger";
    });
}

// Device data
var devices = new List<Device>();

app.MapGet("/device", () => devices)
    .WithName("GetDevices")
    .WithTags("Devices")
    .RequireAuthorization();

app.MapGet("/device/{id}", (string id) =>
{
    var device = devices.FirstOrDefault(d => d.Identifier == id);
    return device is not null ? Results.Ok(device) : Results.NotFound();
})
.WithName("GetDeviceById")
.WithTags("Devices")
.RequireAuthorization();

app.MapPost("/device", (Device device) =>
{
    devices.Add(device);
    return Results.Created($"/device/{device.Identifier}", device);
})
.WithName("CreateDevice")
.WithTags("Devices")
.RequireAuthorization();

app.MapPut("/device/{id}", (string id, Device updatedDevice) =>
{
    var device = devices.FirstOrDefault(d => d.Identifier == id);
    if (device is null) return Results.NotFound();
    device.Description = updatedDevice.Description;
    device.Manufacturer = updatedDevice.Manufacturer;
    device.Url = updatedDevice.Url;
    device.Commands = updatedDevice.Commands;
    return Results.Ok(device);
})
.WithName("UpdateDevice")
.WithTags("Devices")
.RequireAuthorization();

app.MapDelete("/device/{id}", (string id) =>
{
    var device = devices.FirstOrDefault(d => d.Identifier == id);
    if (device is null) return Results.NotFound();
    devices.Remove(device);
    return Results.Ok();
})
.WithName("DeleteDevice")
.WithTags("Devices")
.RequireAuthorization();

app.MapPost("/auth/login", (LoginRequest loginRequest) =>
{
    // Authentication logic
    if (loginRequest.Username == "admin" && loginRequest.Password == "admin")
    {
        var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);
        return Results.Ok(new { token = tokenString });
    }
    static byte[] GenerateRandomBytes(int length)
    {
        using (var rng = new RNGCryptoServiceProvider())
        {
            byte[] randomBytes = new byte[length];
            rng.GetBytes(randomBytes);
            return randomBytes;
        }
    }
    return Results.Unauthorized();
});

app.Run();

