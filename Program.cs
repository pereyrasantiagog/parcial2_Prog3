using parcial2_Prog3.Algorithm;
using parcial2_Prog3.Data;
using parcial2_Prog3.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace parcial2_Prog3
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- Examen Final - Algoritmia Avanzada con C# y PostgreSQL ---");

            // --- Parte A: Configuración y Datos del Alumno ---
            var masterControl = new MasterControl
            {
                NombreAlumno = "Santiago",
                ApellidoAlumno = "Pereyra",
                DniAlumno = "41.650.097",
                NombreMateria = "Programación 3"
            };

            // --- Parte B: Lógica de Vuelo y Algoritmia ---
            int n = GetBoardSize();
            var (startX, startY) = GetStartCoordinates(n);
            
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var dronAlgorithm = new DronAlgorithm(n);
            List<Parcela> camino = dronAlgorithm.EncontrarCamino(startX, startY);

            stopwatch.Stop();

            if (camino.Any())
            {
                Console.WriteLine($"\n¡Solución encontrada en {stopwatch.ElapsedMilliseconds} ms!");
                string pathString = string.Join(" -> ", camino.Select(p => $"({p.X},{p.Y})"));
                Console.WriteLine("Camino: " + pathString);
                masterControl.PathSolucionFinal = pathString;

                // Llamada al método de renderizado visual de la Parte E
                DibujarMatriz(n, camino);

                // --- Parte C y D: Persistencia y Ofuscación ---
                try
                {
                    var dbRepo = new DatabaseRepository();
                    dbRepo.InsertMasterAndDetail(masterControl, camino);
                    Console.WriteLine("\nDatos guardados correctamente en la base de datos.");

                    // --- Parte E: Ingeniería Inversa ---
                    dbRepo.ReverseEngineerLast5Steps();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nSe produjo un error en la base de datos: {ex.Message}");
                    Console.WriteLine("\nAsegúrese de que PostgreSQL esté en ejecución y la cadena de conexión en 'appsettings.json' sea correcta.");
                }
            }
            else
            {
                Console.WriteLine("\nNo se pudo encontrar una solución para el tamaño y punto de inicio dados.");
            }

            Console.WriteLine("\n--- Fin del Programa ---");
        }

        /// <summary>
        /// Solicita al usuario el tamaño del terreno (N).
        /// </summary>
        private static int GetBoardSize()
        {
            int n;
            while (true)
            {
                Console.Write("\nIngrese el tamaño del terreno (N x N), por ejemplo, 5: ");
                if (int.TryParse(Console.ReadLine(), out n) && n > 0)
                {
                    return n;
                }
                Console.WriteLine("Entrada inválida. Por favor, ingrese un número entero positivo.");
            }
        }

        /// <summary>
        /// Solicita al usuario las coordenadas de inicio (X, Y).
        /// </summary>
        private static (int, int) GetStartCoordinates(int n)
        {
            int x, y;
            while (true)
            {
                Console.Write($"Ingrese la coordenada X de inicio (0 a {n - 1}): ");
                if (int.TryParse(Console.ReadLine(), out x) && x >= 0 && x < n)
                {
                    break;
                }
                Console.WriteLine("Coordenada X inválida.");
            }
            while (true)
            {
                Console.Write($"Ingrese la coordenada Y de inicio (0 a {n - 1}): ");
                if (int.TryParse(Console.ReadLine(), out y) && y >= 0 && y < n)
                {
                    break;
                }
                Console.WriteLine("Coordenada Y inválida.");
            }
            return (x, y);
        }

        /// <summary>
        /// Procesa la lista de parcelas visitadas y renderiza bidimensionalmente el terreno en la salida estándar.
        /// Cumple estrictamente con la Parte E del examen: inicializa una estructura matricial en memoria donde las 
        /// posiciones inalcanzables u omitidas se denotan con un punto ('.') y las posiciones visitadas reflejan 
        /// su orden cronológico exacto de aterrizaje.
        /// La complejidad temporal de la representación es O(N^2) debido al recorrido completo del espacio muestral.
        /// </summary>
        /// <param name="n">Dimensión escalar N del tablero cuadrado (N x N).</param>
        /// <param name="camino">Lista secuencial que contiene los movimientos atómicos validados por el algoritmo.</param>
        private static void DibujarMatriz(int n, List<Parcela> camino)
        {
            Console.WriteLine("\n--- Matriz del Recorrido ---");
            
            // Inicialización de la matriz visual asumiendo el peor caso temporal (ninguna celda alcanzable)
            string[,] matrizConsola = new string[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    matrizConsola[i, j] = " . ";
                }
            }

            // Proyección del historial lógico sobre la matriz espacial.
            // Nota de diseño: Se aplica un desplazamiento negativo (-1) al contador de pasos para
            // satisfacer el requerimiento de la cátedra de iniciar la secuencia en 0 (despegue).
            foreach (var parcela in camino)
            {
                matrizConsola[parcela.X, parcela.Y] = (parcela.Paso - 1).ToString().PadLeft(2, ' ') + " ";
            }

            // Volcado secuencial en consola
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Console.Write(matrizConsola[i, j]);
                }
                Console.WriteLine();
            }
        }
    }
}