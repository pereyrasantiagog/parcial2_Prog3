using Microsoft.Extensions.Configuration;
using System.IO;

namespace parcial2_Prog3
{
    /// <summary>
    /// Clase estática para gestionar la configuración de la aplicación.
    /// Su responsabilidad es leer la cadena de conexión desde el archivo appsettings.json.
    /// Esto centraliza el acceso a la configuración y evita hardcodear valores sensibles.
    /// </summary>
    public static class Config
    {
        private static IConfigurationRoot _configuration;

        static Config()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            _configuration = builder.Build();
        }

        /// <summary>
        /// Obtiene la cadena de conexión a la base de datos PostgreSQL.
        /// </summary>
        /// <returns>La cadena de conexión.</returns>
        public static string GetConnectionString()
        {
            var connectionString = _configuration.GetConnectionString("PostgreSqlConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("La cadena de conexión 'PostgreSqlConnection' no se encontró en el archivo appsettings.json.");
            }
            return connectionString;
        }
    }
}
