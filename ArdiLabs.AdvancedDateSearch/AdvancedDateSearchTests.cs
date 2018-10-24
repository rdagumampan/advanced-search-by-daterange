
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb;
using RepoDb.Attributes;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace ArdiLabs.AdvancedDateSearch
{
    [Map("[dbo].[TimeseriesData]")]
    public class TimeseriesData
    {
        public string Id { get; set; }
        public DateTime? StartTimeUtc { get; set; }
        public DateTime? EndTimeUtc { get; set; }
    }

    [TestClass]
    public class AdvancedDateSearchTests
    {
        private string connectionString = @"Server=.;Database=SEARCHDATAL;Integrated Security=True;";

        [TestInitialize]
        public void TestSetup()
        {
            var timeSeriesData = new List<TimeseriesData> {
                new TimeseriesData{Id="#1", StartTimeUtc = new DateTime(2018,01,10,0,0,0,DateTimeKind.Utc), EndTimeUtc = new DateTime(2018,01,24,0,0,0,DateTimeKind.Utc)},
                new TimeseriesData{Id="#2", StartTimeUtc = new DateTime(2018,01,10,0,0,0,DateTimeKind.Utc), EndTimeUtc = new DateTime(2018,01,20,0,0,0,DateTimeKind.Utc)},
                new TimeseriesData{Id="#3", StartTimeUtc = new DateTime(2018,01,10,0,0,0,DateTimeKind.Utc), EndTimeUtc = new DateTime(2018,01,14,0,0,0,DateTimeKind.Utc)},
                new TimeseriesData{Id="#4", StartTimeUtc = new DateTime(2018,01,10,0,0,0,DateTimeKind.Utc), EndTimeUtc = new DateTime(2018,01,12,0,0,0,DateTimeKind.Utc)},
                new TimeseriesData{Id="#5", StartTimeUtc = new DateTime(2018,01,10,0,0,0,DateTimeKind.Utc), EndTimeUtc = new DateTime(2018,01,11,0,0,0,DateTimeKind.Utc)},

                new TimeseriesData{Id="#6", StartTimeUtc = new DateTime(2018,01,12,0,0,0,DateTimeKind.Utc), EndTimeUtc = new DateTime(2018,01,24,0,0,0,DateTimeKind.Utc)},
                new TimeseriesData{Id="#7", StartTimeUtc = new DateTime(2018,01,12,0,0,0,DateTimeKind.Utc), EndTimeUtc = new DateTime(2018,01,20,0,0,0,DateTimeKind.Utc)},
                new TimeseriesData{Id="#8", StartTimeUtc = new DateTime(2018,01,12,0,0,0,DateTimeKind.Utc), EndTimeUtc = new DateTime(2018,01,17,0,0,0,DateTimeKind.Utc)},

                new TimeseriesData{Id="#9", StartTimeUtc = new DateTime(2018,01,16,0,0,0,DateTimeKind.Utc), EndTimeUtc = new DateTime(2018,01,24,0,0,0,DateTimeKind.Utc)},
                new TimeseriesData{Id="#10", StartTimeUtc = new DateTime(2018,01,16,0,0,0,DateTimeKind.Utc), EndTimeUtc = new DateTime(2018,01,20,0,0,0,DateTimeKind.Utc)},
                new TimeseriesData{Id="#11", StartTimeUtc = new DateTime(2018,01,16,0,0,0,DateTimeKind.Utc), EndTimeUtc = new DateTime(2018,01,18,0,0,0,DateTimeKind.Utc)},

                new TimeseriesData{Id="#12", StartTimeUtc = new DateTime(2018,01,20,0,0,0,DateTimeKind.Utc), EndTimeUtc = new DateTime(2018,01,24,0,0,0,DateTimeKind.Utc)},
                new TimeseriesData{Id="#13", StartTimeUtc = new DateTime(2018,01,21,0,0,0,DateTimeKind.Utc), EndTimeUtc = new DateTime(2018,01,24,0,0,0,DateTimeKind.Utc)},
            };

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Truncate<TimeseriesData>();
                connection.BulkInsert(timeSeriesData);
            }
        }

        [TestMethod]
        public void StartInclusiveTest()
        {
            var startTimeUtc = new DateTime(2018, 01, 12, 0, 0, 0, DateTimeKind.Utc);
            var endTimeUtc = new DateTime(2018, 01, 20, 0, 0, 0, DateTimeKind.Utc);

            var query = @"
                SELECT 
                 TD.[Id]
                ,TD.[StartTimeUtc]
                ,TD.[EndTimeUtc]
                FROM [SEARCHDATAL].[dbo].[TimeseriesData] TD
                WHERE
                 TD.StartTimeUtc >= @startTimeUtc
                 AND TD.StartTimeUtc < @endTimeUtc;
                ";

            var expected = new string[] { "#6", "#7", "#8", "#9", "#10", "#11", };
            using (var connection = new SqlConnection(connectionString))
            {
                var results = connection.ExecuteQuery<TimeseriesData>(query, new { startTimeUtc, endTimeUtc }).ToList();
                results.Count().ShouldBe(6);
                results.All(r => expected.Contains(r.Id)).ShouldBe(true);
                expected.All(r => results.Select(s=> s.Id).Contains(r)).ShouldBe(true);
            }
        }

        [TestMethod]
        public void EndInclusiveTest()
        {
            var startTimeUtc = new DateTime(2018, 01, 12, 0, 0, 0, DateTimeKind.Utc);
            var endTimeUtc = new DateTime(2018, 01, 20, 0, 0, 0, DateTimeKind.Utc);

            var query = @"
                SELECT 
                 TD.[Id]
                ,TD.[StartTimeUtc]
                ,TD.[EndTimeUtc]
                FROM [SEARCHDATAL].[dbo].[TimeseriesData] TD
                WHERE
                 TD.EndtimeUtc <= @endTimeUtc 
                 AND TD.EndTimeUtc > @startTimeUtc;
                ";

            var expected = new string[] { "#2", "#3", "#7", "#8", "#10", "#11", };
            using (var connection = new SqlConnection(connectionString))
            {
                var results = connection.ExecuteQuery<TimeseriesData>(query, new { startTimeUtc, endTimeUtc }).ToList();
                results.Count().ShouldBe(6);
                results.All(r => expected.Contains(r.Id)).ShouldBe(true);
                expected.All(r => results.Select(s => s.Id).Contains(r)).ShouldBe(true);
            }
        }


        [TestMethod]
        public void InclusiveTest()
        {
            var startTimeUtc = new DateTime(2018, 01, 12, 0, 0, 0, DateTimeKind.Utc);
            var endTimeUtc = new DateTime(2018, 01, 20, 0, 0, 0, DateTimeKind.Utc);

            var query = @"
                    SELECT 
                     TD.[Id]
                    ,TD.[StartTimeUtc]
                    ,TD.[EndTimeUtc]
                    FROM [SEARCHDATAL].[dbo].[TimeseriesData] TD
                    WHERE
                     TD.StartTimeUtc >= @startTimeUtc 
                     AND TD.EndTimeUtc <= @endTimeUtc;
                ";

            var expected = new string[] { "#7", "#8", "#10", "#11", };
            using (var connection = new SqlConnection(connectionString))
            {
                var results = connection.ExecuteQuery<TimeseriesData>(query, new { startTimeUtc, endTimeUtc }).ToList();
                results.Count().ShouldBe(4);
                results.All(r => expected.Contains(r.Id)).ShouldBe(true);
                expected.All(r => results.Select(s => s.Id).Contains(r)).ShouldBe(true);
            }
        }

        [TestMethod]
        public void OverlappedTest()
        {
            var startTimeUtc = new DateTime(2018, 01, 12, 0, 0, 0, DateTimeKind.Utc);
            var endTimeUtc = new DateTime(2018, 01, 20, 0, 0, 0, DateTimeKind.Utc);

            var query = @"
                SELECT 
                 TD.[Id]
                ,TD.[StartTimeUtc]
                ,TD.[EndTimeUtc]
                FROM [SEARCHDATAL].[dbo].[TimeseriesData] TD
                WHERE
                TD.EndtimeUtc > @startTimeUtc 
                AND TD.StartTimeUtc < @endTimeUtc;
                ";

            var expected = new string[] { "#1", "#2", "#3", "#6", "#7", "#8", "#9", "#10", "#11" };
            using (var connection = new SqlConnection(connectionString))
            {
                var results = connection.ExecuteQuery<TimeseriesData>(query, new { startTimeUtc, endTimeUtc }).ToList();
                results.Count().ShouldBe(9);
                results.All(r => expected.Contains(r.Id)).ShouldBe(true);
                expected.All(r => results.Select(s => s.Id).Contains(r)).ShouldBe(true);
            }
        }

        [TestMethod]
        public void InProgressTest()
        {
            var startTimeUtc = new DateTime(2018, 01, 12, 0, 0, 0, DateTimeKind.Utc);
            var endTimeUtc = new DateTime(2018, 01, 20, 0, 0, 0, DateTimeKind.Utc);

            var query = @"
                SELECT 
                 TD.[Id]
                ,TD.[StartTimeUtc]
                ,TD.[EndTimeUtc]
                FROM [SEARCHDATAL].[dbo].[TimeseriesData] TD
                WHERE
                    TD.StartTimeUtc >= @startTimeUtc
	                AND TD.StartTimeUtc < @endTimeUtc
                    AND TD.EndtimeUtc > @endTimeUtc;
                ";

            var expected = new string[] { "#6", "#9" };
            using (var connection = new SqlConnection(connectionString))
            {
                var results = connection.ExecuteQuery<TimeseriesData>(query, new { startTimeUtc, endTimeUtc }).ToList();
                results.Count().ShouldBe(2);
                results.All(r => expected.Contains(r.Id)).ShouldBe(true);
                expected.All(r => results.Select(s => s.Id).Contains(r)).ShouldBe(true);
            }
        }
    }
}
