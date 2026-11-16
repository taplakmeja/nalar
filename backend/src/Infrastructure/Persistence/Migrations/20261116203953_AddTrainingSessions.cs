using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTrainingSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExamProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Slug = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CategoryWeightsJson = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    QuestionCount = table.Column<int>(type: "integer", nullable: false),
                    DurationMinutes = table.Column<int>(type: "integer", nullable: false),
                    PassingScorePercent = table.Column<decimal>(type: "numeric", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrainingSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Mode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CategoryScope = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    DifficultyScope = table.Column<int>(type: "integer", nullable: false),
                    ExamProfileId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeadlineUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TotalQuestions = table.Column<int>(type: "integer", nullable: false),
                    CorrectCount = table.Column<int>(type: "integer", nullable: false),
                    IncorrectCount = table.Column<int>(type: "integer", nullable: false),
                    UnansweredCount = table.Column<int>(type: "integer", nullable: false),
                    Score = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    IsPassed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingSessions_ExamProfiles_ExamProfileId",
                        column: x => x.ExamProfileId,
                        principalTable: "ExamProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "SessionQuestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Category = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    GeneratorId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    GeneratorVersion = table.Column<int>(type: "integer", nullable: false),
                    Seed = table.Column<long>(type: "bigint", nullable: false),
                    Difficulty = table.Column<int>(type: "integer", nullable: false),
                    ParametersJson = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    CorrectAnswer = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    OptionsJson = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    SubmittedAnswer = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: true),
                    AnsweredAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SessionQuestions_TrainingSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "TrainingSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExamProfiles_Slug",
                table: "ExamProfiles",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SessionQuestions_SessionId",
                table: "SessionQuestions",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSessions_ExamProfileId",
                table: "TrainingSessions",
                column: "ExamProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSessions_UserId",
                table: "TrainingSessions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SessionQuestions");

            migrationBuilder.DropTable(
                name: "TrainingSessions");

            migrationBuilder.DropTable(
                name: "ExamProfiles");
        }
    }
}
