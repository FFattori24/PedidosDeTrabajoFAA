using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace PPTT.Pages.Vistas
{
    [Authorize(Policy = "RequireRole1")]
    public class VerTrabajoEmpleadoModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
