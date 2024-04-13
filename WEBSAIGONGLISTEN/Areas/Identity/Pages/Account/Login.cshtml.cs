// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using WEBSAIGONGLISTEN.Models;
using Microsoft.CodeAnalysis.Scripting;
using Owl.reCAPTCHA.v2;
using NuGet.Common;
using Owl.reCAPTCHA;

//namespace WEBSAIGONGLISTEN.Areas.Identity.Pages.Account
//{
//    public class LoginModel : PageModel
//    {
//        private readonly SignInManager<ApplicationUser> _signInManager;
//        private readonly ILogger<LoginModel> _logger;
//        private readonly IreCAPTCHASiteVerifyV2 _siteVerify;

//        public LoginModel(SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger, IreCAPTCHASiteVerifyV2 siteVerify)
//        {
//            _signInManager = signInManager;
//            _logger = logger;
//            _siteVerify = siteVerify;
//        }

//        [BindProperty]
//        public InputModel Input { get; set; }

//        public IList<AuthenticationScheme> ExternalLogins { get; set; }

//        /// <summary>
//        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
//        ///     directly from your code. This API may change or be removed in future releases.
//        /// </summary>
//        public string ReturnUrl { get; set; }

//        /// <summary>
//        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
//        ///     directly from your code. This API may change or be removed in future releases.
//        /// </summary>
//        [TempData]
//        public string ErrorMessage { get; set; }

//        public string Captcha { get; set; }

//        /// <summary>
//        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
//        ///     directly from your code. This API may change or be removed in future releases.
//        /// </summary>
//        public class InputModel
//        {
//            /// <summary>
//            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
//            ///     directly from your code. This API may change or be removed in future releases.
//            /// </summary>
//            [Required]
//            [EmailAddress]
//            public string Email { get; set; }

//            /// <summary>
//            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
//            ///     directly from your code. This API may change or be removed in future releases.
//            /// </summary>
//            [Required]
//            [DataType(DataType.Password)]
//            public string Password { get; set; }

//            /// <summary>
//            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
//            ///     directly from your code. This API may change or be removed in future releases.
//            /// </summary>
//            [Display(Name = "Remember me?")]
//            public bool RememberMe { get; set; }
//        }

//        public async Task OnGetAsync(string returnUrl = null)
//        {
//            if (!string.IsNullOrEmpty(ErrorMessage))
//            {
//                ModelState.AddModelError(string.Empty, ErrorMessage);
//            }

//            returnUrl ??= Url.Content("~/");

//            // Clear the existing external cookie to ensure a clean login process
//            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

//            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

//            ReturnUrl = returnUrl;
//        }

//        public async Task<IActionResult> OnPostAsync(string returnUrl = null, LoginModel loginModel)
//        {
//            if (string.IsNullOrEmpty(loginModel.Captcha))
//            {
//                ModelState.AddModelError("error", "Invalid captcha");
//                return View(loginModel);
//            }

//            var response = await _siteVerify.Verify(new reCAPTCHASiteVerifyRequest
//            {
//                Response = loginModel.Captcha,
//                RemoteIp = HttpContext.Connection.RemoteIpAddress.ToString()
//            });

//            if (!response.Success)
//            {
//                ModelState.AddModelError("error", "Invalid captcha");
//                return View(loginModel);
//            }

//            returnUrl ??= Url.Content("~/");

//            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

//            if (ModelState.IsValid)
//            {
//                // This doesn't count login failures towards account lockout
//                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
//                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
//                if (result.Succeeded)
//                {
//                    /*  _logger.LogInformation("User logged in.");
//                      return LocalRedirect(returnUrl);*/
//                    /* ===========================================*/

//                    var user = await _signInManager.UserManager.FindByEmailAsync(Input.Email);
//                    if (user != null)
//                    {
//                        // Kiểm tra xem người dùng có trong vai trò "Admin" hay không
//                        if (await _signInManager.UserManager.IsInRoleAsync(user, "Admin"))
//                        {
//                            _logger.LogInformation("Admin logged in.");
//                            return RedirectToAction("Index", "Product", new { area = "Admin" });
//                        }
//                        // Kiểm tra xem người dùng có trong vai trò "Company" hay không
//                        else if (await _signInManager.UserManager.IsInRoleAsync(user, "Company"))
//                        {
//                            _logger.LogInformation("Company logged in.");
//                            return RedirectToAction("Index", "Product", new { area = "Admin" });
//                        }
//                    }
//                    _logger.LogInformation("User logged in.");
//                    return LocalRedirect(returnUrl);

//                }
//                if (result.RequiresTwoFactor)
//                {
//                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
//                }
//                if (result.IsLockedOut)
//                {
//                    _logger.LogWarning("User account locked out.");
//                    return RedirectToPage("./Lockout");
//                }
//                else
//                {
//                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
//                    return Page();
//                }
//            }

//            // If we got this far, something failed, redisplay form
//            return Page();
//        }
//        /*
//                public class LoginSiteModel
//                {
//                    [BindProperty]
//                    public InputModel Input { get; set; }
//                    public IList<AuthenticationScheme> ExternalLogins { get; set; }
//                    [TempData]
//                    public string ErrorMessage { get; set; }
//                    public string Password { get; set; }
//                    public string ReturnUrl { get; set; }

//                }*/
//    }
//}

//6LcorbkpAAAAAOSZZ5kTimR5bsbRmU4TeErp84bO
//6LcorbkpAAAAAC5oGihIiVHY50_IsXRSnxrQUnHA

// Import các namespace cần thiết

namespace WEBSAIGONGLISTEN.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly IreCAPTCHASiteVerifyV2 _siteVerify;

        public LoginModel(SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger, IreCAPTCHASiteVerifyV2 siteVerify)
        {
            _signInManager = signInManager;
            _logger = logger;
            _siteVerify = siteVerify;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        [BindProperty(Name = "g-recaptcha-response")]
        public string RecaptchaResponse { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (string.IsNullOrEmpty(RecaptchaResponse))
            {
                ModelState.AddModelError(string.Empty, "Please complete the reCAPTCHA");
                return Page();
            }

            var response = await _siteVerify.Verify(new reCAPTCHASiteVerifyRequest
            {
                Response = RecaptchaResponse,
                RemoteIp = HttpContext.Connection.RemoteIpAddress.ToString()
            });

            if (!response.Success)
            {
                ModelState.AddModelError(string.Empty, "reCAPTCHA verification failed");
                return Page();
            }

            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    /*  _logger.LogInformation("User logged in.");
                      return LocalRedirect(returnUrl);*/
                    /* ===========================================*/

                    var user = await _signInManager.UserManager.FindByEmailAsync(Input.Email);
                    if (user != null)
                    {
                        // Kiểm tra xem người dùng có trong vai trò "Admin" hay không
                        if (await _signInManager.UserManager.IsInRoleAsync(user, "Admin"))
                        {
                            _logger.LogInformation("Admin logged in.");
                            return RedirectToAction("Index", "Product", new { area = "Admin" });
                        }
                        // Kiểm tra xem người dùng có trong vai trò "Company" hay không
                        else if (await _signInManager.UserManager.IsInRoleAsync(user, "Company"))
                        {
                            _logger.LogInformation("Company logged in.");
                            return RedirectToAction("Index", "Product", new { area = "Admin" });
                        }
                    }
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);

                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}