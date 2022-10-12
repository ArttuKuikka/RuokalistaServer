using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RuokalistaServer.Data.Migrations
{
    public partial class ruokalistadd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ruokalista",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WeekId = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Maanantai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tiistai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Keskiviikko = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Torstai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Perjantai = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ruokalista", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ruokalista");
        }
    }
}
