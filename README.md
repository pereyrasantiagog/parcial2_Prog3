# Parcial 2 - Programación 3: Simulador de Trayectoria de Dron Automatizado

**Alumno:** Santiago Pereyra
**Materia:** Programación 3  

---

## 📌 Descripción del Proyecto

Este proyecto es una aplicación de consola en **.NET 8** que simula el sistema de navegación de un Dron de Inspección. El objetivo principal es calcular y registrar el recorrido de un dron sobre una cuadrícula de tamaño variable ($N \times N$), cumpliendo con un patrón de movimiento estricto (saltos en "L" de 2x1) y asegurando que pise todas las parcelas físicamente alcanzables sin repetir ninguna.

La aplicación integra algoritmia avanzada (Backtracking recursivo y heurísticas de optimización) con persistencia de datos relacional mediante el uso estricto de **ADO.NET síncrono**.

## 🛠️ Tecnologías Utilizadas

* **Lenguaje:** C# (.NET 8.0)
* **Base de Datos:** PostgreSQL
* **Acceso a Datos:** ADO.NET (Proveedor `Npgsql`)
* **Configuración:** `Microsoft.Extensions.Configuration`

---

## 🚀 Características Principales y Cumplimiento de Consignas

1. **Algoritmia y Lógica de Vuelo (Parte B):**
   * **Backtracking Recursivo:** Exploración profunda para encontrar la ruta óptima.
   * **Heurística de Menor Grado:** Implementación obligatoria que ordena los destinos candidatos priorizando aquellos con menos salidas libres (grado ascendente). Esto minimiza el retroceso algorítmico y permite resolver tableros complejos de forma eficiente.
   * **Búsqueda en Amplitud (BFS):** Cálculo dinámico previo a la recursión para determinar el número exacto de parcelas que componen la isla conexa (objetivo alcanzable).

2. **Infraestructura y Persistencia (Parte C y D):**
   * **ADO.NET Síncrono Puro:** Gestión manual de conexiones, comandos y parámetros, sin la utilización de ORMs ni métodos asíncronos.
   * **Transacciones:** Inserción atómica del maestro (`tb_master_control`) y su detalle (`tb_det_log`) protegida por `NpgsqlTransaction`.
   * **Ofuscación de Datos y Control de Flujo:** Persistencia secuencial de los movimientos implementando un ciclo `while` (prohibición de `for`/`foreach`). El número de paso se ofusca antes de su inserción (pasos pares se multiplican por 2; impares se guardan en negativo).

3. **Interfaz e Ingeniería Inversa (Parte E):**
   * **Representación Matricial:** Volcado visual en consola de la matriz de recorrido espacial en O(N²).
   * **Ingeniería Inversa "En Caliente":** Lectura de los últimos 5 movimientos insertados (`ORDER BY id DESC LIMIT 5`) con `ExecuteReader` y reconstrucción matemática de los pasos originales desofuscados.

---

## ⚙️ Instrucciones de Configuración y Ejecución

### 1. Requisitos Previos
* Tener instalado el SDK de .NET 8 o superior.
* Tener instalado y en ejecución un servidor PostgreSQL local.

### 2. Configuración de la Base de Datos
1. Abrir PostgreSQL (mediante pgAdmin, DBeaver o consola).
2. Crear una base de datos llamada `parcial_dron`.
3. Ejecutar el script DDL provisto en el archivo `Database.sql` para generar las tablas `tb_master_control` y `tb_det_log` con sus respectivas restricciones de clave foránea.

### 3. Configuración de la Aplicación
1. Abrir el archivo `appsettings.json` ubicado en la raíz del proyecto.
2. Modificar los valores de `User Id` y `Password` dentro de la sección `ConnectionStrings` para que coincidan con las credenciales locales de su motor PostgreSQL:
   ```json
   "ConnectionStrings": {
     "PostgreSqlConnection": "Server=localhost;Port=5432;Database=parcial_dron;User Id=TU_USUARIO;Password=TU_CONTRASEÑA;"
   }
