using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;
using Amazon.Runtime;
using Amazon.SecurityToken;

public class CreateDataPoint : MonoBehaviour
{
    public List<DataPoint> dataPoints = new List<DataPoint>();

    private string _currentTestTimestamp;
    private string _currentDailyPeriod;
    private int _currentGlucoseLevel;
    private int _currentBasalDose;
    private int _currentBolusDose;

    private string _userID = "benjmercier11@gmail.com";
    private string _testTimestamp = "2020-08-27 23:20";
    private string _dailyPeriod = "Night";
    private int _glucoseLevel = 67;

    private void Start()
    {
        //RecordDataPoint();
    }

    private void RecordDataPoint()
    {
        Table table = Table.LoadTable(DynamoDBManager.Instance.DynamoDBClient, DynamoDBManager.Instance.tableName);

        var data = new Document();

        data["userID"] = _userID;
        data["testTimestamp"] = _testTimestamp;
        data["dailyPeriod"] = _dailyPeriod;
        data["glucoseLevel"] = _glucoseLevel;

        table.PutItemAsync(data);
    }
}
