using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneyPilot.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRecurringTransactionEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RecurringTransactionId",
                table: "Expenses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RecurringTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    RecurrenceType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Interval = table.Column<int>(type: "int", nullable: false),
                    DayOfWeek = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DayOfMonth = table.Column<int>(type: "int", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextOccurrence = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurringTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecurringTransactions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecurringTransactions_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_RecurringTransactionId",
                table: "Expenses",
                column: "RecurringTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringTransactions_CategoryId",
                table: "RecurringTransactions",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringTransactions_UserId_NextOccurrence_IsActive",
                table: "RecurringTransactions",
                columns: new[] { "UserId", "NextOccurrence", "IsActive" });

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_RecurringTransactions_RecurringTransactionId",
                table: "Expenses",
                column: "RecurringTransactionId",
                principalTable: "RecurringTransactions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_RecurringTransactions_RecurringTransactionId",
                table: "Expenses");

            migrationBuilder.DropTable(
                name: "RecurringTransactions");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_RecurringTransactionId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "RecurringTransactionId",
                table: "Expenses");
        }
    }
}
