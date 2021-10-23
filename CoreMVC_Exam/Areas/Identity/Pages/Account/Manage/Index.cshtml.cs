using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CoreMVC_Exam.Areas.Identity.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoreMVC_Exam.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<CoreMVC_ExamUser> _userManager;
        private readonly SignInManager<CoreMVC_ExamUser> _signInManager;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public IndexModel(
            UserManager<CoreMVC_ExamUser> userManager,
            SignInManager<CoreMVC_ExamUser> signInManager,
            IWebHostEnvironment hostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _hostingEnvironment = hostEnvironment;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            public string FirstName { get; set; }

            public string LastName { get; set; }

            public DateTime Birthday { get; set; }

            public string PhotoPath { get; set; }

            public string Passport { get; set; }

            [Display(Name = "Image")]
            public IFormFile Picture { get; set; }
        }

        private async Task LoadAsync(CoreMVC_ExamUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                Birthday = user.Birthday,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Passport = user.Passport,
                PhotoPath = user.PathToImage
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

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

            user.Birthday = Input.Birthday;
            user.FirstName = Input.FirstName;
            user.LastName = Input.LastName;

            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
