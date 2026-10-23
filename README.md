# ClientesAPI

API REST desarrollada en C# (.NET 10) con SQL Server, autenticación JWT y control de acceso por roles.

## Base de datos (SQL Server)

Script base para crear tablas e insertar datos iniciales:

```sql
CREATE TABLE clientes
(
    id          INT IDENTITY(1,1) PRIMARY KEY,
    apellidos   NVARCHAR(70)     NOT NULL,
    nombres     NVARCHAR(70)     NOT NULL,
    telefono    NVARCHAR(9)      NOT NULL,
    correo      NVARCHAR(150)    NOT NULL,
    dni         CHAR(8)          NOT NULL,
    direccion   NVARCHAR(150)    NOT NULL,
    CONSTRAINT uk_dni_cli UNIQUE (dni),
    CONSTRAINT ck_dni_cli CHECK (dni LIKE '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]')
);
GO

CREATE TABLE productos
(
    id          INT IDENTITY(1,1) PRIMARY KEY,
    nombre      NVARCHAR(100)    NOT NULL,
    descripcion NVARCHAR(100)    NOT NULL,
    precio      DECIMAL(7,2)     NOT NULL,
    garantia    TINYINT          NOT NULL,
    numserie    VARCHAR(40)      NULL
);
GO

INSERT INTO productos (nombre, descripcion, precio, garantia, numserie) VALUES
('Teclado', 'Logitech negro USB', 75, 12, 'ABC-12345'),
('Mouse', 'Genius USB', 40, 6, NULL);
GO
```

## Autenticación JWT

### Registro de usuario

`POST https://localhost:7281/api/auth/registrar`

Body:

```json
{
  "username": "jhon",
  "password": "clave123"
}
```

Respuesta:

```json
{
  "mensaje": "Usuario registrado correctamente"
}
```

### Login (obtener token)

`POST https://localhost:7281/api/auth/login`

Body:

```json
{
  "username": "jhon",
  "password": "clave123"
}
```

Respuesta:

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

## Pruebas de endpoints

### 1) Petición sin autorización

`GET https://localhost:7281/api/clientes`

Respuesta esperada: `401 Unauthorized`

### 2) Consulta con usuario normal

`GET https://localhost:7281/api/clientes`

Header:

```http
Authorization: Bearer <TOKEN_USUARIO>
```

Respuesta esperada: `200 OK` (lista de clientes).

### 3) Eliminar cliente (solo Admin)

`DELETE https://localhost:7281/api/clientes/1`

Pasos:

1. Asignar el rol `Admin` al usuario en la base de datos.
2. Hacer login nuevamente para generar un token con privilegios de admin.
3. Enviar el `DELETE` con el token de admin.

Header:

```http
Authorization: Bearer <TOKEN_ADMIN>
```

Respuestas posibles:

- Con rol Admin: `200 OK` / `204 No Content`
- Sin rol Admin: `403 Forbidden`
