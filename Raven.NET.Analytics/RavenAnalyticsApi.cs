using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Raven.NET.Analytics.Misc;
using Raven.NET.Analytics.Services;
using Raven.NET.Core.Storage;

namespace Raven.NET.Analytics
{
    public class RavenAnalyticsApi : IHostedService
    {
        internal static bool PopulateRandomData;
        public Task StartAsync(CancellationToken cancellationToken)
        {
            new Thread(
                delegate ()
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
                }).Start();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
        
        private IResult GetRavens()
        {
            var ravenStorage = RavenStorage.GetInstance();
            var analyticsBuilder = new AnalyticBuilder(ravenStorage);
            var ravens = !PopulateRandomData ? analyticsBuilder.BuildRavensData() : RandomDataGenerator.CreateRavens(8);
            return Results.Ok(ravens);
        }
        
        private IResult GetSubjects()
        {
            var ravenStorage = RavenStorage.GetInstance();
            var analyticsBuilder = new AnalyticBuilder(ravenStorage);
            var subjects = !PopulateRandomData ? analyticsBuilder.BuildSubjectsData() : RandomDataGenerator.CreateSubjects(256);
            return Results.Ok(subjects);
        }

        private IResult RavensHandler(HttpContext context)
        {
            var data = Resources.ReadResource("ravens.html");
            return Results.Content(data, "text/html");
        }
        
        private IResult AboutHandler(HttpContext context)
        {
            var data = Resources.ReadResource("about.html");
            return Results.Content(data, "text/html");
        }

        private IResult SubjectHandler(HttpContext context)
        {
            var data = Resources.ReadResource("subjects.html");
            return Results.Content(data, "text/html");
        }
    }
}