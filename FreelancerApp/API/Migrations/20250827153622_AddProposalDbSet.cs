using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddProposalDbSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proposal_AspNetUsers_ClientUserId",
                table: "Proposal");

            migrationBuilder.DropForeignKey(
                name: "FK_Proposal_AspNetUsers_FreelancerUserId",
                table: "Proposal");

            migrationBuilder.DropForeignKey(
                name: "FK_Proposal_Photos_PhotoId",
                table: "Proposal");

            migrationBuilder.DropForeignKey(
                name: "FK_Proposal_Projects_ProjectId",
                table: "Proposal");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Proposal",
                table: "Proposal");

            migrationBuilder.RenameTable(
                name: "Proposal",
                newName: "Proposals");

            migrationBuilder.RenameIndex(
                name: "IX_Proposal_ProjectId",
                table: "Proposals",
                newName: "IX_Proposals_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Proposal_PhotoId",
                table: "Proposals",
                newName: "IX_Proposals_PhotoId");

            migrationBuilder.RenameIndex(
                name: "IX_Proposal_FreelancerUserId",
                table: "Proposals",
                newName: "IX_Proposals_FreelancerUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Proposal_ClientUserId",
                table: "Proposals",
                newName: "IX_Proposals_ClientUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Proposals",
                table: "Proposals",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Proposals_AspNetUsers_ClientUserId",
                table: "Proposals",
                column: "ClientUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Proposals_AspNetUsers_FreelancerUserId",
                table: "Proposals",
                column: "FreelancerUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Proposals_Photos_PhotoId",
                table: "Proposals",
                column: "PhotoId",
                principalTable: "Photos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Proposals_Projects_ProjectId",
                table: "Proposals",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proposals_AspNetUsers_ClientUserId",
                table: "Proposals");

            migrationBuilder.DropForeignKey(
                name: "FK_Proposals_AspNetUsers_FreelancerUserId",
                table: "Proposals");

            migrationBuilder.DropForeignKey(
                name: "FK_Proposals_Photos_PhotoId",
                table: "Proposals");

            migrationBuilder.DropForeignKey(
                name: "FK_Proposals_Projects_ProjectId",
                table: "Proposals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Proposals",
                table: "Proposals");

            migrationBuilder.RenameTable(
                name: "Proposals",
                newName: "Proposal");

            migrationBuilder.RenameIndex(
                name: "IX_Proposals_ProjectId",
                table: "Proposal",
                newName: "IX_Proposal_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Proposals_PhotoId",
                table: "Proposal",
                newName: "IX_Proposal_PhotoId");

            migrationBuilder.RenameIndex(
                name: "IX_Proposals_FreelancerUserId",
                table: "Proposal",
                newName: "IX_Proposal_FreelancerUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Proposals_ClientUserId",
                table: "Proposal",
                newName: "IX_Proposal_ClientUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Proposal",
                table: "Proposal",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Proposal_AspNetUsers_ClientUserId",
                table: "Proposal",
                column: "ClientUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Proposal_AspNetUsers_FreelancerUserId",
                table: "Proposal",
                column: "FreelancerUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Proposal_Photos_PhotoId",
                table: "Proposal",
                column: "PhotoId",
                principalTable: "Photos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Proposal_Projects_ProjectId",
                table: "Proposal",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
