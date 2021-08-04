using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class GetData : MonoBehaviour
{
    public List<DataPoint> dataList = new List<DataPoint>();

    private string _userID = "benjmercier11@gmail.com";
    private string _userID2 = "benjamin.j.mercier@gmail.com";
    private int _level = 150;

    private void Start()
    {
        //QueryData();
    }

    public void QueryData()
    {
        var context = new DynamoDBContext(DynamoDBManager.Instance.DynamoDBClient);

        var config = new DynamoDBOperationConfig();
        config.QueryFilter = new List<ScanCondition>();
        config.QueryFilter.Add(new ScanCondition("glucoseLevel", ScanOperator.LessThan, _level));

        var response = context.QueryAsync<DataPoint>(_userID, config).GetRemainingAsync();

        foreach (var item in response.Result)
        {
            dataList.Add(item);
            Debug.Log(item.glucoseLevel);
        }

        Debug.Log("Item Count: " + response.Result.Count);
    }
}
