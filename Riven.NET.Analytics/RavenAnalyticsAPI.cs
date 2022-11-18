using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Riven.NET.Analytics
{
    public static class RavenAnalyticsApi
    {
        public static void AddRavenAnalytics(this IServiceCollection serviceCollection) 
        {
            var builder = WebApplication.CreateBuilder();
            builder.WebHost.UseUrls("https://*:6006");
            var app = builder.Build();

            app.MapGet("/analytics", RavensHandler);
            app.MapGet("/analytics/ravens", GetRavens);
            app.MapGet("/analytics/subjects", GetSubjects);

            app.Run();
        }

        private static IResult GetRavens(HttpContext context)
        {
            var ravens = new[]
            {
                new
                {
                    Name = "UserEventWatcher",
                    SubjectCount = 323,
                    CreatedAt = DateTime.Now.AddHours(-2),
                    UpdatedAt = DateTime.Now.AddMinutes(-5),
                },
                new
                {
                    Name = "DomainEventWatcher",
                    SubjectCount = 143,
                    CreatedAt = DateTime.Now.AddHours(-1).AddMilliseconds(323123),
                    UpdatedAt = DateTime.Now.AddMinutes(-2),
                },
                new
                {
                    Name = "ExceptionWatcher",
                    SubjectCount = 11,
                    CreatedAt = DateTime.Now.AddHours(-4).AddMilliseconds(323123),
                    UpdatedAt = DateTime.Now.AddMinutes(-87),
                },
                new
                {
                    Name = "AllObjectsWatcher",
                    SubjectCount = 39921,
                    CreatedAt = DateTime.Now.AddHours(-2).AddMilliseconds(3421),
                    UpdatedAt = DateTime.Now.AddMinutes(-2),
                },
                new
                {
                    Name = "CustomEventWatcher",
                    SubjectCount = 2,
                    CreatedAt = DateTime.Now.AddHours(-7).AddMilliseconds(564),
                    UpdatedAt = DateTime.Now.AddMinutes(-321),
                },
                new
                {
                    Name = "DummyTestWatcher",
                    SubjectCount = 1,
                    CreatedAt = DateTime.Now.AddHours(-2).AddMilliseconds(777777),
                    UpdatedAt = DateTime.Now.AddMinutes(-3),
                }
            };

            return Results.Ok(ravens);
        }
        
        private static IResult GetSubjects(HttpContext context)
        {
            var ravens = new[]
            {
                new
                {
                    Name = "UserEventWatcher",
                    SubjectCount = 323,
                    CreatedAt = DateTime.Now.AddHours(-2),
                    UpdatedAt = DateTime.Now.AddMinutes(-5),
                },
                new
                {
                    Name = "DomainEventWatcher",
                    SubjectCount = 143,
                    CreatedAt = DateTime.Now.AddHours(-1).AddMilliseconds(323123),
                    UpdatedAt = DateTime.Now.AddMinutes(-2),
                },
                new
                {
                    Name = "ExceptionWatcher",
                    SubjectCount = 11,
                    CreatedAt = DateTime.Now.AddHours(-4).AddMilliseconds(323123),
                    UpdatedAt = DateTime.Now.AddMinutes(-87),
                },
                new
                {
                    Name = "AllObjectsWatcher",
                    SubjectCount = 39921,
                    CreatedAt = DateTime.Now.AddHours(-2).AddMilliseconds(3421),
                    UpdatedAt = DateTime.Now.AddMinutes(-2),
                },
                new
                {
                    Name = "CustomEventWatcher",
                    SubjectCount = 2,
                    CreatedAt = DateTime.Now.AddHours(-7).AddMilliseconds(564),
                    UpdatedAt = DateTime.Now.AddMinutes(-321),
                },
                new
                {
                    Name = "DummyTestWatcher",
                    SubjectCount = 1,
                    CreatedAt = DateTime.Now.AddHours(-2).AddMilliseconds(777777),
                    UpdatedAt = DateTime.Now.AddMinutes(-3),
                }
            };

            return Results.Ok(ravens);
        }

        private static IResult RavensHandler(HttpContext context)
        {
            var data = File.ReadAllText("HTML/ravens.html");
            return Results.Content(data, "text/html");
        }
        
        private static IResult SubjectsHandler(HttpContext context)
        {
            var data = File.ReadAllText("HTML/ravens.html");
            return Results.Content(data, "text/html");
        }
        
    }
}