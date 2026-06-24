-- Script DDL para PostgreSQL
-- Creación de la tabla Maestra de Control

CREATE TABLE tb_master_control (
    id SERIAL PRIMARY KEY,
    nombre_alumno VARCHAR(100) NOT NULL,
    apellido_alumno VARCHAR(100) NOT NULL,
    dni_alumno VARCHAR(20) NOT NULL,
    nombre_materia VARCHAR(100) NOT NULL,
    fecha_creacion TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    path_solucion_final TEXT
);

-- Creación de la tabla de Detalle de Log del Dron

CREATE TABLE tb_det_log (
    id SERIAL PRIMARY KEY,
    id_master INT NOT NULL,
    paso_nro INT NOT NULL,
    posicion_x INT NOT NULL,
    posicion_y INT NOT NULL,
    fecha_creacion TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_master_control
        FOREIGN KEY(id_master) 
        REFERENCES tb_master_control(id)
        ON DELETE CASCADE
);
