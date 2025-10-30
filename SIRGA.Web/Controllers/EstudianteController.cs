using Microsoft.AspNetCore.Mvc;
using SIRGA.Web.Models.API;
using SIRGA.Web.Models.Estudiante;
using SIRGA.Web.Models.Profile;
using SIRGA.Web.Services;
using System.Security.Claims;

namespace SIRGA.Web.Controllers
{
    public class EstudianteController : Controller
    {
        private readonly ApiService _apiService;

        public EstudianteController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileResponse = await _apiService.GetAsync<ApiResponse<UserProfileDto>>("api/Profile/My-Profile");

            if (profileResponse?.Data?.MustCompleteProfile == true)
            {
                return RedirectToAction(nameof(CompleteProfile));
            }

            var model = new EstudianteDashboardViewModel
            {
                Profile = profileResponse?.Data,
                UserName = User.FindFirstValue(ClaimTypes.Name)
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult CompleteProfile()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CompleteProfile(CompleteStudentProfileDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var response = await _apiService.PostAsync<CompleteStudentProfileDto, ApiResponse<object>>(
                    "api/Estudiante/Completar-Perfil", model);

                if (response?.Success == true)
                {
                    TempData["SuccessMessage"] = "Perfil completado exitosamente";
                    return RedirectToAction(nameof(Index));
                }

                TempData["ErrorMessage"] = response?.Message ?? "Error al completar el perfil";
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al procesar la solicitud";
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var profileResponse = await _apiService.GetAsync<ApiResponse<UserProfileDto>>("api/Profile/My-Profile");

            if (profileResponse?.Success != true)
            {
                TempData["ErrorMessage"] = "Error al cargar el perfil";
                return RedirectToAction(nameof(Index));
            }

            return View(profileResponse.Data);
        }
    }
    public class EstudianteDashboardViewModel
    {
        public UserProfileDto Profile { get; set; }
        public string UserName { get; set; }
    }
}
