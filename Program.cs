using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using UserEntityWebAPI.Services;
using System.Text;

namespace UserEntityWebAPI
{
    public class Program
    {
        // TODO: � admin ������� ������. ���������, ��� �� �������� �� admin � �� ���������� ������������

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddSingleton<UserService>();

            builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                JwtOptions jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? throw new InvalidOperationException("JWT Options �� �������");

                // SSL ��� �������� ������������ �� ������������. � ���������� ����� ������������ true
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // ����� �� �������������� �������� ��� ��������� ������
                    ValidateIssuer = true,
                    // ������, �������������� ��������
                    ValidIssuer = jwtOptions.Issuer,

                    // ����� �� �������������� ����������� ������
                    ValidateAudience = true,
                    // ��������� ����������� ������
                    ValidAudience = jwtOptions.Audience,
                    // ����� �� �������������� ����� �������������
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,

                    // ��������� ����� ������������
                    ValidateIssuerSigningKey = true,
                    // ��������� ����� ������������
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key))
                };
            });

            builder.Services.AddAuthentication();

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
