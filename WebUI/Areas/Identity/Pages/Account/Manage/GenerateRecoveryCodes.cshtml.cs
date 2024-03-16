// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebUI.Areas.Identity.Pages.Account.Manage
{
    public class GenerateRecoveryCodesModel(
        UserManager<IdentityUser> userManager,
        ILogger<GenerateRecoveryCodesModel> logger) : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly ILogger<GenerateRecoveryCodesModel> _logger = logger;

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string[] RecoveryCodes
        {
            get; set;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage
        {
            get; set;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
            if (!isTwoFactorEnabled)
            {
                throw new InvalidOperationException("Cannot generate recovery codes for user because they do not have 2FA enabled.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (await _userManager.GetUserAsync(User) == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(await _userManager.GetUserAsync(User));
            var userId = await _userManager.GetUserIdAsync(await _userManager.GetUserAsync(User));
            if (!isTwoFactorEnabled)
            {
                throw new InvalidOperationException("Cannot generate recovery codes for user as they do not have 2FA enabled.");
            }

            RecoveryCodes = (await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(await _userManager.GetUserAsync(User), 10)).ToArray();

            _logger.LogInformation("User with ID '{UserId}' has generated new 2FA recovery codes.", userId);
            StatusMessage = "You have generated new recovery codes.";
            return RedirectToPage("./ShowRecoveryCodes");
        }
    }
}
