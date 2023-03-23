#nullable disable

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DA.Migrations
{
 /// <inheritdoc />
 public partial class v20 : Migration
 {
  /// <inheritdoc />
  protected override void Up(MigrationBuilder migrationBuilder)
  {
   migrationBuilder.AddColumn<Guid>(
name: "UserGUID",
table: "User",
type: "uniqueidentifier",
nullable: false,
defaultValueSql: "newId()");
  }

  /// <inheritdoc />
  protected override void Down(MigrationBuilder migrationBuilder)
  {
   migrationBuilder.DropColumn(
       name: "UserGUID",
       table: "User");


  }
 }
}
