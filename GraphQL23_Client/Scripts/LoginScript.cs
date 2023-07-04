using Firebase.Auth;
using Firebase.Auth.Providers;
using GraphQL23_Client.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphQL23_Client.Scripts
{
    public class LoginScript
    {
        private readonly FirebaseAuthConfig _config;
        private readonly TokenStore _tokenStore;
        private readonly FirebaseAuthClient _client;

        public LoginScript(FirebaseAuthConfig config, TokenStore tokenStore)
        {
            _config = config;
            _tokenStore = tokenStore;
            _client = new FirebaseAuthClient(config);
        }

        public async Task Run()
        {
            Console.WriteLine("Enter your email:");
            string email = Console.ReadLine();

            Console.WriteLine("Enter your password:");
            string password = Console.ReadLine();

            var userCredential = await _client.SignInWithEmailAndPasswordAsync(email, password);
            _tokenStore.AccessToken = userCredential.User.Credential.IdToken;


            Console.WriteLine("Successfully logged in.");
        }
    }
}
