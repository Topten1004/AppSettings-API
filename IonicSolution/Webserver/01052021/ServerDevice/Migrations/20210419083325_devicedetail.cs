using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ServerDevice.Migrations
{
    public partial class devicedetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeviceDetail",
                columns: table => new
                {
                    strDeviceId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    strDeviceIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    strPassword = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    strVolume = table.Column<float>(type: "real", nullable: false),
                    strShowDubugPanel = table.Column<bool>(type: "bit", nullable: false),
                    boolUseSampling = table.Column<bool>(type: "bit", nullable: false),
                    boolIsOnline = table.Column<bool>(type: "bit", nullable: false),
                    bytFaceDetectionQuality = table.Column<byte>(type: "tinyint", nullable: false),
                    bytFaceDetectionMode = table.Column<byte>(type: "tinyint", nullable: false),
                    ushortFaceDetectionFrameRateMaximum = table.Column<int>(type: "int", nullable: false),
                    boolQrCodeActive = table.Column<bool>(type: "bit", nullable: false),
                    byteDetectionCount = table.Column<byte>(type: "tinyint", nullable: false),
                    dateClientDateTimeTicks = table.Column<DateTime>(type: "datetime2", nullable: false),
                    dateServerDateTimeTicks = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceDetail", x => x.strDeviceId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceDetail");
        }
    }
}
