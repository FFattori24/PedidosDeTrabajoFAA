using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace PPTT.Pages.Vistas
{
    public class IngresoPersonalModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public IngresoPersonalModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public required int DNI { get; set; }

        [BindProperty]
        public required int NumeroDeControl { get; set; }

        [BindProperty]
        public required string Contrase�a { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Validar el login usando el procedimiento almacenado
            bool isValid = await ValidarUsuario(DNI, NumeroDeControl, Contrase�a);

            if (isValid)
            {
                // Redirigir al index si es v�lido
                return RedirectToPage("/Index");
            }
            else
            {
                // Si no es v�lido, retornar a la misma p�gina y mostrar un mensaje
                ModelState.AddModelError(string.Empty, "Las credenciales no son v�lidas.");
                return Page();
            }
        }

        private async Task<bool> ValidarUsuario(int dni, int numeroDeControl, string contrase�a)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("Validar", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // A�adir par�metros al procedimiento almacenado
                        command.Parameters.AddWithValue("@DNI", dni);
                        command.Parameters.AddWithValue("@Numero_De_Control", numeroDeControl);
                        command.Parameters.AddWithValue("@Password", contrase�a);

                        // Ejecutar y obtener los resultados
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                            {
                                // Si hay filas, significa que el usuario es v�lido
                                return true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Manejar errores
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            // Si no se encontr� el usuario o hubo alg�n error, retornar falso
            return false;
        }
    }
}
