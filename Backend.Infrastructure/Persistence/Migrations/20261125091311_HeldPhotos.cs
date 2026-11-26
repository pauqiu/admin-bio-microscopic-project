using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace UCR.EB.BioMicroscopeAdmin.Backend.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class HeldPhotos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "foto_pendiente",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    url_archivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    nombre_archivo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    fecha_carga = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    pila = table.Column<int>(type: "integer", nullable: true),
                    posicion = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_foto_pendiente", x => x.id);
                    table.CheckConstraint("CK_foto_pendiente_posicion", "posicion IS NULL OR (posicion >= 1 AND posicion <= 6)");
                });

            migrationBuilder.CreateIndex(
                name: "IX_foto_pendiente_pila_posicion",
                table: "foto_pendiente",
                columns: new[] { "pila", "posicion" },
                unique: true,
                filter: "pila IS NOT NULL AND posicion IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "foto_pendiente");
        }
    }
}
