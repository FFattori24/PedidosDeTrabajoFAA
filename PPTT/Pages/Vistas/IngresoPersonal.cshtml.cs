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
            bool isValid = await EjecutarValidarStoredProcedure(DNI, NumeroDeControl, Contrase�a);

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

        private async Task<bool> EjecutarValidarStoredProcedure(int dni, int numeroDeControl, string password)
        {
            // Obtener la cadena de conexi�n desde appsettings.json
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // Crear el comando y especificar el nombre del stored procedure
                    using (SqlCommand command = new SqlCommand("Validar", connection))
                    {
                        // Especificar que es un stored procedure
                        command.CommandType = CommandType.StoredProcedure;

                        // Agregar los par�metros que el stored procedure espera
                        command.Parameters.AddWithValue("@DNI", dni);
                        command.Parameters.AddWithValue("@Numero_De_Control", numeroDeControl);
                        command.Parameters.AddWithValue("@Password", password);

                        // Ejecutar el stored procedure y leer los resultados
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                            {
                                // Si hay filas, significa que el login es v�lido
                                return true;
                            }
                            else
                            {
                                // Si no hay filas, las credenciales no son v�lidas
                                return false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepci�n que pueda ocurrir
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
    }
}
