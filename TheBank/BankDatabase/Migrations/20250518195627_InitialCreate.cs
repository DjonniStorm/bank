using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankDatabase.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clerks",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Surname = table.Column<string>(type: "text", nullable: false),
                    MiddleName = table.Column<string>(type: "text", nullable: false),
                    Login = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clerks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Storekeepers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Surname = table.Column<string>(type: "text", nullable: false),
                    MiddleName = table.Column<string>(type: "text", nullable: false),
                    Login = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Storekeepers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Surname = table.Column<string>(type: "text", nullable: false),
                    Balance = table.Column<decimal>(type: "numeric", nullable: false),
                    ClerkId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clients_Clerks_ClerkId",
                        column: x => x.ClerkId,
                        principalTable: "Clerks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Deposits",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    InterestRate = table.Column<float>(type: "real", nullable: false),
                    Cost = table.Column<decimal>(type: "numeric", nullable: false),
                    Period = table.Column<int>(type: "integer", nullable: false),
                    ClerkId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deposits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Deposits_Clerks_ClerkId",
                        column: x => x.ClerkId,
                        principalTable: "Clerks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Abbreviation = table.Column<string>(type: "text", nullable: false),
                    Cost = table.Column<decimal>(type: "numeric", nullable: false),
                    StorekeeperId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Currencies_Storekeepers_StorekeeperId",
                        column: x => x.StorekeeperId,
                        principalTable: "Storekeepers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Periods",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StorekeeperId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Periods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Periods_Storekeepers_StorekeeperId",
                        column: x => x.StorekeeperId,
                        principalTable: "Storekeepers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DepositClients",
                columns: table => new
                {
                    DepositId = table.Column<string>(type: "text", nullable: false),
                    ClientId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepositClients", x => new { x.DepositId, x.ClientId });
                    table.ForeignKey(
                        name: "FK_DepositClients_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DepositClients_Deposits_DepositId",
                        column: x => x.DepositId,
                        principalTable: "Deposits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Replenishments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DepositId = table.Column<string>(type: "text", nullable: false),
                    ClerkId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Replenishments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Replenishments_Clerks_ClerkId",
                        column: x => x.ClerkId,
                        principalTable: "Clerks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Replenishments_Deposits_DepositId",
                        column: x => x.DepositId,
                        principalTable: "Deposits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DepositCurrencies",
                columns: table => new
                {
                    DepositId = table.Column<string>(type: "text", nullable: false),
                    CurrencyId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepositCurrencies", x => new { x.DepositId, x.CurrencyId });
                    table.ForeignKey(
                        name: "FK_DepositCurrencies_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DepositCurrencies_Deposits_DepositId",
                        column: x => x.DepositId,
                        principalTable: "Deposits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreditPrograms",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Cost = table.Column<decimal>(type: "numeric", nullable: false),
                    MaxCost = table.Column<decimal>(type: "numeric", nullable: false),
                    StorekeeperId = table.Column<string>(type: "text", nullable: false),
                    PeriodId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditPrograms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditPrograms_Periods_PeriodId",
                        column: x => x.PeriodId,
                        principalTable: "Periods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CreditPrograms_Storekeepers_StorekeeperId",
                        column: x => x.StorekeeperId,
                        principalTable: "Storekeepers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreditProgramClients",
                columns: table => new
                {
                    ClientId = table.Column<string>(type: "text", nullable: false),
                    CreditProgramId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditProgramClients", x => new { x.ClientId, x.CreditProgramId });
                    table.ForeignKey(
                        name: "FK_CreditProgramClients_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CreditProgramClients_CreditPrograms_CreditProgramId",
                        column: x => x.CreditProgramId,
                        principalTable: "CreditPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CurrencyCreditPrograms",
                columns: table => new
                {
                    CreditProgramId = table.Column<string>(type: "text", nullable: false),
                    CurrencyId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyCreditPrograms", x => new { x.CreditProgramId, x.CurrencyId });
                    table.ForeignKey(
                        name: "FK_CurrencyCreditPrograms_CreditPrograms_CreditProgramId",
                        column: x => x.CreditProgramId,
                        principalTable: "CreditPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CurrencyCreditPrograms_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clerks_Email",
                table: "Clerks",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clerks_Login",
                table: "Clerks",
                column: "Login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clerks_PhoneNumber",
                table: "Clerks",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_ClerkId",
                table: "Clients",
                column: "ClerkId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditProgramClients_CreditProgramId",
                table: "CreditProgramClients",
                column: "CreditProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditPrograms_Name",
                table: "CreditPrograms",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CreditPrograms_PeriodId",
                table: "CreditPrograms",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditPrograms_StorekeeperId",
                table: "CreditPrograms",
                column: "StorekeeperId");

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_Abbreviation",
                table: "Currencies",
                column: "Abbreviation",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_StorekeeperId",
                table: "Currencies",
                column: "StorekeeperId");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyCreditPrograms_CurrencyId",
                table: "CurrencyCreditPrograms",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_DepositClients_ClientId",
                table: "DepositClients",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_DepositCurrencies_CurrencyId",
                table: "DepositCurrencies",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Deposits_ClerkId",
                table: "Deposits",
                column: "ClerkId");

            migrationBuilder.CreateIndex(
                name: "IX_Periods_StorekeeperId",
                table: "Periods",
                column: "StorekeeperId");

            migrationBuilder.CreateIndex(
                name: "IX_Replenishments_ClerkId",
                table: "Replenishments",
                column: "ClerkId");

            migrationBuilder.CreateIndex(
                name: "IX_Replenishments_DepositId",
                table: "Replenishments",
                column: "DepositId");

            migrationBuilder.CreateIndex(
                name: "IX_Storekeepers_Email",
                table: "Storekeepers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Storekeepers_Login",
                table: "Storekeepers",
                column: "Login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Storekeepers_PhoneNumber",
                table: "Storekeepers",
                column: "PhoneNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CreditProgramClients");

            migrationBuilder.DropTable(
                name: "CurrencyCreditPrograms");

            migrationBuilder.DropTable(
                name: "DepositClients");

            migrationBuilder.DropTable(
                name: "DepositCurrencies");

            migrationBuilder.DropTable(
                name: "Replenishments");

            migrationBuilder.DropTable(
                name: "CreditPrograms");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.DropTable(
                name: "Deposits");

            migrationBuilder.DropTable(
                name: "Periods");

            migrationBuilder.DropTable(
                name: "Clerks");

            migrationBuilder.DropTable(
                name: "Storekeepers");
        }
    }
}
