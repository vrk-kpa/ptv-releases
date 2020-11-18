/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/
using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PTV.Database.Migrations.Migrations.Release_3_0
{
    public partial class JobLogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            Console.WriteLine($"{DateTime.UtcNow} - Adding tables related to Job status.");
            
            migrationBuilder.CreateTable(
                name: "JobExecutionType",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    LastOperationIdentifier = table.Column<Guid>(nullable: false),
                    LastOperationTimeStamp = table.Column<DateTime>(nullable: false),
                    LastOperationType = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    OrderNumber = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobExecutionType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobStatusType",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    LastOperationIdentifier = table.Column<Guid>(nullable: false),
                    LastOperationTimeStamp = table.Column<DateTime>(nullable: false),
                    LastOperationType = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    OrderNumber = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobStatusType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobExecutionTypeName",
                schema: "public",
                columns: table => new
                {
                    LocalizationId = table.Column<Guid>(nullable: false),
                    TypeId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    LastOperationIdentifier = table.Column<Guid>(nullable: false),
                    LastOperationTimeStamp = table.Column<DateTime>(nullable: false),
                    LastOperationType = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobExecutionTypeName", x => new { x.TypeId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_JobExecutionTypeName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobExecutionTypeName_JobExecutionType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "JobExecutionType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobSummary",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    LastOperationIdentifier = table.Column<Guid>(nullable: false),
                    LastOperationTimeStamp = table.Column<DateTime>(nullable: false),
                    LastOperationType = table.Column<int>(nullable: false),
                    OperationName = table.Column<string>(nullable: true),
                    StartUtc = table.Column<DateTime>(nullable: false),
                    JobType = table.Column<string>(nullable: true),
                    JobExecutionTypeId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSummary", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobSummary_JobExecutionType_JobExecutionTypeId",
                        column: x => x.JobExecutionTypeId,
                        principalSchema: "public",
                        principalTable: "JobExecutionType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobStatusTypeName",
                schema: "public",
                columns: table => new
                {
                    LocalizationId = table.Column<Guid>(nullable: false),
                    TypeId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    LastOperationIdentifier = table.Column<Guid>(nullable: false),
                    LastOperationTimeStamp = table.Column<DateTime>(nullable: false),
                    LastOperationType = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobStatusTypeName", x => new { x.TypeId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_JobStatusTypeName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobStatusTypeName_JobStatusType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "JobStatusType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobLog",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    LastOperationIdentifier = table.Column<Guid>(nullable: false),
                    LastOperationTimeStamp = table.Column<DateTime>(nullable: false),
                    LastOperationType = table.Column<int>(nullable: false),
                    TimeUtc = table.Column<DateTime>(nullable: false),
                    NlogLevel = table.Column<string>(nullable: true),
                    JobStatusTypeId = table.Column<Guid>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    Exception = table.Column<string>(nullable: true),
                    StackTrace = table.Column<string>(nullable: true),
                    JobSummaryId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobLog_JobStatusType_JobStatusTypeId",
                        column: x => x.JobStatusTypeId,
                        principalSchema: "public",
                        principalTable: "JobStatusType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobLog_JobSummary_JobSummaryId",
                        column: x => x.JobSummaryId,
                        principalSchema: "public",
                        principalTable: "JobSummary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobExeTyp_Id",
                schema: "public",
                table: "JobExecutionType",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_JobExeTypNam_LocalizationId",
                schema: "public",
                table: "JobExecutionTypeName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_JobExeTypNam_TypeId",
                schema: "public",
                table: "JobExecutionTypeName",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_JobLog_Id",
                schema: "public",
                table: "JobLog",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_JobLog_JobStatusTypeId",
                schema: "public",
                table: "JobLog",
                column: "JobStatusTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_JobLog_JobSummaryId",
                schema: "public",
                table: "JobLog",
                column: "JobSummaryId");

            migrationBuilder.CreateIndex(
                name: "IX_JobStaTyp_Id",
                schema: "public",
                table: "JobStatusType",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_JobStaTypNam_LocalizationId",
                schema: "public",
                table: "JobStatusTypeName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_JobStaTypNam_TypeId",
                schema: "public",
                table: "JobStatusTypeName",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSum_Id",
                schema: "public",
                table: "JobSummary",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_JobSum_JobExecutionTypeId",
                schema: "public",
                table: "JobSummary",
                column: "JobExecutionTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobExecutionTypeName",
                schema: "public");

            migrationBuilder.DropTable(
                name: "JobLog",
                schema: "public");

            migrationBuilder.DropTable(
                name: "JobStatusTypeName",
                schema: "public");

            migrationBuilder.DropTable(
                name: "JobSummary",
                schema: "public");

            migrationBuilder.DropTable(
                name: "JobStatusType",
                schema: "public");

            migrationBuilder.DropTable(
                name: "JobExecutionType",
                schema: "public");
        }
    }
}
