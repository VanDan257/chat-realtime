using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace notip_server.Migrations
{
    /// <inheritdoc />
    public partial class add_loginuserhistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "UserTokens",
                type: "char(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(string),
                oldType: "char(36)")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "UserRoles",
                type: "char(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(string),
                oldType: "char(36)")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "UserLogins",
                type: "char(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(string),
                oldType: "char(36)")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "UserClaims",
                type: "char(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(string),
                oldType: "char(36)")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "User",
                type: "char(36)",
                unicode: false,
                maxLength: 32,
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldUnicode: false,
                oldMaxLength: 32)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "GroupCode",
                table: "Message",
                type: "char(36)",
                unicode: false,
                maxLength: 32,
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldUnicode: false,
                oldMaxLength: 32)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "Message",
                type: "char(36)",
                unicode: false,
                maxLength: 32,
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldUnicode: false,
                oldMaxLength: 32)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserCode",
                table: "GroupUser",
                type: "char(36)",
                unicode: false,
                maxLength: 32,
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldUnicode: false,
                oldMaxLength: 32)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "GroupCode",
                table: "GroupUser",
                type: "char(36)",
                unicode: false,
                maxLength: 32,
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldUnicode: false,
                oldMaxLength: 32)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "GroupCall",
                type: "char(36)",
                unicode: false,
                maxLength: 32,
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldUnicode: false,
                oldMaxLength: 32)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "Code",
                table: "GroupCall",
                type: "char(36)",
                unicode: false,
                maxLength: 32,
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldUnicode: false,
                oldMaxLength: 32)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "Group",
                type: "char(36)",
                unicode: false,
                maxLength: 32,
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldUnicode: false,
                oldMaxLength: 32)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "Code",
                table: "Group",
                type: "char(36)",
                unicode: false,
                maxLength: 32,
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldUnicode: false,
                oldMaxLength: 32)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "SenderCode",
                table: "Friend",
                type: "char(36)",
                unicode: false,
                maxLength: 32,
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldUnicode: false,
                oldMaxLength: 32)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReceiverCode",
                table: "Friend",
                type: "char(36)",
                unicode: false,
                maxLength: 32,
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldUnicode: false,
                oldMaxLength: 32)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserCode",
                table: "Call",
                type: "char(36)",
                unicode: false,
                maxLength: 32,
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldUnicode: false,
                oldMaxLength: 32)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "GroupCallCode",
                table: "Call",
                type: "char(36)",
                unicode: false,
                maxLength: 32,
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldUnicode: false,
                oldMaxLength: 32)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LoginUserHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    LoginTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginUserHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoginUserHistories_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_LoginUserHistories_UserId",
                table: "LoginUserHistories",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoginUserHistories");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserTokens",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserRoles",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserLogins",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserClaims",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "User",
                type: "char(36)",
                unicode: false,
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldUnicode: false,
                oldMaxLength: 32)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "GroupCode",
                table: "Message",
                type: "char(36)",
                unicode: false,
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldUnicode: false,
                oldMaxLength: 32)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Message",
                type: "char(36)",
                unicode: false,
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldUnicode: false,
                oldMaxLength: 32)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "UserCode",
                table: "GroupUser",
                type: "char(36)",
                unicode: false,
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldUnicode: false,
                oldMaxLength: 32)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "GroupCode",
                table: "GroupUser",
                type: "char(36)",
                unicode: false,
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldUnicode: false,
                oldMaxLength: 32)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "GroupCall",
                type: "char(36)",
                unicode: false,
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldUnicode: false,
                oldMaxLength: 32)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "GroupCall",
                type: "char(36)",
                unicode: false,
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldUnicode: false,
                oldMaxLength: 32)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Group",
                type: "char(36)",
                unicode: false,
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldUnicode: false,
                oldMaxLength: 32)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Group",
                type: "char(36)",
                unicode: false,
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldUnicode: false,
                oldMaxLength: 32)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "SenderCode",
                table: "Friend",
                type: "char(36)",
                unicode: false,
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldUnicode: false,
                oldMaxLength: 32)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ReceiverCode",
                table: "Friend",
                type: "char(36)",
                unicode: false,
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldUnicode: false,
                oldMaxLength: 32)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "UserCode",
                table: "Call",
                type: "char(36)",
                unicode: false,
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldUnicode: false,
                oldMaxLength: 32)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "GroupCallCode",
                table: "Call",
                type: "char(36)",
                unicode: false,
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldUnicode: false,
                oldMaxLength: 32)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");
        }
    }
}
