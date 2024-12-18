﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RuokalistaServer.Data.Migrations
{
    /// <inheritdoc />
    public partial class Kasvisruokalista : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "BackgroundForWeek",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Kasvisruokalista",
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
                    table.PrimaryKey("PK_Kasvisruokalista", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Kasvisruokalista");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "BackgroundForWeek");
        }
    }
}
