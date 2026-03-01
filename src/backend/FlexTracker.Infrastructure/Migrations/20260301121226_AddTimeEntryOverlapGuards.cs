using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlexTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTimeEntryOverlapGuards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) Speed up overlap lookups by narrowing candidates by date/time.
            migrationBuilder.CreateIndex(
                name: "IX_TimeEntries_Date_StartTime_EndTime",
                table: "TimeEntries",
                columns: new[] { "Date", "StartTime", "EndTime" });

            // 2) Block overlapping inserts.
            migrationBuilder.Sql(
                """
                CREATE TRIGGER IF NOT EXISTS TR_TimeEntries_PreventOverlap_Insert
                BEFORE INSERT ON TimeEntries
                FOR EACH ROW
                WHEN EXISTS (
                    SELECT 1
                    FROM TimeEntries t
                    WHERE t.Date = NEW.Date
                      AND NEW.StartTime < t.EndTime
                      AND NEW.EndTime > t.StartTime
                )
                BEGIN
                    SELECT RAISE(ABORT, 'FT_OVERLAP_TIME_ENTRY');
                END;
                """);

            // 3) Block overlapping updates (excluding self row).
            migrationBuilder.Sql(
                """
                CREATE TRIGGER IF NOT EXISTS TR_TimeEntries_PreventOverlap_Update
                BEFORE UPDATE ON TimeEntries
                FOR EACH ROW
                WHEN EXISTS (
                    SELECT 1
                    FROM TimeEntries t
                    WHERE t.Date = NEW.Date
                      AND t.Id <> NEW.Id
                      AND NEW.StartTime < t.EndTime
                      AND NEW.EndTime > t.StartTime
                )
                BEGIN
                    SELECT RAISE(ABORT, 'FT_OVERLAP_TIME_ENTRY');
                END;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS TR_TimeEntries_PreventOverlap_Insert;");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS TR_TimeEntries_PreventOverlap_Update;");

            migrationBuilder.DropIndex(
                name: "IX_TimeEntries_Date_StartTime_EndTime",
                table: "TimeEntries");
        }
    }
}
