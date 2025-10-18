# -_-_ProductApi-API_REST_con_.NET_8_y_Oracle_19C .
# 🧩 ProductApi – API REST con .NET 8 y Oracle 19c

API REST funcional desarrollada en **.NET 8 (Visual Studio 2022)** que:

✅ Recibe y envía **JSON** con los campos:
```json
{
  "id": 1,
  "producto": "Laptop Lenovo",
  "estadoProducto": "Disponible",
  "fechaTimestamp": "2025-10-18T15:35:20.123Z"
}
```

✅ Guarda los datos en **Oracle 19c** (local o cloud).  
✅ Alternativamente, puedes usar **bases de datos gratuitas y open source en la nube** como *Supabase*, *Neon*, *PlanetScale*, *Railway* o *MongoDB Atlas*.

---

## 🧱 Estructura del Proyecto

```
ProductApi/
│
├── Controllers/
│   └── ProductosController.cs
├── Models/
│   └── Producto.cs
├── Data/
│   └── OracleDbContext.cs
├── appsettings.json
└── Program.cs
```

---

## ⚙️ Paso 1: Crear el proyecto

En Visual Studio 2022 o terminal:

```bash
dotnet new webapi -n ProductApi
```

O mediante interfaz gráfica:

➡️ **Nuevo proyecto → “ASP.NET Core Web API” → Framework .NET 8.0**

---

## 📄 Paso 2: Modelo de Datos

Archivo: `Models/Producto.cs`

```csharp
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductApi.Models
{
    [Table("PRODUCTOS")]
    public class Producto
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }

        [Column("PRODUCTO")]
        [Required]
        public string? Nombre { get; set; }

        [Column("ESTADO_PRODUCTO")]
        [Required]
        public string? EstadoProducto { get; set; }

        [Column("FECHA_TIMESTAMP")]
        public DateTime FechaTimestamp { get; set; } = DateTime.Now;
    }
}
```

---

## ⚙️ Paso 3: Contexto de Oracle con Entity Framework Core

Instala los paquetes NuGet:

```bash
dotnet add package Oracle.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Tools
```

Archivo: `Data/OracleDbContext.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using ProductApi.Models;

namespace ProductApi.Data
{
    public class OracleDbContext : DbContext
    {
        public OracleDbContext(DbContextOptions<OracleDbContext> options)
            : base(options)
        {
        }

        public DbSet<Producto> Productos => Set<Producto>();
    }
}
```

---

## ⚙️ Paso 4: Configurar la Conexión

Archivo: `appsettings.json`

```json
{
  "ConnectionStrings": {
    "OracleDb": "User Id=admin;Password=tu_password;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=tu_host)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=tu_servicio)));"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

🔹 Ejemplo de `HOST`:

| Tipo | Host Ejemplo |
|------|---------------|
| Local | `localhost` |
| Oracle Cloud Free | `adb.us-ashburn-1.oraclecloud.com` |

---

## ⚙️ Paso 5: Configuración Principal (Program.cs)

Archivo: `Program.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using ProductApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Configuración DB
builder.Services.AddDbContext<OracleDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleDb")));

// Configuración API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

---

## 🧩 Paso 6: Controlador REST

Archivo: `Controllers/ProductosController.cs`

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductApi.Data;
using ProductApi.Models;

namespace ProductApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly OracleDbContext _context;

        public ProductosController(OracleDbContext context)
        {
            _context = context;
        }

        // GET api/productos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            return await _context.Productos.ToListAsync();
        }

        // GET api/productos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
                return NotFound();
            return producto;
        }

        // POST api/productos
        [HttpPost]
        public async Task<ActionResult<Producto>> PostProducto([FromBody] Producto producto)
        {
            producto.FechaTimestamp = DateTime.Now;
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProducto), new { id = producto.Id }, producto);
        }
    }
}
```

---

## 🧪 Paso 7: Probar con JSON

Ejemplo de petición **POST**  
URL: `https://localhost:5001/api/productos`

**Request**
```json
{
  "id": 1,
  "nombre": "Laptop Lenovo",
  "estadoProducto": "Disponible"
}
```

**Response**
```json
{
  "id": 1,
  "nombre": "Laptop Lenovo",
  "estadoProducto": "Disponible",
  "fechaTimestamp": "2025-10-18T15:35:20.123Z"
}
```

---

## 🌩️ Alternativas sin Oracle (gratuitas y open source)

| Servicio | Tipo | Plan Gratis | Duración |
|-----------|------|--------------|-----------|
| **Supabase** | PostgreSQL | ✅ | Permanente |
| **Neon.tech** | PostgreSQL | ✅ | Permanente |
| **MongoDB Atlas** | MongoDB | ✅ | Permanente |
| **Railway.app** | PostgreSQL/MySQL | ✅ | Renovable cada mes |
| **PlanetScale** | MySQL | ✅ | Permanente |

Para usar PostgreSQL, reemplaza la configuración en `Program.cs`:

```csharp
builder.Services.AddDbContext<OracleDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresDb")));
```

Y agrega el paquete:

```bash
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```

---

## 🚀 Resultado Final

Tu API estará disponible en:

👉 **Swagger UI:**  
`https://localhost:5001/swagger`

Desde ahí puedes:

- Enviar y recibir JSON.  
- Probar endpoints GET y POST.  
- Ver tus datos almacenados en Oracle o la base de datos cloud elegida.

---

## 🧠 Tecnologías utilizadas

- .NET 8.0  
- ASP.NET Core Web API  
- Entity Framework Core  
- Oracle Database 19c  
- Swagger UI  
- (Opcional) PostgreSQL / MySQL / MongoDB (alternativas cloud)

---

## 🧑‍💻 Autor

**Giovanny Tapiero Cataño**  
💼 GitHub: [giovaTC](https://github.com/giovaTC)  
🌐 Proyecto educativo y funcional para integrar **.NET + Oracle + Cloud DB**.

---

## 🖼️ Logo sugerido

Puedes usar este nombre en tu logo:

```
ProductApi – .NET + Oracle Cloud Ready 🚀
```
