using Comandas.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Comandas.Services
{
    public class CurrentUserService
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly UserManager<ApplicationUser> _userManager;

        public CurrentUserService(AuthenticationStateProvider authenticationStateProvider, UserManager<ApplicationUser> userManager)
        {
            _authenticationStateProvider = authenticationStateProvider;
            _userManager = userManager;
        }

        public async Task<string> GetCurrentUserIdAsync()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity.IsAuthenticated)
            {
                var currentUser = await _userManager.GetUserAsync(user);
                return currentUser.Id;
            }
            else
            {
                return null;
            }
        }
        public async Task<string> GetCurrentUserNameAsync()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity.IsAuthenticated)
            {
                var currentUser = await _userManager.GetUserAsync(user);
                return currentUser.Email;
            }
            else
            {
                return null;
            }
        }

    }
}
