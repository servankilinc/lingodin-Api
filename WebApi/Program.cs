using Autofac.Extensions.DependencyInjection;
using Autofac;
using Business;
using Core;
using Core.Utils.Auth;
using DataAccess;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using System.Threading.RateLimiting;
using WebApi.GlobalExceptionHandler;
using Model;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//options.AddPolicy("AllowOrigin", builder => builder.WithOrigins().AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder => builder
        //.WithHeaders("content-type", "authorization")
        //.WithHeaders("source-client", "from-oneday")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin());
});


// ----------------------------- Rate Limiter Implementation -----------------------------
var _policyName = "sliding";
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddSlidingWindowLimiter(policyName: _policyName, slidingOptions =>
    {
        slidingOptions.PermitLimit = 10;
        slidingOptions.Window = TimeSpan.FromSeconds(15);
        slidingOptions.SegmentsPerWindow = 4;
        slidingOptions.QueueLimit = 2;
        slidingOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
});



// ----------------------------- JWT Implementation -----------------------------
TokenOptions tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<TokenOptions>()!;
builder.Services.AddSingleton<TokenOptions>(tokenOptions);
builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidIssuer = tokenOptions.Issuer,
        ValidAudience = tokenOptions.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.SecurityKey))
    };
});



// ----------------------------- Global Exception Handler Injection -----------------------------
builder.Services.AddExceptionHandler<ExceptionHandler>();
builder.Services.AddProblemDetails();



// ----------------------------- Logger Injection -----------------------------
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Host.UseSerilog();



// ----------------------------- Service Registrations -----------------------------
builder.Services.AddCoreServices(builder.Configuration);
builder.Services.AddModelServices();
builder.Services.AddDataAccessServices(builder.Configuration);
builder.Services.AddBussinessServices(builder.Configuration);



// ----------------------------- Autofac Module Injections -----------------------------
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(builder =>
    {
        builder.RegisterModule(new Core.AutofacModule());
        builder.RegisterModule(new DataAccess.AutofacModule());
        builder.RegisterModule(new Business.AutofacModule());
    }
);

builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

var app = builder.Build();

app.UseStaticFiles();


app.UseSwagger();
app.UseSwaggerUI();

//app.ApplyMigrations();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandler();

//app.UseSerilogRequestLogging();

app.UseRateLimiter();

app.MapControllers().RequireRateLimiting(_policyName);

app.Run();
