using parcial2_Prog3.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace parcial2_Prog3.Algorithm
{
    /// <summary>
    /// Contiene la lógica principal para resolver el recorrido del dron.
    /// Implementa un algoritmo de backtracking recursivo con una heurística de "menor grado"
    /// para encontrar un camino que cubra todas las parcelas del terreno.
    /// </summary>
    public class DronAlgorithm
    {
        private int[,] _terreno;
        private int _tamano;
        private List<Parcela> _camino;
        
        // NUEVO: Variable para almacenar el total de parcelas físicamente alcanzables
        private int _objetivoAlcanzables; 

        // Movimientos posibles del dron en forma de "L" (2+1).
        // Son 8 movimientos posibles desde una posición (x, y).
        private readonly int[] _movimientosX = { 1, 1, 2, 2, -1, -1, -2, -2 };
        private readonly int[] _movimientosY = { 2, -2, 1, -1, 2, -2, 1, -1 };

        public DronAlgorithm(int tamano)
        {
            if (tamano <= 0)
            {
                throw new ArgumentException("El tamaño del terreno debe ser un entero positivo.", nameof(tamano));
            }
            _tamano = tamano;
            _terreno = new int[_tamano, _tamano];
            _camino = new List<Parcela>();
        }

        /// <summary>
        /// Método principal que inicia la búsqueda del recorrido.
        /// </summary>
        /// <param name="startX">Coordenada X inicial.</param>
        /// <param name="startY">Coordenada Y inicial.</param>
        /// <returns>Una lista de parcelas que representa el camino encontrado, o una lista vacía si no hay solución.</returns>
        public List<Parcela> EncontrarCamino(int startX, int startY)
        {
            if (!EsValido(startX, startY))
            {
                Console.WriteLine("Las coordenadas de inicio están fuera de los límites del terreno.");
                return new List<Parcela>();
            }

            // NUEVO: Calcular el límite de éxito evaluando el grafo de componentes conexos
            _objetivoAlcanzables = ContarAlcanzables(startX, startY);

            // Marcar la celda inicial como visitada con el paso 1.
            _terreno[startX, startY] = 1;
            _camino.Add(new Parcela(startX, startY, 1));

            if (ResolverRecursivo(startX, startY, 2))
            {
                return _camino;
            }

            return new List<Parcela>(); // No se encontró solución
        }

        /// <summary>
        /// El núcleo del algoritmo: la función recursiva de backtracking.
        /// </summary>
        /// <param name="x">Posición X actual.</param>
        /// <param name="y">Posición Y actual.</param>
        /// <param name="paso">El número del paso actual a intentar.</param>
        /// <returns>True si se encontró una solución desde este punto, de lo contrario False.</returns>
        private bool ResolverRecursivo(int x, int y, int paso)
        {
            // CORRECCIÓN CLAVE: La condición de éxito ahora se basa estrictamente en el número 
            // de parcelas físicamente alcanzables calculadas previamente, y no en el tamaño total del tablero.
            if (paso > _objetivoAlcanzables)
            {
                return true;
            }

            // HEURÍSTICA DE MENOR GRADO:
            // Antes de intentar un movimiento, obtenemos todos los destinos posibles y los ordenamos.
            // La ordenación se basa en el "grado" de cada destino, que es el número de salidas
            // libres que tiene. Se priorizan los destinos con menos salidas.
            // Esto reduce drásticamente el árbol de búsqueda y acelera la convergencia.
            var candidatos = ObtenerCandidatosOrdenados(x, y);

            foreach (var candidato in candidatos)
            {
                int nextX = candidato.X;
                int nextY = candidato.Y;

                // Intentar el movimiento
                _terreno[nextX, nextY] = paso;
                _camino.Add(new Parcela(nextX, nextY, paso));

                // Llamada recursiva para el siguiente paso
                if (ResolverRecursivo(nextX, nextY, paso + 1))
                {
                    return true; // ¡Solución encontrada!
                }

                // BACKTRACKING:
                // Si la llamada recursiva no llevó a una solución, deshacemos el movimiento.
                // Esto es crucial. Volvemos al estado anterior para probar otra rama del árbol de búsqueda.
                _terreno[nextX, nextY] = 0; // Marcar como no visitado
                _camino.RemoveAt(_camino.Count - 1); // Quitar del camino
            }

            return false; // No se encontró solución desde (x, y)
        }
        
        /// <summary>
        /// Obtiene y ordena los movimientos candidatos desde una posición dada según la heurística de menor grado.
        /// </summary>
        /// <param name="x">Posición X actual.</param>
        /// <param name="y">Posición Y actual.</param>
        /// <returns>Una lista de parcelas candidatas ordenadas por el número de salidas posibles (grado).</returns>
        private List<Parcela> ObtenerCandidatosOrdenados(int x, int y)
        {
            var candidatos = new List<Tuple<Parcela, int>>();

            for (int i = 0; i < 8; i++)
            {
                int nextX = x + _movimientosX[i];
                int nextY = y + _movimientosY[i];

                if (EsValido(nextX, nextY) && _terreno[nextX, nextY] == 0)
                {
                    int grado = CalcularGrado(nextX, nextY);
                    candidatos.Add(new Tuple<Parcela, int>(new Parcela(nextX, nextY), grado));
                }
            }

            // Ordenar por grado ascendente (menor grado primero)
            return candidatos.OrderBy(c => c.Item2).Select(c => c.Item1).ToList();
        }

        /// <summary>
        /// Calcula el "grado" de una celda, que es el número de movimientos válidos y no visitados desde ella.
        /// </summary>
        /// <param name="x">Coordenada X de la celda.</param>
        /// <param name="y">Coordenada Y de la celda.</param>
        /// <returns>El número de salidas (grado) de la celda.</returns>
        private int CalcularGrado(int x, int y)
        {
            int grado = 0;
            for (int i = 0; i < 8; i++)
            {
                int nextX = x + _movimientosX[i];
                int nextY = y + _movimientosY[i];
                if (EsValido(nextX, nextY) && _terreno[nextX, nextY] == 0)
                {
                    grado++;
                }
            }
            return grado;
        }

        /// <summary>
        /// Verifica si una coordenada (x, y) está dentro de los límites del terreno.
        /// </summary>
        private bool EsValido(int x, int y)
        {
            return x >= 0 && x < _tamano && y >= 0 && y < _tamano;
        }

        /// <summary>
        /// Pre-calcula la cantidad total de parcelas que son físicamente alcanzables desde la posición de despegue.
        /// Utiliza un algoritmo de Búsqueda en Amplitud (BFS - Breadth-First Search) para recorrer el grafo
        /// implícito de movimientos válidos del dron, identificando así el tamaño real de la componente conexa.
        /// Este valor es crítico para definir la condición de parada del backtracking en terrenos donde 
        /// el patrón de movimiento deja celdas aisladas.
        /// </summary>
        /// <param name="startX">Coordenada X inicial de despegue.</param>
        /// <param name="startY">Coordenada Y inicial de despegue.</param>
        /// <returns>La cantidad de celdas que el dron puede llegar a pisar.</returns>
        private int ContarAlcanzables(int startX, int startY)
        {
            var visitados = new bool[_tamano, _tamano];
            var cola = new Queue<Tuple<int, int>>();
            int count = 0;

            cola.Enqueue(new Tuple<int, int>(startX, startY));
            visitados[startX, startY] = true;

            while (cola.Count > 0)
            {
                var actual = cola.Dequeue();
                count++;

                for (int i = 0; i < 8; i++)
                {
                    int nextX = actual.Item1 + _movimientosX[i];
                    int nextY = actual.Item2 + _movimientosY[i];

                    if (EsValido(nextX, nextY) && !visitados[nextX, nextY])
                    {
                        visitados[nextX, nextY] = true;
                        cola.Enqueue(new Tuple<int, int>(nextX, nextY));
                    }
                }
            }
            return count;
        }
    }
}