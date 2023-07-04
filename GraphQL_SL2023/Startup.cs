using FirebaseAdmin;
using FirebaseAdminAuthentication.DependencyInjection.Extensions;
using FirebaseAdminAuthentication.DependencyInjection.Models;
using FluentValidation.AspNetCore;
using GraphQL_SL2023.DataLoaders;
using GraphQL_SL2023.Models;
using GraphQL_SL2023.Schema.Mutations;
using GraphQL_SL2023.Schema.Queries;
using GraphQL_SL2023.Schema.Subscriptions;
using GraphQL_SL2023.Services.Courses;
using GraphQL_SL2023.Services.Instructors;
using GraphQL_SL2023.Validators;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using AppAny.HotChocolate.FluentValidation;
using Google.Apis.Auth.OAuth2;

namespace GraphQL_SL2023
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddFluentValidation();
            services.AddTransient<CourseTypeInputValidator>();
            services.AddTransient<InstructorTypeInputValidator>();

            services.AddGraphQLServer()
            .AddQueryType<Query>()
            .AddMutationType<Mutation>()
            .AddTypeExtension<CourseMutation>()
            .AddTypeExtension<InstructorMutation>()
            .AddSubscriptionType<Subscription>()
            .AddTypeExtension<InstructorQuery>()
            .AddTypeExtension<CourseQuery>()
            .AddType<CourseType>()
            .AddType<InstructorType>()
            .AddInMemorySubscriptions()
            .AddFiltering()
            .AddProjections()
            .AddSorting()
            .AddAuthorization()
            .AddFluentValidation(o =>
            {
                o.UseDefaultErrorMapper();
            });

            services.AddSingleton(FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromJson(_configuration.GetValue<string>("FIREBASE_CONFIG"))
            }));
            services.AddFirebaseAuthentication();

            services.AddAuthorization(
                o => o.AddPolicy("IsAdmin",
                    p => p.RequireClaim(FirebaseUserClaimType.EMAIL, "geiwojina@gmail.com")));

            var connectionString = _configuration.GetConnectionString("default");
            services.AddPooledDbContextFactory<SchoolDbContext>(o => o.UseSqlite(connectionString).LogTo(Console.WriteLine));

            services.AddScoped<CoursesRepository>();
            services.AddScoped<InstructorsRepository>();
            services.AddScoped<InstructorDataLoader>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseAuthentication();

            app.UseWebSockets();

            app.UseEndpoints(endpoint =>
            {
                endpoint.MapGraphQL();
            });
        }
    }
}
