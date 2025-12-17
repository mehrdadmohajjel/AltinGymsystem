using Gym.Api.BackgroundJobs;
using Gym.Application.DTO.Wallet;
using Gym.Application.Events;
using Gym.Application.Security;
using Gym.Application.Services;
using Gym.Infrastructure.Notification;
using Gym.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Parbad.Builder;
using Parbad.Gateway.Mellat;
using Parbad.Gateway.ParbadVirtual;
using Parbad.Gateway.ZarinPal;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// -------------------- Controllers & Swagger --------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//------------------------------------------------------------

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    connectionString = builder.Configuration.GetConnectionString("GymDb");
    if (string.IsNullOrEmpty(connectionString))
    {
        // رشته اتصال پیش‌فرض
        connectionString = "Server=.;Database=AltinGymDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
    }
}
builder.Services.AddDbContext<GymDbContext>(options =>
    options.UseSqlServer(connectionString));


// -------------------- JWT Authentication --------------------
builder.Services.AddAuthentication("Bearer")
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
        )
    };
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddHostedService<ServiceExpirationJob>();
builder.Services.AddScoped<IMellatAccountResolver, MellatAccountResolver>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
builder.Services.AddScoped<INotificationService, KavenegarNotificationService>();


// -------------------- Payment Settings --------------------
// 👈 ثبت Strongly Typed Settings

// -------------------- Parbad Configuration --------------------
//builder.Services.AddParbad()
//    .ConfigureGateways(gateways =>
//    {
//        gateways.AddMellat();
//    })
//    .ConfigureHttpContext(http =>
//        http.UseDefaultAspNetCore())
//    .ConfigureStorage(storage =>
//        storage.UseMemoryCache());
builder.Services.AddParbad()
    .ConfigureGateways(gateway =>
    {
        // Configure ZarinPal Gateway
        gateway.AddZarinPal()
            .WithAccounts(accounts =>
            {
                var merchantId = builder.Configuration["Parbad:ZarinPal:MerchantId"];
                if (string.IsNullOrEmpty(merchantId))
                {
                    throw new InvalidOperationException("ZarinPal MerchantId is not configured in appsettings.json.");
                }

                accounts.AddInMemory(acc =>
                {
                    acc.MerchantId = merchantId;
                    acc.IsSandbox = builder.Configuration.GetValue<bool>("Parbad:ZarinPal:IsSandbox");
                });
            });

        // Configure Mellat Gateway
        gateway.AddMellat()
            .WithAccounts(source =>
            {
                var terminalId = builder.Configuration.GetValue<int>("Parbad:Mellat:TerminalId");
                var userName = builder.Configuration["Parbad:Mellat:UserName"];
                var userPassword = builder.Configuration["Parbad:Mellat:UserPassword"];

                if (terminalId == 0 || string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userPassword))
                {
                    throw new InvalidOperationException("Mellat gateway configuration is incomplete in appsettings.json.");
                }

                source.AddInMemory(account =>
                {
                    account.TerminalId = terminalId;
                    account.UserName = userName;
                    account.UserPassword = userPassword;
                });
            });
        gateway.AddParbadVirtual().WithOptions(options =>
        {
            options.GatewayPath = "/MyVirtual";
        });
    }).ConfigureHttpContext(httpContextBuilder =>
    {
        httpContextBuilder.UseDefaultAspNetCore();
    }).ConfigureStorage(sotragebuilder =>
    {
        sotragebuilder.UseMemoryCache();
    });

builder.Services.AddHttpContextAccessor();



// -------------------- Application Services --------------------

var app = builder.Build();

// -------------------- Middleware Pipeline --------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
