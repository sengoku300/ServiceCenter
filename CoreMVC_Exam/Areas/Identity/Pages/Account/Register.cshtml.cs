using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using CoreMVC_Exam.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace CoreMVC_Exam.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<CoreMVC_ExamUser> _signInManager;
        private readonly UserManager<CoreMVC_ExamUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public RegisterModel(
            UserManager<CoreMVC_ExamUser> userManager,
            SignInManager<CoreMVC_ExamUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<RegisterModel> logger,
            IWebHostEnvironment hostingEnvironment,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _hostingEnvironment = hostingEnvironment;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Эл.Почта")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Пароль")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Подтверждение пароля")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [PersonalData]
            [Display(Name = "Пасспорт")]
            [Required(ErrorMessage = "Введите номер вашего пасспорта!")]
            [StringLength(50)]
            public string Passport { get; set; }

            [PersonalData]
            [Column(TypeName = "nvarchar(100)")]
            [Required(ErrorMessage = "Введите своё имя!")]
            [Display(Name = "Имя")]
            public string FirstName { get; set; }

            [PersonalData]
            [Column(TypeName = "nvarchar(100)")]
            [Display(Name = "Фамилия")]
            [Required(ErrorMessage = "Введите свою фамилию!")]
            public string LastName { get; set; }

            [Display(Name = "День рождения")]
            [DataType(DataType.Date)]
            public DateTime Birthday { get; set; }

            [Display(Name = "Фото")]
            [Required(ErrorMessage = "Нужно загрузить ваше фото!")]
            public IFormFile Picture { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new CoreMVC_ExamUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    Birthday = Input.Birthday,
                    Passport = Input.Passport
                };

                if (Input.Picture != null)
                {
                    var savePath = _hostingEnvironment.WebRootPath
                                           + "\\Files\\" + Input.Picture.FileName;

                    // сохранение файла
                    using (var fileStream = new FileStream(savePath, FileMode.Create))
                    {
                        await Input.Picture.CopyToAsync(fileStream);
                    }

                    user.PathToImage = "\\Files\\" + Input.Picture.FileName;
                }

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    // добавление роли и пользователя в эту роль
                    bool roleExists = await _roleManager.RoleExistsAsync("Master");

                    if (!roleExists)
                    {
                        // Создание роли
                        var role = new IdentityRole("Master");
                        await _roleManager.CreateAsync(role);
                    }

                    // Добавление пользователя в роль
                    await _userManager.AddToRoleAsync(user, "Master");
                   
                    Claim claim = new Claim("Position", "Master");
                    await _userManager.AddClaimAsync(user, claim);
                    await _userManager.UpdateAsync(user);

                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
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
