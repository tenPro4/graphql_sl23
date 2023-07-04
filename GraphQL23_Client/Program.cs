//using GraphQL23_Client;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using StrawberryShake;

//var serviceCollection = new ServiceCollection();

//var configuration = new ConfigurationBuilder()
//    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
//    .Build();

//var graphqlApiUrl = configuration.GetValue<string>("GRAPHQL_API_URL");

//string httpGraphQLApiUrl = $"https://{graphqlApiUrl}";
//string webSocketsGraphQLApiUrl = $"ws://{graphqlApiUrl}";

//serviceCollection.AddExampleClient()
//    .ConfigureHttpClient(c => c.BaseAddress = new Uri(httpGraphQLApiUrl));

//var serviceProvider = serviceCollection.BuildServiceProvider();

//var client = serviceProvider.GetRequiredService<IExampleClient>();

//IOperationResult<IGetCourseByIdResult> result = await client.GetCourseById.ExecuteAsync(Guid.Parse("8F004571-15E9-42EB-ADAA-739D390C9F77"));

//if (result.IsSuccessResult())
//{
//    IGetCourseById_CourseById rrr = result.Data.CourseById;
//    Console.WriteLine($@"Course name {rrr.Name}");
//}

//Console.ReadLine();

using Firebase.Auth;
using Firebase.Auth.Providers;
using GraphQL23_Client.Scripts;
using GraphQL23_Client.Stores;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http.Headers;

namespace GraphQL23_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    string graphqlApiUrl = context.Configuration.GetValue<string>("GRAPHQL_API_URL");

                    string httpGraphQLApiUrl = $"https://{graphqlApiUrl}";
                    string webSocketsGraphQLApiUrl = $"ws://{graphqlApiUrl}";

                    services
                        .AddExampleClient()
                        .ConfigureHttpClient((services, c) =>
                        {
                            c.BaseAddress = new Uri(httpGraphQLApiUrl);

                            TokenStore tokenStore = services.GetRequiredService<TokenStore>();
                            c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenStore.AccessToken);
                        })
                        .ConfigureWebSocketClient(c => c.Uri = new Uri(webSocketsGraphQLApiUrl));

                    var client = services.BuildServiceProvider().GetRequiredService<IExampleClient>();

                    client.CourseCreated.Watch().Subscribe(result => { 
                        string name = result.Data.CourseCreated.Name;
                        Console.WriteLine($"Course {name} was created");
                    });

                    services.AddHostedService<Startup>();

                    services.AddSingleton<TokenStore>();

                    string firebaseApiKey = context.Configuration.GetValue<string>("FIREBASE_API_KEY");
                    string firebaseDomain = context.Configuration.GetValue<string>("FIREBASE_AUTH_DOMAIN");
                    var config = new FirebaseAuthConfig
                    {
                        ApiKey = firebaseApiKey,
                        AuthDomain = firebaseDomain,
                        Providers = new FirebaseAuthProvider[]
                        {
                            new GoogleProvider().AddScopes("email"),
                            new EmailProvider()
                        }
                    };
                    services.AddSingleton(config);

                    services.AddTransient<GetCoursesScript>();
                    services.AddTransient<CreateCourseScript>();
                    services.AddTransient<LoginScript>();
                    services.AddTransient<SearchScript>();
                })
                .Build()
                .Run();
        }
    }
    public class Startup : IHostedService
    {
        private readonly GetCoursesScript _searchScript;

        public Startup(GetCoursesScript searchScript)
        {
            _searchScript = searchScript;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _searchScript.Run();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}