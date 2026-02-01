using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Logging;
using System.Text;
using CreditCardManagement.API.Data;
using CreditCardManagement.API.Services;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Credit Card Management API", 
        Version = "v1",
        Description = "API for managing credit cards with secure authentication"
    });
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
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
            Array.Empty<string>()
        }
    });
});

// Database - Using SQLite for easier setup (can switch to SQL Server by changing connection string)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (connectionString?.Contains("SQLite") == true || connectionString?.Contains("Data Source") == true)
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
}
else
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null
            )));
}

// Data Protection pour OAuth state management - DOIT être configuré AVANT Authentication
var dataProtectionKeysPath = Path.Combine(builder.Environment.ContentRootPath, "DataProtection-Keys");
Directory.CreateDirectory(dataProtectionKeysPath);
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionKeysPath))
    .SetApplicationName("CreditCardManagement")
    .SetDefaultKeyLifetime(TimeSpan.FromDays(90));

// Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyThatShouldBeAtLeast32CharactersLong!";

var oauthSettings = builder.Configuration.GetSection("OAuth");
var googleClientId = oauthSettings["Google:ClientId"];
var googleClientSecret = oauthSettings["Google:ClientSecret"];
var facebookAppId = oauthSettings["Facebook:AppId"];
var facebookAppSecret = oauthSettings["Facebook:AppSecret"];

var authBuilder = builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
        options.Cookie.HttpOnly = true;
        // Configuration des cookies selon l'environnement
        // En développement HTTP : Lax + None (pas besoin de Secure)
        // En production HTTPS : None + Always (pour cross-site OAuth)
        if (builder.Environment.IsDevelopment())
        {
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.SecurePolicy = CookieSecurePolicy.None;
        }
        else
        {
            options.Cookie.SameSite = SameSiteMode.None;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        }
        options.Cookie.Name = ".AspNetCore.Cookies";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
        // Augmenter le temps d'expiration pour les tickets OAuth
        options.Events.OnSigningIn = async context =>
        {
            // Augmenter la durée de vie du cookie pour OAuth (10 minutes pour être sûr)
            context.Properties.ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10);
            // S'assurer que le cookie est persistant
            context.Properties.IsPersistent = true;
            await Task.CompletedTask;
        };
        
        // OnSignedIn est appelé APRÈS que le cookie soit sauvegardé dans la réponse
        // C'est le bon moment pour rediriger vers notre endpoint personnalisé
        options.Events.OnSignedIn = async context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            var path = context.HttpContext.Request.Path.Value?.ToLower();
            logger.LogInformation("OnSignedIn called for path: {Path} - Cookie is now saved", path);
            
            // Vérifier si c'est une requête OAuth Google
            if (path?.Contains("/signin-google") == true)
            {
                logger.LogInformation("Redirecting to google-callback-success after cookie is saved");
                var backendUrl = $"{context.HttpContext.Request.Scheme}://{context.HttpContext.Request.Host}";
                context.HttpContext.Response.Redirect($"{backendUrl}/api/OAuth/google-callback-success");
            }
            // Vérifier si c'est une requête OAuth Facebook
            else if (path?.Contains("/signin-facebook") == true)
            {
                logger.LogInformation("Redirecting to facebook-callback-success after cookie is saved");
                var backendUrl = $"{context.HttpContext.Request.Scheme}://{context.HttpContext.Request.Host}";
                context.HttpContext.Response.Redirect($"{backendUrl}/api/OAuth/facebook-callback-success");
            }
            
            await Task.CompletedTask;
        };
    options.LoginPath = "/api/oauth/google";
    options.LogoutPath = "/api/auth/logout";
    // Important: permettre la persistance de l'état OAuth
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    };
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"] ?? "CreditCardManagement",
        ValidAudience = jwtSettings["Audience"] ?? "CreditCardManagementUsers",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
})
.AddGoogle(options =>
{
    // Utiliser les credentials de la configuration, même s'ils sont des placeholders
    // Cela permet à OAuth de se configurer et de retourner une erreur appropriée si les credentials ne sont pas valides
    options.ClientId = googleClientId ?? "YOUR_GOOGLE_CLIENT_ID";
    options.ClientSecret = googleClientSecret ?? "YOUR_GOOGLE_CLIENT_SECRET";
    options.CallbackPath = "/signin-google";
    options.SaveTokens = true;
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    
    // Configurer les scopes Google (email et profile sont demandés par défaut, mais on les spécifie explicitement)
    options.Scope.Add("email");
    options.Scope.Add("profile");
    
    // Rediriger vers notre endpoint après que le ticket soit reçu
    // Le cookie sera écrit par SignInScheme, puis on redirige
    options.Events.OnTicketReceived = async context =>
    {
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("OnTicketReceived called for Google OAuth - cookie will be written by SignInScheme");
        
        // Ne PAS utiliser HandleResponse() ici car cela empêche le cookie d'être écrit
        // On va utiliser OnSignedIn pour rediriger après que le cookie soit écrit
        await Task.CompletedTask;
    };
    
    // Gérer les erreurs OAuth
    options.Events.OnRemoteFailure = async context =>
    {
        var frontendUrl = builder.Configuration["FrontendUrl"] ?? "http://localhost:5173";
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError("Google OAuth failed: {Error}", context.Failure?.Message);
        context.Response.Redirect($"{frontendUrl}/login?error=google_auth_failed");
        context.HandleResponse();
        await Task.CompletedTask;
    };
})
.AddFacebook(options =>
{
    // Utiliser les credentials de la configuration, même s'ils sont des placeholders
    // Cela permet à OAuth de se configurer et de retourner une erreur appropriée si les credentials ne sont pas valides
    options.AppId = facebookAppId ?? "YOUR_FACEBOOK_APP_ID";
    options.AppSecret = facebookAppSecret ?? "YOUR_FACEBOOK_APP_SECRET";
    // Utiliser le callback path par défaut du middleware OAuth (comme pour Google)
    options.CallbackPath = "/signin-facebook";
    
    // Ne PAS ajouter de scopes explicitement - Facebook utilise public_profile par défaut
    // L'ajout explicite de scopes peut causer des erreurs si le scope n'est pas approuvé
    // On laisse Facebook utiliser les scopes par défaut (public_profile uniquement)
    // Si vous avez besoin de l'email, configurez-le dans le Facebook Developer Dashboard
    // L'email sera récupéré automatiquement via les claims si la permission est approuvée
    
    // Sauvegarder les tokens
    options.SaveTokens = true;
    // Utiliser le même cookie scheme pour l'état OAuth
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    // Rediriger vers notre endpoint personnalisé après que le ticket soit reçu
    // Ne PAS rediriger dans OnTicketReceived
    // Laisser le middleware OAuth gérer le callback normalement
    // Le cookie sera écrit, puis on interceptera /signin-facebook avec un endpoint
    options.Events.OnTicketReceived = async context =>
    {
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("OnTicketReceived called for Facebook OAuth - cookie will be written by SignInScheme");
        await Task.CompletedTask;
    };
    // Gérer les erreurs
    options.Events.OnRemoteFailure = async context =>
    {
        var frontendUrl = builder.Configuration["FrontendUrl"] ?? "http://localhost:5173";
        context.Response.Redirect($"{frontendUrl}/login?error=facebook_auth_failed");
        context.HandleResponse();
        await Task.CompletedTask;
    };
});

builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICreditCardService, CreditCardService>();
builder.Services.AddScoped<ICardValidationService, CardValidationService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IWhatsAppService, WhatsAppService>();
builder.Services.AddScoped<IBankTransferService, BankTransferService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IStatementService, StatementService>();
builder.Services.AddScoped<ILoanService, LoanService>();

// FluentValidation
builder.Services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Program>());

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// IMPORTANT: CORS doit être avant Authentication pour OAuth
app.UseCors("AllowVueApp");

// Désactiver HTTPS redirection en développement pour éviter les problèmes avec HTTP local
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

// Route racine pour afficher les informations de l'API
app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapControllers();

// Ensure database is created with retry logic
// IMPORTANT: Ne PAS supprimer la base de données pour conserver les données utilisateurs
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    var maxRetries = 5;
    var retryDelay = TimeSpan.FromSeconds(2);
    
    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            logger.LogInformation("Attempting to connect to database (attempt {Attempt}/{MaxRetries})...", i + 1, maxRetries);
            
            // Vérifier si la base de données existe
            var canConnect = await dbContext.Database.CanConnectAsync();
            
            if (!canConnect)
            {
                // Créer la base de données si elle n'existe pas
                await dbContext.Database.EnsureCreatedAsync();
                logger.LogInformation("Database created successfully.");
            }
            else
            {
                // La base existe, vérifier et créer les tables manquantes
                // Cette approche préserve les données existantes
                try
                {
                    // Toujours essayer de créer les tables manquantes (elles seront créées seulement si elles n'existent pas)
                    logger.LogInformation("Checking and creating missing tables (BankTransfers, Payments, Statements, Loans)...");
                    await CreateMissingTablesAsync(dbContext, logger);
                    logger.LogInformation("Table check/creation completed.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error creating missing tables: {Error}", ex.Message);
                    // Ne pas échouer complètement, juste logger l'erreur
                    // L'utilisateur peut créer les tables manuellement si nécessaire
                }
            }
            
            logger.LogInformation("Database connection successful! Schema is up to date. Data is preserved.");
            break;
        }
        catch (Exception ex)
        {
            logger.LogWarning("Database connection attempt {Attempt} failed: {Error}", i + 1, ex.Message);
            if (i == maxRetries - 1)
            {
                logger.LogError("Failed to connect to database after {MaxRetries} attempts. Please check your SQL Server connection.", maxRetries);
                throw;
            }
            Thread.Sleep(retryDelay);
        }
    }
}

