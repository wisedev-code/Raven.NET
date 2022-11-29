using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Raven.NET.Analytics.Misc;
using Raven.NET.Analytics.Services;
using Raven.NET.Core.Providers.Interfaces;
using Raven.NET.Core.Storage;
using Raven.NET.Core.Storage.Interfaces;

namespace Raven.NET.Analytics
{
    public class RavenAnalyticsApi : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            var builder = WebApplication.CreateBuilder();
            builder.WebHost.UseUrls("https://*:6006");

            var app = builder.Build();
            
            app.MapGet("/analytics", RavensHandler);
            app.MapGet("/analytics/s", SubjectHandler);
            app.MapGet("/analytics/about", AboutHandler);
            app.MapGet("/analytics/ravens", GetRavens);
            app.MapGet("/analytics/subjects", GetSubjects);

            app.Run();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        private IResult GetRavens()
        {
            var ravenStorage = RavenStorage.GetInstance();
            var analyticsBuilder = new AnalyticBuilder(ravenStorage);
            var ravens = analyticsBuilder.BuildRavensData(); //RandomDataGenerator.CreateRavens(8);
            return Results.Ok(ravens);
        }
        
        private IResult GetSubjects(HttpContext context)
        {
            var ravenStorage = RavenStorage.GetInstance();
            var analyticsBuilder = new AnalyticBuilder(ravenStorage);
            var subjects = analyticsBuilder.BuildSubjectsData(); //RandomDataGenerator.CreateSubjects(256);
            return Results.Ok(subjects);
        }

        private IResult RavensHandler(HttpContext context)
        {
            var data = File.ReadAllText("HTML/ravens.html");
            return Results.Content(data, "text/html");
        }
        
        private IResult AboutHandler(HttpContext context)
        {
            var data = File.ReadAllText("HTML/about.html");
            return Results.Content(data, "text/html");
        }

        private IResult SubjectHandler(HttpContext context)
        {
            var data = File.ReadAllText("HTML/subjects.html");
            return Results.Content(data, "text/html");
        }
    }
}