using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RuokalistaServer.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddVoteDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Votes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ruokalistaId = table.Column<int>(type: "int", nullable: false),
                    level1_votes_maanantai = table.Column<double>(type: "float", nullable: true),
                    level2_votes_maanantai = table.Column<double>(type: "float", nullable: true),
                    level3_votes_maanantai = table.Column<double>(type: "float", nullable: true),
                    level4_votes_maanantai = table.Column<double>(type: "float", nullable: true),
                    level1_votes_tiistai = table.Column<double>(type: "float", nullable: true),
                    level2_votes_tiistai = table.Column<double>(type: "float", nullable: true),
                    level3_votes_tiistai = table.Column<double>(type: "float", nullable: true),
                    level4_votes_tiistai = table.Column<double>(type: "float", nullable: true),
                    level1_votes_keskiviikko = table.Column<double>(type: "float", nullable: true),
                    level2_votes_keskiviikko = table.Column<double>(type: "float", nullable: true),
                    level3_votes_keskiviikko = table.Column<double>(type: "float", nullable: true),
                    level4_votes_keskiviikko = table.Column<double>(type: "float", nullable: true),
                    level1_votes_torstai = table.Column<double>(type: "float", nullable: true),
                    level2_votes_torstai = table.Column<double>(type: "float", nullable: true),
                    level3_votes_torstai = table.Column<double>(type: "float", nullable: true),
                    level4_votes_torstai = table.Column<double>(type: "float", nullable: true),
                    level1_votes_perjantai = table.Column<double>(type: "float", nullable: true),
                    level2_votes_perjantai = table.Column<double>(type: "float", nullable: true),
                    level3_votes_perjantai = table.Column<double>(type: "float", nullable: true),
                    level4_votes_perjantai = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votes", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Votes");
        }
    }
}
