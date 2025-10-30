using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SIRGA.Web.Models.API;
using SIRGA.Web.Models.Auth;
using SIRGA.Web.Models.Profile;
using SIRGA.Web.Services;
using System.Security.Claims;

namespace SIRGA.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApiService _apiService;

        public AccountController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var apiResponse = await _apiService.PostAsync<LoginRequest, ApiAuthResponse>(
                    "api/Auth/Login", model);

                if (apiResponse?.Success != true || apiResponse.Data == null)
                {
                    TempData["ErrorMessage"] = apiResponse?.Message ?? "Usuario o contraseña incorrectos";
                    return View(model);
                }

                var response = apiResponse.Data;

                Console.WriteLine($"JWToken recibido: {response.JWToken}");
                Console.WriteLine($"JWToken is null or empty: {string.IsNullOrEmpty(response.JWToken)}");

                // Guardar en sesión
                HttpContext.Session.SetString("UserId", response.UserId);

                // Solo guardar el token si viene en la respuesta
                if (!string.IsNullOrEmpty(response.JWToken))
                {
                    HttpContext.Session.SetString("JWTToken", response.JWToken);
                    Console.WriteLine($"Token guardado en sesión");
                }
                else
                {
                    Console.WriteLine($"❌ No hay JWToken en la respuesta de la API");
                }

                // Crear claims para cookies
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, response.UserId),
            new Claim(ClaimTypes.Name, $"{response.FirstName} {response.LastName}"),
            new Claim(ClaimTypes.Email, response.Email),
            new Claim("FirstName", response.FirstName),
            new Claim("LastName", response.LastName)
        };

                foreach (var role in response.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                };

                await HttpContext.SignInAsync("Cookies",
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // LOGS PARA DEBUGGING
                var primaryRole = response.Roles.FirstOrDefault();
                Console.WriteLine($"Usuario logueado: {response.FirstName} {response.LastName}");
                Console.WriteLine($"Rol principal: {primaryRole}");
                Console.WriteLine($"Todos los roles: {string.Join(", ", response.Roles)}");
                Console.WriteLine($"Debe completar perfil: {response.MustCompleteProfile}");

                // Verificar si debe completar perfil
                if (response.MustCompleteProfile)
                {
                    Console.WriteLine($"Redirigiendo a completar perfil");
                    return primaryRole switch
                    {
                        "Estudiante" => RedirectToAction("CompleteProfile", "Estudiante"),
                        "Profesor" => RedirectToAction("CompleteProfile", "Profesor"),
                        _ => RedirectToAction("Index", "Home")
                    };
                }

                // Redirigir según el rol
                Console.WriteLine($" Redirigiendo a dashboard de: {primaryRole}");

                return primaryRole switch
                {
                    "Admin" => RedirectToAction("Index", "Admin"),
                    "Estudiante" => RedirectToAction("Index", "Estudiante"),
                    "Profesor" => RedirectToAction("Index", "Profesor"),
                    _ => RedirectToAction("Index", "Home")
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en login: {ex.Message}");
                Console.WriteLine($"❌ Stack trace: {ex.StackTrace}");
                TempData["ErrorMessage"] = "Error al procesar la solicitud. Intente nuevamente.";
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequest model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var response = await _apiService.PostAsync<RegisterRequest, object>(
                    "api/Auth/register", model);

                if (response == null)
                {
                    TempData["ErrorMessage"] = "Error al registrar usuario";
                    return View(model);
                }

                TempData["SuccessMessage"] = "Registro exitoso. Por favor verifica tu correo electrónico.";
                return RedirectToAction(nameof(Login));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al procesar la solicitud. Intente nuevamente.";
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            try
            {
                // 1. OBTENER LA URL BASE DEL FRONTEND AQUÍ
                var origin = $"{Request.Scheme}://{Request.Host}"; // Esto es correcto en el frontend

                // 2. Crear un objeto de solicitud que incluya el email y el origin
                var requestData = new { email, origin }; // Objeto anónimo para el POST

                // 3. Enviar al API
                var response = await _apiService.PostAsync<object, object>(
                    "api/Auth/Forgot-Password", requestData);

                TempData["SuccessMessage"] = "Si el correo existe, recibirás instrucciones para restablecer tu contraseña.";
                return RedirectToAction(nameof(Login));
            }
            catch
            {
                TempData["SuccessMessage"] = "Si el correo existe, recibirás instrucciones para restablecer tu contraseña.";
                return RedirectToAction(nameof(Login));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmailAsync(string userId, string token)
        {
            try
            {
                var response = await _apiService.GetAsync<object>(
                    $"api/Auth/Confirm-Email?userId={userId}&token={Uri.EscapeDataString(token)}");

                if (response != null)
                {
                    TempData["SuccessMessage"] = "Email confirmado exitosamente. Ya puedes iniciar sesión.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al confirmar el email. El enlace puede haber expirado.";
                }
            }
            catch
            {
                TempData["ErrorMessage"] = "Error al confirmar el email.";
            }

            return RedirectToAction(nameof(Login));
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmEmailPost(string userId, string token)
        {
            var response = await _apiService.GetAsync<object>(
                $"api/Auth/Confirm-Email?userId={userId}&token={Uri.EscapeDataString(token)}");

            if (response != null)
            {
                TempData["SuccessMessage"] = "Email confirmado exitosamente. Ya puedes iniciar sesión.";
                return RedirectToAction(nameof(Login));
            }

            TempData["ErrorMessage"] = "Error al confirmar el email.";
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult ResetPassword(string? token = null, string? email = null)
        {
            // Verifica si el token y el email están presentes en la URL.
            // Si no lo están, redirige a una página de error o login.
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "Enlace de restablecimiento de contraseña no válido.";
                return RedirectToAction(nameof(Login));
            }

            // Pasa los datos a la vista para que puedan ser incluidos en el formulario ocultos.
            var model = new ResetPasswordRequest { Token = token, Email = email };
            return View(model);
        }

       
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest model)
        {
            
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                // Llamar al endpoint de la API usando el modelo ApiResponse<object>
                // La T en este caso es 'object' porque no esperamos datos ('Data') útiles, solo Success/Message/Errors.
                var apiResponse = await _apiService.PostAsync<ResetPasswordRequest, ApiResponse<object>>(
                    "api/Auth/Reset-Password", model);

                
                if (apiResponse?.Success == true)
                {
                    TempData["SuccessMessage"] = apiResponse.Message ?? "Contraseña restablecida exitosamente. Ya puedes iniciar sesión.";
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    
                    string errorMessage = apiResponse?.Message ?? "Error desconocido al restablecer la contraseña.";

                   
                    if (apiResponse?.Errors != null && apiResponse.Errors.Any())
                    {
                        
                        errorMessage = string.Join(" | ", apiResponse.Errors);
                    }

                    ModelState.AddModelError(string.Empty, errorMessage);

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                
                ModelState.AddModelError(string.Empty, "Error de comunicación con el servicio. Inténtelo de nuevo.");
                
                return View(model);
            }
        }
    }
}
