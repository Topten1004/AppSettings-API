using Microsoft.EntityFrameworkCore.Migrations;

namespace ServerDevice.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeviceDynamicProperties",
                columns: table => new
                {
                    PropertyId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayText = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: false),
                    PlaceHolder = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: false),
                    IsReadOnly = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceDynamicProperties", x => x.PropertyId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceDynamicProperties");
        }
    }
}
