using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Seguridad_API.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//Add services to the container.
builder.Services.AddControllers();

//Configurar Automapper
builder.Services.AddAutoMapper(typeof(Program));

//DBContext
builder.Services.AddDbContext<RnpdnoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RNPDNOConnection")));

//Configurar JWTToken
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
   .AddJwtBearer(opciones => opciones.TokenValidationParameters = new TokenValidationParameters
   {
       ValidateIssuer = true,
       ValidateAudience = true,
       ValidateLifetime = true,
       ValidateIssuerSigningKey = true,
       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["jwtSecretKey"] ?? throw new InvalidOperationException("jwtSecretKey Not Found."))),
       ValidIssuer = builder.Configuration["jwtIssuer"],
       ValidAudience = builder.Configuration["jwtAudience"],
       ClockSkew = TimeSpan.Zero
   }
   );

//Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API_Seguridad v1"));
}

app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.Run();
