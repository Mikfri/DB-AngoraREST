using DB_AngoraLib.EF_DbContext;
using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
using DB_AngoraLib.Services.AccountService;
using DB_AngoraLib.Services.ApplicationServices;
using DB_AngoraLib.Services.EmailService;
using DB_AngoraLib.Services.RabbitService;
using DB_AngoraLib.Services.RoleService;
using DB_AngoraLib.Services.SigninService;
using DB_AngoraLib.Services.TokenService;
using DB_AngoraLib.Services.TransferService;
using DB_AngoraLib.Services.ValidationService;
using DB_AngoraREST.DB_DataStarter;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//-----------------: DB-AngoraLib Services
builder.Services.AddScoped<IGRepository<Rabbit>, GRepository<Rabbit>>();
builder.Services.AddScoped<IRabbitService, RabbitServices>();
builder.Services.AddScoped<IGRepository<User>, GRepository<User>>();
builder.Services.AddScoped<IAccountService, AccountServices>();
builder.Services.AddScoped<IGRepository<ApplicationBreeder>, GRepository<ApplicationBreeder>>();
builder.Services.AddScoped<IApplicationService, ApplicationServices>();
builder.Services.AddScoped<IGRepository<TransferRequst>, GRepository<TransferRequst>>();
builder.Services.AddScoped<ITransferService, TransferServices>();
builder.Services.AddScoped<IGRepository<RefreshToken>, GRepository<RefreshToken>>();
builder.Services.AddScoped<ITokenService, TokenServices>();
builder.Services.AddScoped<IGRepository<Notification>, GRepository<Notification>>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddTransient<IEmailService, EmailServices>();

builder.Services.AddScoped<Rabbit_Validator>();

// Mine Lib IdentityUser services
builder.Services.AddScoped<ISigninService, SigninServices>();
builder.Services.AddScoped<IRoleService, RoleServices>();

// Bind EmailSettings fra appsettings.json
builder.Services.Configure<Settings_Email>(builder.Configuration.GetSection("EmailSettings"));

// Registrer EmailService med dependency injection
//builder.Services.AddTransient<IEmailService, EmailService>();

builder.Services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache

builder.Services.AddControllers();

/// Du kan tilføje en betingelse for at skifte mellem forbindelsesstrengene for eksempel,
/// baseret på en miljøvariabel eller en konfigurationsværdi
var connectionStringName = "DefaultConnection";
//if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
//{
//    connectionStringName = "SecretConnection";
//}

// -----------------: DB CONNECTION-STRING & MIGRATION SETUP
builder.Services.AddDbContext<DB_AngoraContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString(connectionStringName),
    b => b.MigrationsAssembly("DB-AngoraREST")));


//------------------: IDENTITY
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<DB_AngoraContext>()
    .AddDefaultTokenProviders()
    .AddSignInManager()
    .AddRoles<IdentityRole>();

//------------------: JWT
//--------: Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    ///Når en uautoriseret anmodning modtages, vil brugeren blive omdirigeret til Google's login side.
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    //options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
})
// Dette er Googles OAuth2.0 config, så "AddOAuth("OAuth", options =>" er ikke nødvendig
.AddGoogle(googleOptions => 
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    googleOptions.CallbackPath = "/signin-google";
    //googleOptions.CallbackPath = "https://localhost:7276/signin-google";  // Forkert?
    googleOptions.Scope.Add("https://www.googleapis.com/auth/userinfo.email");
    googleOptions.Scope.Add("https://www.googleapis.com/auth/userinfo.profile");
    googleOptions.Scope.Add("openid");
});


//builder.Services.AddAuthentication()


//--------: Authorization
builder.Services.AddAuthorization(options =>
{
    //-----------------: RABBIT POLICIES
    options.AddPolicy("UpdateRabbit", policy =>
    policy.RequireAssertion(context =>
        context.User.HasClaim("Rabbit:Update", "Own") ||
        context.User.HasClaim("Rabbit:Update", "Any")));

    options.AddPolicy("DeleteRabbit", policy =>
    policy.RequireAssertion(context =>
        context.User.HasClaim("Rabbit:Delete", "Own") ||
        context.User.HasClaim("Rabbit:Delete", "Any")));
});

//--------------------: SWAGGER
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer(); // Swagger API-dokumentation (///<summary> over dine API end-points vises i UI)

//--------: Google.OAuth2, Authentication UI
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "DB-AngoraREST API", Version = "v1" });

    // Definerer OAuth2.0 flowet for Swagger UI
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            Implicit = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri("https://accounts.google.com/o/oauth2/v2/auth"),
                Scopes = new Dictionary<string, string>
                {
                    { "https://www.googleapis.com/auth/userinfo.email", "Email" },
                    { "https://www.googleapis.com/auth/userinfo.profile", "Profile" },
                    { "openid", "OpenID" }
                }
            }
        }
    });

    // Tilføjer OAuth2.0 sikkerhedskrav til Swagger UI, så det kan bruge det definerede flow
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                },
                Scheme = "oauth2",
                Name = "oauth2",
                In = ParameterLocation.Header
            },
            new List<string>() // Scopes her, hvis nødvendigt
        }
    });
});

//--------: JWT, Authentication UI
//builder.Services.AddSwaggerGen(options =>
//{
//    options.AddSecurityDefinition("Oauth2", new OpenApiSecurityScheme
//    {
//        In = ParameterLocation.Header,
//        Name = "Authorization",
//        Type = SecuritySchemeType.Http, // .ApiKey,
//        //Type = SecuritySchemeType.OAuth2,
//        Scheme = "Bearer"
//    });
//    options.OperationFilter<SecurityRequirementsOperationFilter>(true, "Oauth2");

//});


//--------: JSON ENUM CONVERTER
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve; // Sørger for at refence-loop kan håndteres, som er tilfældet for Rabbit_PedigreeDTO

});


var app = builder.Build();

//-----------------: DB-INITIALIZER setup
// Get the service scope factory
var serviceScopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
// Create a new scope
using (var scope = serviceScopeFactory.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DB_AngoraContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // Initialize the database
    DbInitializer.Initialize(context, userManager, roleManager);
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.UseAuthentication();    // IdentityUser setup
app.UseAuthorization();

app.MapControllers();

app.Run();