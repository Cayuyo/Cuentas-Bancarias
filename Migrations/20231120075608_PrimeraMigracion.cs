using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cuentas_Bancarias.Migrations
{
    public partial class PrimeraMigracion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Apellido = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Correo = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Contrasena = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Fecha_Creacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Fecha_Modificacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.UsuarioId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
