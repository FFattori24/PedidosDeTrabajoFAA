using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace PPTT.Pages.Vistas
{
    public class IngresoPrimeraVezModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public IngresoPrimeraVezModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public required int DNI { get; set; }

        [BindProperty]
        public required int NumeroDeControl { get; set; }

        [BindProperty]
        public required string ViejaContrase�a { get; set; }

        [BindProperty]
        public required string NuevaContrase�a { get; set; }

        [BindProperty]
        public required string ConfirmacionContrase�a { get; set; }

        public void OnGet()
        {
            // Mensajes para comprobar que los valores por defecto est�n cargados
            Console.WriteLine("OnGet:");
            Console.WriteLine($"DNI: {DNI}");
            Console.WriteLine($"NumeroDeControl: {NumeroDeControl}");
            Console.WriteLine($"ViejaContrase�a: {ViejaContrase�a}");
            Console.WriteLine($"NuevaContrase�a: {NuevaContrase�a}");
            Console.WriteLine($"ConfirmacionContrase�a: {ConfirmacionContrase�a}");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine("Inicio OnPostAsync");
            Console.WriteLine($"DNI: {DNI}");
            Console.WriteLine($"NumeroDeControl: {NumeroDeControl}");
            Console.WriteLine($"ViejaContrase�a: {ViejaContrase�a}");
            Console.WriteLine($"NuevaContrase�a: {NuevaContrase�a}");
            Console.WriteLine($"ConfirmacionContrase�a: {ConfirmacionContrase�a}");

            bool isChanged = await CambiarContrase�a(DNI, ViejaContrase�a, NuevaContrase�a);
            Console.WriteLine("Despu�s de CambiarContrase�a");

            if (isChanged)
            {
                Console.WriteLine("Contrase�a cambiada correctamente.");
                int _rol = HttpContext.Session.GetInt32("UserRole") ?? 0;
                HttpContext.Session.SetInt32("UserRole", _rol);

                if (_rol < 2)
                {
                    return RedirectToPage("/Index");
                }
                else if (_rol > 1)
                {
                    return Page();
                }
                else
                {
                    return Page();
                }
            }
            else
            {
                Console.WriteLine("Fallo al cambiar la contrase�a.");
                ModelState.AddModelError(string.Empty, "Las credenciales no son v�lidas o no se pudo cambiar la contrase�a.");
                return Page();
            }
        }

        private async Task<bool> CambiarContrase�a(int dni,  string viejacontrase�a, string nuevacontrase�a)
        {
            Console.WriteLine("CambiarContrase�a - Par�metros recibidos:");
            Console.WriteLine($"DNI: {dni}");
            Console.WriteLine($"ViejaContrase�a: {viejacontrase�a}");
            Console.WriteLine($"NuevaContrase�a: {nuevacontrase�a}");

            string connectionString = _configuration.GetConnectionString("ConnectionSQL");
            Console.WriteLine("Connection string: " + connectionString);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    Console.WriteLine("Antes de abrir la conexi�n");
                    await connection.OpenAsync();
                    Console.WriteLine("Conexi�n abierta");

                    using (SqlCommand command = new SqlCommand("PrimerIngreso", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@DNI", dni);
                        command.Parameters.AddWithValue("@Contrase�a_Vieja", viejacontrase�a);
                        command.Parameters.AddWithValue("@Contrase�a_Nueva", nuevacontrase�a);

                        Console.WriteLine("Antes de ejecutar el stored procedure");
                        Console.WriteLine($"Stored Procedure: PrimerIngreso");
                        Console.WriteLine($"@DNI: {dni}");
                        Console.WriteLine($"@Contrase�a_Vieja: {viejacontrase�a}");
                        Console.WriteLine($"@Contrase�a_Nueva: {nuevacontrase�a}");

                        // Ejecuta el stored procedure
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        Console.WriteLine($"Stored procedure ejecutado. Filas afectadas: {rowsAffected}");

                        // Verifica si se afect� alguna fila
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                // Muestra el error en la consola para depuraci�n
                Console.WriteLine($"Error al cambiar la contrase�a: {ex.Message}");
                return false;
            }
        }
    }
}
