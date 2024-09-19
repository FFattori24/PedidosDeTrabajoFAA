
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PPTT.Pages.Vistas
{
    public class EmpleadosModel : PageModel
    {

        [BindProperty]
        public required string Nombre { get; set; }

        [BindProperty]
        public required string Apellido { get; set; }

        [BindProperty]
        public required string Correo { get; set; }

        [BindProperty]
        public required int NumeroDeControl { get; set; }

        [BindProperty]
        public required string Division { get; set; }

        [BindProperty]
        public required string Servicio { get; set; }

        [BindProperty]
        public required string Rol { get; set; }

        public IActionResult OnGet()
        {
            // Obt�n el rol del usuario desde la sesi�n
            int? userRole = HttpContext.Session.GetInt32("UserRole");

            // Verifica si el rol no es igual a 2
            if (userRole != 2)
            {
                // Redirige a otra p�gina si el rol no es 2
                return RedirectToPage("/Menu"); // Cambia "/AccessDenied" a la p�gina de destino que desees
            }

            // Si el rol es 2, contin�a con la l�gica de la p�gina
            return Page();
        }

        public IActionResult OnPost()
        {
            return RedirectToPage("/Index");
        }
    }
}