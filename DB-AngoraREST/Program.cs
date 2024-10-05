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
using Microsoft.AspNetCore.Authentication;
using Azure.Identity;
using Microsoft.AspNetCore.CookiePolicy;
using DB_AngoraLib.Services.BreederBrandService;
using DB_AngoraLib.Services.BreederService;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
//-----------------: DB-AngoraLib Services
builder.Services.AddScoped<IGRepository<Rabbit>, GRepository<Rabbit>>();
builder.Services.AddScoped<IRabbitService, RabbitServices>();
builder.Services.AddScoped<IGRepository<User>, GRepository<User>>();
builder.Services.AddScoped<IAccountService, AccountServices>();
builder.Services.AddScoped<IBreederService, BreederServices>();
builder.Services.AddScoped<IGRepository<ApplicationBreeder>, GRepository<ApplicationBreeder>>();
builder.Services.AddScoped<IApplicationService, ApplicationServices>();
builder.Services.AddScoped<IGRepository<TransferRequst>, GRepository<TransferRequst>>();
builder.Services.AddScoped<ITransferService, TransferServices>();

builder.Services.AddScoped<IGRepository<BreederBrand>, GRepository<BreederBrand>>();
builder.Services.AddScoped<IBreederBrandService, BreederBrandServices>();

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

builder.Services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache


/// Du kan tilf�je en betingelse for at skifte mellem forbindelsesstrengene for eksempel,
/// baseret p� en milj�variabel eller en konfigurationsv�rdi
//var connectionStringName = "DefaultConnection";  // "DefaultConnection" "SecretConnection"
//if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
//{
//    connectionStringName = "SecretConnection";
//}

// -----------------: DB CONNECTION-STRING & MIGRATION SETUP
builder.Services.AddDbContext<DB_AngoraContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    b => b.MigrationsAssembly("DB-AngoraREST")));


//------------------: IDENTITY
builder.Services.AddIdentity<User, IdentityRole>(
    //options =>
    //{
    //    options.ClaimsIdentity.UserNameClaimType = "UserID";
    //}
    )
    .AddEntityFrameworkStores<DB_AngoraContext>()
    .AddDefaultTokenProviders();
//.AddTokenProvider(authSettings.TokenProviderName, typeof(DataProtectorTokenProvider<User>));
//.AddSignInManager()
//.AddRoles<IdentityRole>();



builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    ///N�r en uautoriseret anmodning modtages, vil brugeren blive omdirigeret til Google's login side.
    //options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

})
// Dette er Googles OAuth2.0 config,
//.AddGoogle(googleOptions =>   // validerer automatisk "ya29." tokenet fra Google
//{
//    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
//    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
//    //googleOptions.CallbackPath = new PathString("/api/auth/signin-google");
//    googleOptions.CallbackPath = "/signin-google";
//    //googleOptions.CallbackPath = builder.Configuration["Authentication:Google:CallbackPath"];
//    Console.WriteLine($"Google CallbackPath: {googleOptions.CallbackPath}");

//    //googleOptions.SaveTokens = true;    // Hvad g�r denne? - Gemmer tokens i cookie?
//})
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
});



//--------: Authorization
builder.Services.AddAuthorization(options =>
{

    //-----------------: USER POLICIES
    options.AddPolicy("ReadUser", policy =>
    policy.RequireAssertion(context =>
        context.User.HasClaim("User:Read", "Own") ||
        context.User.HasClaim("User:Read", "Any")));

    options.AddPolicy("UpdateUser", policy =>
    policy.RequireAssertion(context =>
        context.User.HasClaim("User:Update", "Own") ||
        context.User.HasClaim("User:Update", "Any")));

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

//--------: JSON ENUM CONVERTER
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve; // S�rger for at refence-loop kan h�ndteres, som er tilf�ldet for Rabbit_PedigreeDTO

});

builder.Services.AddEndpointsApiExplorer(); // Swagger API-dokumentation (///<summary> over dine API end-points vises i UI)

//--------------------: SWAGGER
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//--------: Google.OAuth2, Authentication UI
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "DB-AngoraREST API", Version = "v1" });

    // Tilføj Google OAuth2 konfiguration
    //options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    //{
    //    Type = SecuritySchemeType.OAuth2,
    //    Flows = new OpenApiOAuthFlows
    //    {
    //        AuthorizationCode = new OpenApiOAuthFlow
    //        {
    //            AuthorizationUrl = new Uri("https://accounts.google.com/o/oauth2/v2/auth"),
    //            TokenUrl = new Uri("https://oauth2.googleapis.com/token"),  // Dette er Google's token-endpoint som bruges til at validere tokenet
    //            Scopes = new Dictionary<string, string>
    //            {
    //                { "openid", "OpenID" },
    //                { "profile", "Profile" },
    //                { "email", "Email" }
    //            }
    //        }
    //    }
    //});
    // Tilføj JWT Bearer konfiguration
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });
    // Tilføj sikkerhedskrav for OAuth2 og Bearer-token
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        // Google OAuth2
        //{
        //    new OpenApiSecurityScheme
        //    {
        //        Reference = new OpenApiReference
        //        {
        //            Type = ReferenceType.SecurityScheme,
        //            Id = "oauth2"
        //        },
        //        Scheme = "oauth2",
        //        Name = "oauth2",
        //        In = ParameterLocation.Header,
        //    },
        //    new List<string>() // Scopes her, hvis nødvendigt
        //},
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});


//--------: Konfigurer CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "MyAllowSpecificOrigins",
    policy =>
    {
        policy.WithOrigins("https://localhost:7276") // Erstat med den korrekte oprindelse for Swagger UI
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials(); // Tillad credentials såsom cookies, autorisation headers eller TLS klient certifikater
    });
});

var app = builder.Build();

//-----------------: DB-INITIALIZER setup // UDKOMENTER n�r der skal laves DGML
// Get the service scope factory

var serviceScopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using (var scope = serviceScopeFactory.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DB_AngoraContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    DbInitializer.Initialize(context, userManager, roleManager);
}

app.UseCors("MyAllowSpecificOrigins");
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