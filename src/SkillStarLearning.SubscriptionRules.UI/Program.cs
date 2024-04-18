using SkillStarLearning.SubscriptionRules.Application;
using SkillStarLearning.SubscriptionRules.Infrastructure;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SkillStarLearning.SubscriptionRules.UI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddApplicationServices();
            builder.Services.AddInfrastructureServices();
            builder.Services.AddControllers();

            var app = builder.Build();

            app.MapGet("/", () => new
            {
                Application = "SubscriptionRules",
                TargetFramework = "net10.0"
            });

            app.MapControllers();

            app.Run();
        }
    }
}
