using Amazon.DynamoDBv2.DataModel;

[DynamoDBTable("TestingArchive")]
public class DataPoint
{
    [DynamoDBHashKey]
    public string userID { get; set; }

    [DynamoDBProperty]
    public string testTimestamp { get; set; }

    [DynamoDBProperty]
    public string dailyPeriod { get; set; }

    [DynamoDBProperty]
    public int glucoseLevel { get; set; }

    [DynamoDBProperty]
    public int bolusInsulinDose { get; set; } // fast-acting = bolus

    [DynamoDBProperty]
    public int basalInsulinDose { get; set; } // long-acting = basal

    [DynamoDBProperty]
    public string testNotes { get; set; }

    /*
    [DynamoDBProperty]
    public int testIDNumber { get; set; }
    */
}