// Helper method to create missing tables
static async Task CreateMissingTablesAsync(ApplicationDbContext dbContext, ILogger logger)
{
    try
    {
        var connection = dbContext.Database.GetDbConnection();
        await connection.OpenAsync();
        
        try
        {
            using var command = connection.CreateCommand();
            
            // Créer BankTransfers si elle n'existe pas
            command.CommandText = @"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'BankTransfers')
                BEGIN
                    CREATE TABLE [BankTransfers] (
                        [Id] uniqueidentifier NOT NULL,
                        [UserId] uniqueidentifier NOT NULL,
                        [FromAccount] nvarchar(100) NOT NULL,
                        [ToAccount] nvarchar(100) NOT NULL,
                        [BeneficiaryName] nvarchar(200) NOT NULL,
                        [Amount] decimal(18,2) NOT NULL,
                        [Currency] nvarchar(3) NOT NULL,
                        [Description] nvarchar(500) NULL,
                        [TransferType] nvarchar(50) NOT NULL,
                        [TransferDate] datetime2 NOT NULL,
                        [Status] nvarchar(50) NOT NULL,
                        [CreatedAt] datetime2 NOT NULL,
                        [UpdatedAt] datetime2 NOT NULL,
                        CONSTRAINT [PK_BankTransfers] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_BankTransfers_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
                    );
                    CREATE INDEX [IX_BankTransfers_UserId] ON [BankTransfers] ([UserId]);
                    CREATE INDEX [IX_BankTransfers_TransferDate] ON [BankTransfers] ([TransferDate]);
                    CREATE INDEX [IX_BankTransfers_Status] ON [BankTransfers] ([Status]);
                END
            ";
            await command.ExecuteNonQueryAsync();
            logger.LogInformation("BankTransfers table checked/created.");

            // Créer Payments si elle n'existe pas
            command.CommandText = @"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Payments')
                BEGIN
                    CREATE TABLE [Payments] (
                        [Id] uniqueidentifier NOT NULL,
                        [UserId] uniqueidentifier NOT NULL,
                        [CreditCardId] uniqueidentifier NULL,
                        [PaymentType] nvarchar(100) NOT NULL,
                        [MerchantName] nvarchar(200) NOT NULL,
                        [ReferenceNumber] nvarchar(100) NOT NULL,
                        [Amount] decimal(18,2) NOT NULL,
                        [Currency] nvarchar(3) NOT NULL,
                        [Description] nvarchar(500) NULL,
                        [PaymentDate] datetime2 NOT NULL,
                        [Status] nvarchar(50) NOT NULL,
                        [ReceiptNumber] nvarchar(100) NULL,
                        [CreatedAt] datetime2 NOT NULL,
                        [UpdatedAt] datetime2 NOT NULL,
                        CONSTRAINT [PK_Payments] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_Payments_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
                        CONSTRAINT [FK_Payments_CreditCards_CreditCardId] FOREIGN KEY ([CreditCardId]) REFERENCES [CreditCards] ([Id]) ON DELETE SET NULL
                    );
                    CREATE INDEX [IX_Payments_UserId] ON [Payments] ([UserId]);
                    CREATE INDEX [IX_Payments_CreditCardId] ON [Payments] ([CreditCardId]);
                    CREATE INDEX [IX_Payments_PaymentDate] ON [Payments] ([PaymentDate]);
                    CREATE INDEX [IX_Payments_Status] ON [Payments] ([Status]);
                    CREATE INDEX [IX_Payments_PaymentType] ON [Payments] ([PaymentType]);
                END
            ";
            await command.ExecuteNonQueryAsync();
            logger.LogInformation("Payments table checked/created.");

            // Créer Statements si elle n'existe pas
            command.CommandText = @"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Statements')
                BEGIN
                    CREATE TABLE [Statements] (
                        [Id] uniqueidentifier NOT NULL,
                        [UserId] uniqueidentifier NOT NULL,
                        [CreditCardId] uniqueidentifier NOT NULL,
                        [StatementType] nvarchar(50) NOT NULL,
                        [StartDate] datetime2 NOT NULL,
                        [EndDate] datetime2 NOT NULL,
                        [OpeningBalance] decimal(18,2) NOT NULL,
                        [ClosingBalance] decimal(18,2) NOT NULL,
                        [TotalCredits] decimal(18,2) NOT NULL,
                        [TotalDebits] decimal(18,2) NOT NULL,
                        [TransactionCount] int NOT NULL,
                        [Notes] nvarchar(500) NULL,
                        [GeneratedAt] datetime2 NOT NULL,
                        [CreatedAt] datetime2 NOT NULL,
                        CONSTRAINT [PK_Statements] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_Statements_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
                        CONSTRAINT [FK_Statements_CreditCards_CreditCardId] FOREIGN KEY ([CreditCardId]) REFERENCES [CreditCards] ([Id]) ON DELETE NO ACTION
                    );
                    CREATE INDEX [IX_Statements_UserId] ON [Statements] ([UserId]);
                    CREATE INDEX [IX_Statements_CreditCardId] ON [Statements] ([CreditCardId]);
                    CREATE INDEX [IX_Statements_StartDate] ON [Statements] ([StartDate]);
                    CREATE INDEX [IX_Statements_EndDate] ON [Statements] ([EndDate]);
                END
            ";
            await command.ExecuteNonQueryAsync();
            logger.LogInformation("Statements table checked/created.");

            // Créer Loans si elle n'existe pas
            command.CommandText = @"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Loans')
                BEGIN
                    CREATE TABLE [Loans] (
                        [Id] uniqueidentifier NOT NULL,
                        [UserId] uniqueidentifier NOT NULL,
                        [LoanType] nvarchar(100) NOT NULL,
                        [LoanName] nvarchar(200) NOT NULL,
                        [PrincipalAmount] decimal(18,2) NOT NULL,
                        [RemainingAmount] decimal(18,2) NOT NULL,
                        [InterestRate] decimal(5,2) NOT NULL,
                        [TermMonths] int NOT NULL,
                        [RemainingMonths] int NOT NULL,
                        [MonthlyPayment] decimal(18,2) NOT NULL,
                        [StartDate] datetime2 NOT NULL,
                        [NextPaymentDate] datetime2 NOT NULL,
                        [Status] nvarchar(50) NOT NULL,
                        [Description] nvarchar(500) NULL,
                        [CreatedAt] datetime2 NOT NULL,
                        [UpdatedAt] datetime2 NOT NULL,
                        CONSTRAINT [PK_Loans] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_Loans_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
                    );
                    CREATE INDEX [IX_Loans_UserId] ON [Loans] ([UserId]);
                    CREATE INDEX [IX_Loans_Status] ON [Loans] ([Status]);
                    CREATE INDEX [IX_Loans_NextPaymentDate] ON [Loans] ([NextPaymentDate]);
                END
            ";
            await command.ExecuteNonQueryAsync();
            logger.LogInformation("Loans table checked/created.");

            // Ajouter la colonne ConfirmationCode à CreditCards si elle n'existe pas
            command.CommandText = @"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                               WHERE TABLE_NAME = 'CreditCards' AND COLUMN_NAME = 'ConfirmationCode')
                BEGIN
                    ALTER TABLE [CreditCards] ADD [ConfirmationCode] nvarchar(10) NULL;
                END
            ";
            await command.ExecuteNonQueryAsync();
            logger.LogInformation("CreditCards.ConfirmationCode column checked/added.");

            // Ajouter la colonne ConfirmationCode à Transactions si elle n'existe pas
            command.CommandText = @"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                               WHERE TABLE_NAME = 'Transactions' AND COLUMN_NAME = 'ConfirmationCode')
                BEGIN
                    ALTER TABLE [Transactions] ADD [ConfirmationCode] nvarchar(10) NULL;
                END
            ";
            await command.ExecuteNonQueryAsync();
            logger.LogInformation("Transactions.ConfirmationCode column checked/added.");

            // Ajouter la colonne IsActive à CreditCards si elle n'existe pas
            command.CommandText = @"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                               WHERE TABLE_NAME = 'CreditCards' AND COLUMN_NAME = 'IsActive')
                BEGIN
                    ALTER TABLE [CreditCards] ADD [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit);
                END
            ";
            await command.ExecuteNonQueryAsync();
            logger.LogInformation("CreditCards.IsActive column checked/added.");
        }
        finally
        {
            await connection.CloseAsync();
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error creating missing tables: {Error}", ex.Message);
        throw;
    }
}

app.Run();

