    using System.IdentityModel.Tokens.Jwt;
    using System.Net.Http.Headers;
    using System.Security.Claims;
    using System.Text;
    using System.Text.Json;
    using CasinoGame.Facade.Models;
    using LobbyService;
    using LobbyService.Interfaces;
    using LobbyService.Services;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.IdentityModel.Tokens;

    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddSignalR();
    builder.Services.AddControllers();
    builder.Services.AddSingleton<IGameTableManager, GameTableManager>();

    // Add gamesettings.json to the configuration
    builder.Configuration.AddJsonFile("gamesettings.json", optional: false, reloadOnChange: true);
    
    var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
    var key = Encoding.ASCII.GetBytes(jwtSettings.Key);

    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new[] {
            new Claim("client", "LobbyService") 
        }),
        Expires = DateTime.UtcNow.AddHours(1),
        Issuer = jwtSettings.Issuer,
        Audience = jwtSettings.Audience,
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
    };

    var tokenHandler = new JwtSecurityTokenHandler();
    var token = tokenHandler.CreateToken(tokenDescriptor);
    var jwtToken = tokenHandler.WriteToken(token);

    builder.Services.AddHttpClient<IGameTableManager, GameTableManager>(client =>
    {
        client.BaseAddress = new Uri("http://walletservice:8080");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
    });
    
    builder.Services.Configure<GameSettings>(builder.Configuration);

    var gamesJson = File.ReadAllText("gamesettings.json");
    var gameSettings = JsonSerializer.Deserialize<GameSettings>(gamesJson, new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    });

    Console.WriteLine(JsonSerializer.Serialize(gameSettings));

    builder.Services.AddSignalR();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend", policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    });



    var app = builder.Build();
    
    app.UseCors("AllowFrontend");

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();


    app.MapHub<LobbyHub>("/lobbyhub");

    app.Run();