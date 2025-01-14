using BitOk.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using BitOk.Data.Models;
using System.Security.Claims;

namespace BitOk.Auth
{
    public class AuthMain : AuthenticationStateProvider
    {
        private readonly ProtectedSessionStorage _storage;
        private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

        public AuthMain(ProtectedSessionStorage storage)
        {
            _storage = storage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            Console.WriteLine("AuthMain: Checking authentication state...");
            var userSessionResult = await _storage.GetAsync<UserSession>("UserSession");
            var userSession = userSessionResult.Success ? userSessionResult.Value : null;

            if (userSession == null)
            {
                Console.WriteLine("AuthMain: No user session found.");
                return new AuthenticationState(_anonymous);
            }

            Console.WriteLine($"AuthMain: User session found for {userSession.Username}.");
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new(ClaimTypes.Name, userSession.Username),
                new(ClaimTypes.Role, userSession.Role)
            }, "CustomAuth"));

            return new AuthenticationState(claimsPrincipal);
        }

        public async Task UpdateAuthenticationState(UserSession? userSession)
        {
            ClaimsPrincipal claimsPrincipal;

            if (userSession != null)
            {
                await _storage.SetAsync("UserSession", userSession);
                claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new(ClaimTypes.Name, userSession.Username),
                    new(ClaimTypes.Role, userSession.Role)
                }, "CustomAuth"));
            }
            else
            {
                await _storage.DeleteAsync("UserSession");
                claimsPrincipal = _anonymous;
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

        public async Task Logout()
        {
            await UpdateAuthenticationState(null);
        }

        public async Task<bool> IsAuthenticated()
        {
            try
            {
                var userSessionResult = await _storage.GetAsync<UserSession>("UserSession");
                var userSession = userSessionResult.Success ? userSessionResult.Value : null;
                return userSession != null;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string?> GetUsername()
        {
            try
            {
                var userSessionResult = await _storage.GetAsync<UserSession>("UserSession");
                var userSession = userSessionResult.Success ? userSessionResult.Value : null;

                return userSession?.Username;
            }
            catch
            {
                return null;
            }
        }

        public async Task<string?> GetRole()
        {
            try
            {
                var userSessionResult = await _storage.GetAsync<UserSession>("UserSession");
                var userSession = userSessionResult.Success ? userSessionResult.Value : null;

                return userSession?.Role;
            }
            catch
            {
                return null;
            }
        }
    }
}