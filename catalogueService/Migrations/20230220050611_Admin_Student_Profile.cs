using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace catalogueService.Migrations
{
    public partial class Admin_Student_Profile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    adminID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    firstName = table.Column<string>(nullable: true),
                    lastName = table.Column<string>(nullable: true),
                    emailAddress = table.Column<string>(nullable: true),
                    phoneNo = table.Column<string>(nullable: true),
                    dateOfBirth = table.Column<DateTime>(nullable: false),
                    gender = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    department = table.Column<string>(nullable: true),
                    departmentalRole = table.Column<string>(nullable: true),
                    accessLevel = table.Column<string>(nullable: true),
                    yearsOfExperience = table.Column<int>(nullable: true),
                    hireDate = table.Column<string>(nullable: true),
                    employmentStatus = table.Column<string>(nullable: true),
                    certifications = table.Column<string>(nullable: true),
                    isAdmin = table.Column<string>(nullable: true),
                    userId = table.Column<int>(nullable: false),
                    _usersuserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.adminID);
                    table.ForeignKey(
                        name: "FK_Admins_Users__usersuserId",
                        column: x => x._usersuserId,
                        principalTable: "Users",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Students",
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
                    table.PrimaryKey("PK_Students", x => x.regNo);
                    table.ForeignKey(
                        name: "FK_Students_Users__usersuserId",
                        column: x => x._usersuserId,
                        principalTable: "Users",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Teachers",
                columns: table => new
                {
                    teacherID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    firstName = table.Column<string>(nullable: true),
                    lastName = table.Column<string>(nullable: true),
                    emailAddress = table.Column<string>(nullable: true),
                    phoneNo = table.Column<string>(nullable: true),
                    dateOfBirth = table.Column<DateTime>(nullable: false),
                    gender = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    employmentStatus = table.Column<string>(nullable: true),
                    subjectTaught = table.Column<string>(nullable: true),
                    educationLevel = table.Column<string>(nullable: true),
                    yearsOfExperience = table.Column<int>(nullable: true),
                    registrationDate = table.Column<string>(nullable: true),
                    classSchedule = table.Column<string>(nullable: true),
                    courseList = table.Column<string>(nullable: true),
                    userId = table.Column<int>(nullable: false),
                    _usersuserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teachers", x => x.teacherID);
                    table.ForeignKey(
                        name: "FK_Teachers_Users__usersuserId",
                        column: x => x._usersuserId,
                        principalTable: "Users",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Admins__usersuserId",
                table: "Admins",
                column: "_usersuserId");

            migrationBuilder.CreateIndex(
                name: "IX_Students__usersuserId",
                table: "Students",
                column: "_usersuserId");

            migrationBuilder.CreateIndex(
                name: "IX_Teachers__usersuserId",
                table: "Teachers",
                column: "_usersuserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Teachers");
        }
    }
}
