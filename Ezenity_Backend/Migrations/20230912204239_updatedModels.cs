using Microsoft.EntityFrameworkCore.Migrations;

namespace Ezenity_Backend.Migrations
{
    /// <summary>
    /// This class represents a database migration that updates the models by renaming
    /// the "Role" table to "Roles" and updating foreign key references.
    /// </summary>
    public partial class updatedModels : Migration
    {
        /// <summary>
        /// Applies the changes to the database schema by updating table and foreign key names.
        /// </summary>
        /// <param name="migrationBuilder">An instance of the MigrationBuilder class used for applying migrations.</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Role_RoleId",
                table: "Accounts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Role",
                table: "Role");

            migrationBuilder.RenameTable(
                name: "Role",
                newName: "Roles");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Roles",
                table: "Roles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Roles_RoleId",
                table: "Accounts",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <summary>
        /// Reverts the changes applied in the Up() method, effectively rolling back this migration.
        /// </summary>
        /// <param name="migrationBuilder">An instance of the MigrationBuilder class used for reverting migrations.</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Roles_RoleId",
                table: "Accounts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Roles",
                table: "Roles");

            migrationBuilder.RenameTable(
                name: "Roles",
                newName: "Role");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Role",
                table: "Role",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Role_RoleId",
                table: "Accounts",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
