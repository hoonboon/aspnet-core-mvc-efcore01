using Microsoft.EntityFrameworkCore.Migrations;

namespace AspnetCoreWebMvcApp03.Migrations
{
    public partial class IdentityNagivationProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey("PK_AppUserToken", "AppUserToken");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AppUserToken",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AppUserToken",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                "PK_AppUserToken", "AppUserToken", 
                new string[] { "UserId", "LoginProvider", "Name" });


            migrationBuilder.DropPrimaryKey("PK_AppUserLogin", "AppUserLogin");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AppUserLogin",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AppUserLogin",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                "PK_AppUserLogin", "AppUserLogin",
                new string[] { "LoginProvider", "ProviderKey" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey("PK_AppUserToken", "AppUserToken");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AppUserToken",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AppUserToken",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 128);

            migrationBuilder.AddPrimaryKey(
                "PK_AppUserToken", "AppUserToken",
                new string[] { "UserId", "LoginProvider", "Name" });


            migrationBuilder.DropPrimaryKey("PK_AppUserLogin", "AppUserLogin");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AppUserLogin",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AppUserLogin",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 128);

            migrationBuilder.AddPrimaryKey(
                "PK_AppUserLogin", "AppUserLogin",
                new string[] { "LoginProvider", "ProviderKey" });
        }
    }
}
