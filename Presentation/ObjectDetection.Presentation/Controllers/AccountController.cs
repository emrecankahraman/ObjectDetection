using Microsoft.AspNetCore.Mvc;
using ObjectDetection.Application.Abstractions;
using ObjectDetection.Presentation.Models;
using System.Threading.Tasks;

namespace ObjectDetection.Presentation.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        // ---------- LOGIN ----------
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var success = await _authService.LoginAsync(model.Email, model.Password, model.RememberMe);
            if (success)
                return RedirectToAction("Upload", "Image");

            ModelState.AddModelError("", "Giriş başarısız.");
            return View(model);
        }

        // ---------- REGISTER ----------
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var success = await _authService.RegisterAsync(model.Email, model.Password);
            if (success)
                return RedirectToAction("Login");

            ModelState.AddModelError("", "Kayıt başarısız.");
            return View(model);
        }

        // ---------- LOGOUT ----------
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return RedirectToAction("Login");
        }
    }
}
