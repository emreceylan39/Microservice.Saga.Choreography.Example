using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Stock.API.Migrations
{
    /// <inheritdoc />
    public partial class mig2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Stocks",
                columns: new[] { "Id", "CreatedDate", "ProductId", "Quantity" },
                values: new object[,]
                {
                    { new Guid("2f27a87d-6ce8-41ba-8a23-a122c02381c2"), new DateTime(2023, 11, 25, 0, 27, 1, 505, DateTimeKind.Local).AddTicks(7298), new Guid("4072df50-6141-44b2-b5aa-e322a519b172"), 55 },
                    { new Guid("4b750210-02ea-4cce-8349-62138757eacd"), new DateTime(2023, 11, 25, 0, 27, 1, 505, DateTimeKind.Local).AddTicks(7301), new Guid("d05a72b2-328f-49f1-9e05-8e2e582f4829"), 22 },
                    { new Guid("b41f59e4-a58d-448c-a4e6-de3c0f1184a4"), new DateTime(2023, 11, 25, 0, 27, 1, 505, DateTimeKind.Local).AddTicks(7285), new Guid("5e53191f-76ab-4147-a82a-320a1d146e3b"), 5 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: new Guid("2f27a87d-6ce8-41ba-8a23-a122c02381c2"));

            migrationBuilder.DeleteData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: new Guid("4b750210-02ea-4cce-8349-62138757eacd"));

            migrationBuilder.DeleteData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: new Guid("b41f59e4-a58d-448c-a4e6-de3c0f1184a4"));
        }
    }
}
