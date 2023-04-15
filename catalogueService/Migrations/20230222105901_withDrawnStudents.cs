using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace catalogueService.Migrations
{
    public partial class withDrawnStudents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "withdrawnStudents",
                columns: table => new
                {
                    regNo = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    firstName = table.Column<string>(nullable: true),
                    lastName = table.Column<string>(nullable: true),
                    emailAddress = table.Column<string>(nullable: true),
                    phoneNo = table.Column<string>(nullable: true),
                    dateOfBirth = table.Column<DateTime>(nullable: false),
                    gender = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    enrollmentStatus = table.Column<string>(nullable: true),
                    program = table.Column<string>(nullable: true),
                    GPA = table.Column<int>(nullable: true),
                    coursesTaken = table.Column<string>(nullable: true),
                    registrationDate = table.Column<string>(nullable: true),
                    graduationDate = table.Column<string>(nullable: true),
                    userId = table.Column<int>(nullable: false),
                    _usersuserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_withdrawnStudents", x => x.regNo);
                    table.ForeignKey(
                        name: "FK_withdrawnStudents_Users__usersuserId",
                        column: x => x._usersuserId,
                        principalTable: "Users",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_withdrawnStudents__usersuserId",
                table: "withdrawnStudents",
                column: "_usersuserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "withdrawnStudents");
        }
    }
}
