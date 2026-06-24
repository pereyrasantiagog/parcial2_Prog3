namespace parcial2_Prog3.Models
{
    /// <summary>
    /// Representa una parcela individual en el terreno del dron.
    /// Almacena las coordenadas (X, Y) y el número de paso cuando el dron la visita.
    /// Es una estructura de datos fundamental para el algoritmo de backtracking.
    /// </summary>
    public class Parcela
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Paso { get; set; }

        public Parcela(int x, int y, int paso = 0)
        {
            X = x;
            Y = y;
            Paso = paso;
        }
    }
}
