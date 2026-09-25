var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();  
builder.Services.AddAuthentication("Bearer") // bearer doðrulama
    .AddJwtBearer(options =>

    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters

        {
            ValidateIssuer = false,  // tokeni hangi sunucu verdi sorusu
            ValidateAudience = false, // bu kimlik hangi proje için sorus
            ValidateLifetime = true, // oturum süresi sorusu
            ValidateIssuerSigningKey = true, // dýþarýdan tokeni engeller sahte token kontrolü
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("uzungizlisifreafk1412f0kaf1039kmd")) // kullanýcýnýn imzalý bilgisini doðrulayan þifreleme anahtarý




        };
    });



builder.Services.AddCors(options =>
{
    options.AddPolicy("davetli kisiler" , policy => 
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();

    
    
    
    });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // 1. Swagger'a "Ben JWT Bearer kullanýyorum, sað üste bir kilit butonu çiz" diyoruz.
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Sistemin üreteceði Token'ý kopyalayýp buraya yapýþtýrýn."
    });

    // 2. Swagger'a "Bu kilit sistemini tüm API kapýlarýnda (metotlarda) aktif et" diyoruz.
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("davetli kisiler");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();