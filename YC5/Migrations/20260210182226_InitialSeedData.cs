using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace YC5.Migrations
{
    /// <inheritdoc />
    public partial class InitialSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Functions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Code", "Name" },
                values: new object[] { "STUDENT_VIEW", "Xem danh sách sinh viên" });

            migrationBuilder.UpdateData(
                table: "Functions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Code", "Name" },
                values: new object[] { "STUDENT_CREATE", "Thêm mới sinh viên" });

            migrationBuilder.UpdateData(
                table: "Functions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Code", "Name" },
                values: new object[] { "STUDENT_UPDATE", "Cập nhật sinh viên" });

            migrationBuilder.InsertData(
                table: "Functions",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[,]
                {
                    { 4, "STUDENT_DELETE", "Xóa sinh viên" },
                    { 5, "STUDENT_IMPORT", "Import sinh viên" },
                    { 6, "STUDENT_EXPORT", "Export sinh viên" }
                });

            migrationBuilder.InsertData(
                table: "RoleFunctions",
                columns: new[] { "FunctionId", "RoleId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 },
                    { 3, 1 },
                    { 4, 1 },
                    { 5, 1 },
                    { 6, 1 }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Administrator" },
                    { 2, "User" }
                });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "Class", "DateOfBirth", "Email", "FullName", "MSSV", "Major", "Phone" },
                values: new object[,]
                {
                    { 1, "D15-01", new DateTime(2002, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "an.nv@gmail.com", "Nguyễn Văn An", "SV001", "CNTT", "0912345678" },
                    { 2, "K15-02", new DateTime(2002, 10, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "binh.tt@gmail.com", "Trần Thị Bình", "SV002", "Kế toán", "0988776655" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Functions",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Functions",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Functions",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "RoleFunctions",
                keyColumns: new[] { "FunctionId", "RoleId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "RoleFunctions",
                keyColumns: new[] { "FunctionId", "RoleId" },
                keyValues: new object[] { 2, 1 });

            migrationBuilder.DeleteData(
                table: "RoleFunctions",
                keyColumns: new[] { "FunctionId", "RoleId" },
                keyValues: new object[] { 3, 1 });

            migrationBuilder.DeleteData(
                table: "RoleFunctions",
                keyColumns: new[] { "FunctionId", "RoleId" },
                keyValues: new object[] { 4, 1 });

            migrationBuilder.DeleteData(
                table: "RoleFunctions",
                keyColumns: new[] { "FunctionId", "RoleId" },
                keyValues: new object[] { 5, 1 });

            migrationBuilder.DeleteData(
                table: "RoleFunctions",
                keyColumns: new[] { "FunctionId", "RoleId" },
                keyValues: new object[] { 6, 1 });

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.UpdateData(
                table: "Functions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Code", "Name" },
                values: new object[] { "STUDENT_IMPORT", "Import Sinh Viên" });

            migrationBuilder.UpdateData(
                table: "Functions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Code", "Name" },
                values: new object[] { "STUDENT_EXPORT", "Export Sinh Viên" });

            migrationBuilder.UpdateData(
                table: "Functions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Code", "Name" },
                values: new object[] { "STUDENT_VIEW", "Xem Sinh Viên" });
        }
    }
}
