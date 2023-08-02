﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Store.Context;
using System.Security.Claims;
using System.Text;

namespace Tests.Controllers
{
    public abstract class BaseController : WebApplicationFactory<Program>
    {
        protected HttpClient Client { get; }
        protected AppDbContext Context { get; }

        public BaseController(HttpClient client)
        {
            Client = client;
        }

        protected BaseController()
        {
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IAuthenticationSchemeProvider));
                if (descriptor == null)
                {
                    // Add the mocked logger
                    var loggerMock = new Mock<ILogger<Program>>();
                    services.AddSingleton(loggerMock.Object);

                    services.AddDbContext<AppDbContext>(options =>
                        options.UseInMemoryDatabase(databaseName: "IntegrationTestDb"), ServiceLifetime.Singleton);


                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    }).AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = false,
                            ValidateAudience = false,
                            ValidateLifetime = false,
                            ValidateIssuerSigningKey = false,
                            ValidIssuer = "Test",
                            ValidAudience = "Test",
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("A key used for tests"))
                        };
                    });
                }
            });

            builder.Configure(app =>
            {
                app.UseAuthentication();
                app.UseMiddleware<TestAuthenticationMiddleware>();
            });
        }

        public class TestAuthenticationMiddleware
        {
            private readonly RequestDelegate _next;

            public TestAuthenticationMiddleware(RequestDelegate next)
            {
                _next = next;
            }

            public async Task InvokeAsync(HttpContext context)
            {
                var testIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "TestUser") }, JwtBearerDefaults.AuthenticationScheme);
                var testPrincipal = new ClaimsPrincipal(testIdentity);
                context.User = testPrincipal;

                await _next(context);
            }
        }
    }
}