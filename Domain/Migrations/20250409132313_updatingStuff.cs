using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domain.Migrations
{
    /// <inheritdoc />
    public partial class updatingStuff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GamePlatforms_Platforms_PlatformId",
                table: "GamePlatforms");

            migrationBuilder.DropForeignKey(
                name: "FK_GamePlatforms_Projects_ProjectId",
                table: "GamePlatforms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GamePlatforms",
                table: "GamePlatforms");

            migrationBuilder.RenameTable(
                name: "GamePlatforms",
                newName: "ProjectPlatforms");

            migrationBuilder.RenameIndex(
                name: "IX_GamePlatforms_ProjectId",
                table: "ProjectPlatforms",
                newName: "IX_ProjectPlatforms_ProjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectPlatforms",
                table: "ProjectPlatforms",
                columns: new[] { "PlatformId", "ProjectId" });

            // 🔧 Fix or remove invalid data before applying the constraint
            migrationBuilder.Sql(@"
                UPDATE ""Categories""
                SET ""ParentCategoryId"" = NULL
                WHERE ""CategoryId"" = ""ParentCategoryId"";
            ");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Category_NoCircularDependency",
                table: "Categories",
                sql: "\"CategoryId\" != \"ParentCategoryId\"");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectPlatforms_Platforms_PlatformId",
                table: "ProjectPlatforms",
                column: "PlatformId",
                principalTable: "Platforms",
                principalColumn: "PlatformId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectPlatforms_Projects_ProjectId",
                table: "ProjectPlatforms",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "ProjectId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectPlatforms_Platforms_PlatformId",
                table: "ProjectPlatforms");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectPlatforms_Projects_ProjectId",
                table: "ProjectPlatforms");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Category_NoCircularDependency",
                table: "Categories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectPlatforms",
                table: "ProjectPlatforms");

            migrationBuilder.RenameTable(
                name: "ProjectPlatforms",
                newName: "GamePlatforms");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectPlatforms_ProjectId",
                table: "GamePlatforms",
                newName: "IX_GamePlatforms_ProjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GamePlatforms",
                table: "GamePlatforms",
                columns: new[] { "PlatformId", "ProjectId" });

            migrationBuilder.AddForeignKey(
                name: "FK_GamePlatforms_Platforms_PlatformId",
                table: "GamePlatforms",
                column: "PlatformId",
                principalTable: "Platforms",
                principalColumn: "PlatformId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GamePlatforms_Projects_ProjectId",
                table: "GamePlatforms",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "ProjectId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
