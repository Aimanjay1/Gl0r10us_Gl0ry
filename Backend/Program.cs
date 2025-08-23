using BizOpsAPI.Data;
using Microsoft.EntityFrameworkCore;
using BizOpsAPI.Repositories;
using BizOpsAPI.Mappings;
using BizOpsAPI.Helpers;
using BizOpsAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Http.Features;
using Supabase;
using QuestPDF.Infrastructure; // ✅ NEW

var builder = WebApplication.CreateBuilder(args);

var raw = builder.Configuration.GetConnectionString("DefaultConnection") ?? "<null>";
try
{
    var csb = new Npgsql.NpgsqlConnectionStringBuilder(raw);
    Console.WriteLine($"[DB] Host={csb.Host}; Port={csb.Port}; Database={csb.Database}");
}
catch
{
    Console.WriteLine($"[DB] Raw connection string (couldn't parse): {raw}");
}

// Global Npgsql behavior (use modern timestamp semantics)
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", false);

// ===== Controllers & Swagger =====
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "BizOpsAPI", Version = "v1" });

    // Make Swagger use same origin/scheme
    c.AddServer(new OpenApiServer { Url = "/" });

    // Bearer scheme
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Paste the raw JWT (no 'Bearer ' prefix).",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
}); // <-- keep this semicolon

// ===== DbContext =====
// Tip: In your connection string, prefer PgBouncer port 6543 and add:
// "PreferSimpleProtocol=true; Command Timeout=30; Maximum Pool Size=50"
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npg =>
        {
            // Transient fault handling + command timeout
            npg.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(2),
                errorCodesToAdd: null);
            npg.CommandTimeout(30);
        });

#if DEBUG
    options.EnableDetailedErrors()
           .EnableSensitiveDataLogging();
#endif
});

// ===== AutoMapper =====
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// ===== Repositories =====
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IInvoiceItemRepository, InvoiceItemRepository>();
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.AddScoped<IReceiptRepository, ReceiptRepository>();
builder.Services.AddScoped<IRevenueRepository, RevenueRepository>();

// ===== Services =====
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();   
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IReceiptService, ReceiptService>();
builder.Services.AddScoped<IRevenueService, RevenueService>();

// ===== Supabase Storage (NEW) =====
builder.Services.Configure<SupabaseSettings>(builder.Configuration.GetSection("Supabase"));

builder.Services.AddSingleton<Supabase.Client>(sp =>
{
    var cfg = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<SupabaseSettings>>().Value;
    var client = new Supabase.Client(cfg.Url, cfg.ServiceRoleKey, new SupabaseOptions
    {
        AutoConnectRealtime = false
    });
    // Initialize once so Storage is ready
    client.InitializeAsync().GetAwaiter().GetResult();
    return client;
});

// Use Supabase-backed file storage (disable LocalFileStorage)
builder.Services.AddSingleton<IFileStorage, SupabaseFileStorage>();

// Helper to turn stored paths into public/signed URLs
builder.Services.AddScoped<ReceiptLinkService>();

// ===== Current user accessor =====
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

// ===== Config binding =====
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// ===== Email ingestion (trigger-on-click) =====
builder.Services.Configure<EmailIngestionSettings>(builder.Configuration.GetSection("EmailIngestion"));
builder.Services.AddScoped<IEmailReceiptIngestionJob, EmailReceiptIngestionJob>();   

// (Optional) allow larger multipart uploads (e.g., 25 MB receipts)
builder.Services.Configure<FormOptions>(o =>
{
    o.MultipartBodyLengthLimit = 25 * 1024 * 1024; // 25 MB
});

// ===== JWT Auth =====
var jwt = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()
          ?? throw new InvalidOperationException("JwtSettings section is missing.");

if (string.IsNullOrWhiteSpace(jwt.Issuer) || string.IsNullOrWhiteSpace(jwt.Audience))
    throw new InvalidOperationException("JwtSettings invalid. Ensure Issuer and Audience are set.");

var signingKey = JwtKeyFactory.Create(jwt);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // set true in production behind HTTPS
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = jwt.Issuer,
        ValidAudience = jwt.Audience,
        IssuerSigningKey = signingKey,
        ClockSkew = TimeSpan.Zero
    };
});

// ===== CORS =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("Dev", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ✅ QuestPDF: set license mode once on startup
QuestPDF.Settings.License = LicenseType.Community;

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("Dev");

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/api/invoices/{id:int}/pdf", async (int id, IInvoiceService svc) =>
{
    var pdf = await svc.GenerateInvoicePdfAsync(id);
    var fileName = $"invoice_{id}.pdf";
    return Results.File(pdf, "application/pdf", fileName);
});

// Apply EF migrations at startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.MapGet("/healthz", () => "ok"); // Render healthcheck

app.Run();
