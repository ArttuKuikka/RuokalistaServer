// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using RuokalistaServer.Data;

namespace RuokalistaServer.Areas.Identity.Pages.Account
{
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
		private readonly ApplicationDbContext _context;

		public ResetPasswordModel(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
		}

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "Salasanan pitää olla ainakin {2} merkkiä pitkä ja saa olla enintään {1} merkkiä pitkä", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Vahvista salasana")]
            [Compare("Password", ErrorMessage = "Salasanat eivät täsmää")]
            public string ConfirmPassword { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            public string Token { get; set; }

        }

        public IActionResult OnGet(string token = null)
		{

			if (string.IsNullOrEmpty(token) || !_context.UserTokens.Any(t => t.Token == token && t.isUsed == false && t.isPasswordResetToken == true))
			{
				return BadRequest("Virheellinen linkki. Ota yhteys tukeen saamasi sähköpostin ohjeiden mukaan");
			}
			Input.Token = token;

			var newuser = _context.UserTokens.First(x => x.Token == token);
			Input.Email = newuser.UserHint ?? "";

			// ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

			return Page();
		}

        public async Task<IActionResult> OnPostAsync()
        {
			if (ModelState.IsValid)
			{
				var token = _context.UserTokens.FirstOrDefault(t => t.Token == Input.Token && t.isUsed == false && t.isPasswordResetToken == true);
				if (token == null)
				{
					ModelState.AddModelError(string.Empty, "Virheellinen linkki. Ota yhteys tukeen saamasi sähköpostin ohjeiden mukaan");
					return Page();
				}
				if(token.UserHint != Input.Email)
				{
					ModelState.AddModelError(string.Empty, "Virheellinen sähköposti. Ota yhteys tukeen saamasi sähköpostin ohjeiden mukaan");
					return Page();
				}

				//check if token expired
				if (token.Expiration < DateTime.Now)
				{
					ModelState.AddModelError(string.Empty, "Linkki on vanhentunut. Ota yhteys tukeen saamasi sähköpostin ohjeiden mukaan");
					return Page();
				}
				var user = _userManager.FindByEmailAsync(Input.Email).Result;

				if (user == null)
				{
					ModelState.AddModelError(string.Empty, "Virheellinen sähköposti. Ota yhteys tukeen saamasi sähköpostin ohjeiden mukaan");
					return Page();
				}

				await _userManager.RemovePasswordAsync(user);
				var result = await _userManager.AddPasswordAsync(user, Input.Password);

				if (result.Succeeded)
				{
					token.isUsed = true;
					await _context.SaveChangesAsync();

					return Redirect("./ResetPasswordConfirmation");
				}
			
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
			}

			// If we got this far, something failed, redisplay form
			return Page();
		}
    }
}
