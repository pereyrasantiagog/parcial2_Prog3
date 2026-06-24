using Npgsql;
using System;
using System.Collections.Generic;
using parcial2_Prog3.Models;

namespace parcial2_Prog3.Data
{
    /// <summary>
    /// Repositorio encargado de toda la interacción con la base de datos PostgreSQL.
    /// Utiliza ADO.NET síncrono con Npgsql, conforme a las restricciones del examen.
    /// Abstrae las operaciones de inserción y consulta para que el resto de la aplicación
    /// no necesite conocer los detalles de la implementación de la base de datos.
    /// </summary>
    public class DatabaseRepository
    {
        private readonly string _connectionString;

        public DatabaseRepository()
        {
            _connectionString = Config.GetConnectionString();
        }

        /// <summary>
        /// Inserta un registro maestro de control y el detalle del recorrido del dron en una única transacción.
        /// Este método demuestra el manejo de transacciones con ADO.NET para garantizar la atomicidad de los datos.
        /// Si alguna de las inserciones falla, se revierte toda la operación.
        /// </summary>
        /// <param name="master">Datos del registro maestro.</param>
        /// <param name="path">La lista de parcelas que componen el recorrido.</param>
        public void InsertMasterAndDetail(MasterControl master, List<Parcela> path)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Insertar el registro maestro
                        // CORRECCIÓN: Usamos int en lugar de long para coincidir con el SERIAL de PostgreSQL
                        int masterId; 
                        using (var cmd = new NpgsqlCommand("INSERT INTO tb_master_control (nombre_alumno, apellido_alumno, dni_alumno, nombre_materia, path_solucion_final) VALUES (@nombre, @apellido, @dni, @materia, @path) RETURNING id", connection))
                        {
                            cmd.Parameters.AddWithValue("nombre", master.NombreAlumno);
                            cmd.Parameters.AddWithValue("apellido", master.ApellidoAlumno);
                            cmd.Parameters.AddWithValue("dni", master.DniAlumno);
                            cmd.Parameters.AddWithValue("materia", master.NombreMateria);
                            cmd.Parameters.AddWithValue("path", master.PathSolucionFinal);
                            
                            var result = cmd.ExecuteScalar();
                            if (result == null || result == DBNull.Value)
                            {
                                throw new Exception("No se pudo obtener el ID del registro maestro insertado.");
                            }
                            
                            // CORRECCIÓN CLAVE: Usar Convert.ToInt32 evita el error de casteo directo
                            masterId = Convert.ToInt32(result); 
                        }

                        // Insertar el detalle del recorrido (log del dron)
                        // Se utiliza un bucle 'while' como lo pide el enunciado.
                        int i = 0;
                        while (i < path.Count)
                        {
                            var parcela = path[i];
                            using (var cmd = new NpgsqlCommand("INSERT INTO tb_det_log (id_master, paso_nro, posicion_x, posicion_y) VALUES (@masterId, @paso, @x, @y)", connection))
                            {
                                int pasoOfuscado = parcela.Paso % 2 == 0 ? parcela.Paso * 2 : -parcela.Paso;
                                cmd.Parameters.AddWithValue("masterId", masterId);
                                cmd.Parameters.AddWithValue("paso", pasoOfuscado);
                                cmd.Parameters.AddWithValue("x", parcela.X);
                                cmd.Parameters.AddWithValue("y", parcela.Y);
                                cmd.ExecuteNonQuery();
                            }
                            i++;
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Error al insertar en la base de datos, se revirtió la transacción: " + ex.Message);
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Realiza la ingeniería inversa de los últimos 5 movimientos del dron.
        /// Consulta la base de datos, obtiene los pasos ofuscados y aplica la lógica inversa
        /// para mostrar los números de paso originales en la consola.
        /// </summary>
        public void ReverseEngineerLast5Steps()
        {
            Console.WriteLine("\n--- Ingeniería Inversa de los Últimos 5 Pasos ---");
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand("SELECT paso_nro, posicion_x, posicion_y FROM tb_det_log ORDER BY id DESC LIMIT 5", connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int pasoOfuscado = reader.GetInt32(0);
                        int x = reader.GetInt32(1);
                        int y = reader.GetInt32(2);

                        int pasoReal;
                        if (pasoOfuscado < 0)
                        {
                            pasoReal = -pasoOfuscado;
                        }
                        else
                        {
                            pasoReal = pasoOfuscado / 2;
                        }

                        Console.WriteLine($"Paso Real: {pasoReal}, Posición: ({x}, {y})");
                    }
                }
            }
        }
    }

    /// <summary>
    /// Modelo que representa la tabla tb_master_control.
    /// </summary>
    public class MasterControl
    {
        public string NombreAlumno { get; set; } = string.Empty;
        public string ApellidoAlumno { get; set; } = string.Empty;
        public string DniAlumno { get; set; } = string.Empty;
        public string NombreMateria { get; set; } = string.Empty;
        public string PathSolucionFinal { get; set; } = string.Empty;
    }
}