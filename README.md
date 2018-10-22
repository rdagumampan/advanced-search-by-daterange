# code4fun-advanced-daterange-search

An baseline implementation for searching timeseries data based on multiple data ranges. Supported modes are StartInclusive, EndInclusive, Inclusive, Overlapped and InProgress.
The underying query language will be SQL92 but would work with any language.

Source from OSISoft SDK Reference:<br>
https://techsupport.osisoft.com/Documentation/PI-AF-SDK/html/T_OSIsoft_AF_Asset_AFSearchMode.htm

## Setup Database

```sql
CREATE TABLE [dbo].[TimeseriesData](
	[Id] [nvarchar](50) NOT NULL,
	[StartTimeUtc] [datetime2](7) NOT NULL,
	[EndTimeUtc] [datetime2](7) NOT NULL
) ON [PRIMARY]
GO
```

## Setup Sample
```sql
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
```

## Test

```sql
[TestMethod]
public void SearchStartingBetweenTest()
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
```
