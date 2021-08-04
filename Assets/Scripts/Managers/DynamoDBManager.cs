using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using System.Collections.Generic;
using UnityEngine;

public class DynamoDBManager : MonoSingleton<DynamoDBManager>
{
    public readonly string tableName = "TestingArchive";

    public DataPoint currentDataPoint;

    public List<DataPoint> activeDataPoints = new List<DataPoint>();

    private const string _userID = "userID";
    private const string _testTimestamp = "testTimestamp";
    private const string _dailyPeriod = "dailyPeriod";
    private const string _glucoseLevel = "glucoseLevel";
    private const string _bolusDose = "bolusDose";
    private const string _basalDose = "basalDose";
    private const string _testNotes = "testNotes";
    private const string _testID = "testID";

    private AmazonDynamoDBClient _dynamoDBClient;
    public AmazonDynamoDBClient DynamoDBClient
    {
        get
        {
            if (_dynamoDBClient == null)
            {
                _dynamoDBClient = new AmazonDynamoDBClient(AWSManager.Instance.cognitoAWSCredentials, AWSManager.Instance.regionEndpoint); // new AmazonDynamoDBClient(AWSManager.Instance.awsAccessKey, AWSManager.Instance.awsSecretKey, AWSManager.Instance.regionEndpoint);
            }

            return _dynamoDBClient;
        }
    }

    private DynamoDBContext _context;
    public DynamoDBContext Context
    {
        get
        {
            if (_context == null)
            {
                _context = new DynamoDBContext(_dynamoDBClient);
            }

            return _context;
        }
    }

    public void CreateNewDataPoint()
    {
        currentDataPoint = new DataPoint();
    }

    public void QueryItemsFromDB(string userID)
    {
        AmazonDynamoDBClient client = DynamoDBClient;

        var request = new QueryRequest
        {
            TableName = tableName,
            KeyConditionExpression = "userID = :v_userID",
            ExpressionAttributeValues = { { ":v_userID", new AttributeValue { S = userID } } },
            ConsistentRead = true
        };

        var result = client.QueryAsync(request).Result;

        activeDataPoints.Clear();

        var items = result.Items;

        foreach (var data in items)
        {
            DataPoint dataPoint = new DataPoint();

            foreach (var valuePair in data)
            {
                string name = valuePair.Key;
                AttributeValue value = valuePair.Value;

                switch (name)
                {
                    case _userID:
                        dataPoint.userID = value.S;
                        break;

                    case _testTimestamp:
                        dataPoint.testTimestamp = value.S;
                        break;

                    case _dailyPeriod:
                        dataPoint.dailyPeriod = value.S;
                        break;

                    case _glucoseLevel:
                        dataPoint.glucoseLevel = int.Parse(value.N);
                        break;

                    case _basalDose:
                        dataPoint.basalInsulinDose = int.Parse(value.N);
                        break;

                    case _bolusDose:
                        dataPoint.bolusInsulinDose = int.Parse(value.N);
                        break;

                    case _testNotes:
                        dataPoint.testNotes = value.S;
                        break;

                    case _testID:
                        // add info to test ID
                        break;

                    default:
                        Debug.Log("Name (" + name + ") and " + "value (" + value + ") is unassigned.");
                        break;
                }
            }

            activeDataPoints.Add(dataPoint);
        }
    }

    public void UploadToDynamoDB()
    {
        Table table = Table.LoadTable(DynamoDBClient, tableName);

        var data = new Document();

        data[_userID] = UIManager.Instance.userID;
        data[_testTimestamp] = currentDataPoint.testTimestamp;
        data[_dailyPeriod] = currentDataPoint.dailyPeriod;
        data[_glucoseLevel] = currentDataPoint.glucoseLevel;
        data[_bolusDose] = currentDataPoint.bolusInsulinDose;
        data[_basalDose] = currentDataPoint.basalInsulinDose;
        data[_testNotes] = currentDataPoint.testNotes;

        table.PutItemAsync(data);

        Debug.Log("Data sent to DynamoDB.");
    }

    


    // Alt methods*****************************************************************************************
    public void AddToListFromDB(string userID)
    {
        // original method
        /*
        var context = new DynamoDBContext(AWSManager.Instance.Client);

        var config = new DynamoDBOperationConfig();
        config.ConsistentRead = true;


        var response = context.QueryAsync<DataPoint>(userID, config).GetRemainingAsync();

        activeDataPoints.Clear();

        foreach (var data in response.Result)
        {
            activeDataPoints.Add(data);
        }*/


        AmazonDynamoDBClient client = DynamoDBClient;

        var request = new QueryRequest
        {
            TableName = tableName,
            KeyConditionExpression = "userID = :v_userID",
            ExpressionAttributeValues = { { ":v_userID", new AttributeValue { S = userID } } },
            Select = Select.ALL_ATTRIBUTES,
            //ProjectionExpression = "testTimestamp, dailyPeriod, glucoseLevel, basalDose, bolusDose, testNotes",
            ConsistentRead = true
        };

        var response = client.QueryAsync(request).Result;

        activeDataPoints.Clear();

        foreach (Dictionary<string, AttributeValue> data in response.Items)
        {
            var doc = Document.FromAttributeMap(data);
            var typeDoc = Context.FromDocument<DataPoint>(doc);

            activeDataPoints.Add(typeDoc);
            Debug.Log("basal: " + typeDoc.basalInsulinDose);
        }
    }

    private void DescribeTable()
    {
        AmazonDynamoDBClient client = DynamoDBClient;

        DescribeTableRequest request = new DescribeTableRequest
        {
            TableName = tableName
        };

        TableDescription tableDescription = client.DescribeTableAsync(request).Result.Table;

        Debug.Log("Table Name: " + tableDescription.TableName);
        Debug.Log("Item Count: " + tableDescription.ItemCount);
        Debug.Log("Table Byte Size: " + tableDescription.TableSizeBytes);

        List<KeySchemaElement> tableSchema = tableDescription.KeySchema;
        for (int i = 0; i < tableSchema.Count; i++)
        {
            KeySchemaElement element = tableSchema[i];
            Debug.Log("Key Name: " + element.AttributeName + "\n" + "Key Type: " + element.KeyType);
        }

        List<AttributeDefinition> attributeDefinitions = tableDescription.AttributeDefinitions;
        for (int i = 0; i < attributeDefinitions.Count; i++)
        {
            AttributeDefinition definition = attributeDefinitions[i];

            Debug.Log("Attribute Name: " + definition.AttributeName + "\n" + "Attribute Type: " + definition.AttributeType);
        }
    }

    private void GetItem(string userID)
    {
        AmazonDynamoDBClient client = DynamoDBClient;

        Dictionary<string, AttributeValue> key = new Dictionary<string, AttributeValue>
        {
            {"userID", new AttributeValue{S = userID} },
            {"testTimestamp", new AttributeValue{S = "2020-10-12 08:33"} }
        };

        GetItemRequest request = new GetItemRequest
        {
            TableName = tableName,
            Key = key
        };

        var result = client.GetItemAsync(request);

        Dictionary<string, AttributeValue> item = result.Result.Item;
        foreach (var keyValuePair in item)
        {
            Debug.Log("Key: " + keyValuePair.Key + "\nString Value: " + keyValuePair.Value.S +
                "  Number Value: " + keyValuePair.Value.N);

            if (keyValuePair.Value.N != null)
            {
                Debug.Log("Type: " + keyValuePair.Value.N.GetType());
            }

        }
    }

    private int GenerateTestIDNumber(string user)
    {
        var request = new QueryRequest
        {
            TableName = tableName,
            KeyConditionExpression = "userID = :v_userID",
            ExpressionAttributeValues = { { ":v_userID", new AttributeValue { S = user } } },
            Limit = 1,
            ScanIndexForward = false
        };

        request.ConsistentRead = true;

        var response = DynamoDBClient.QueryAsync(request);

        string test = null;

        foreach (var item in response.Result.Items)
        {
            test = item["testIDNumber"].N;
        }

        int testIDNumber = int.Parse(test) + 1;
        Debug.Log("TestIDNumber: " + testIDNumber);
        return testIDNumber;
    }

    private int CountUserTests(string userID)
    {
        //dataList.Clear();

        var context = new DynamoDBContext(DynamoDBClient);

        var config = new DynamoDBOperationConfig();
        config.ConsistentRead = true;

        var response = context.QueryAsync<DataPoint>(userID, config).GetRemainingAsync();

        Debug.Log("Item Count: " + response.Result.Count);

        return response.Result.Count;
    }

    private void DescribeTableRequest()
    {
        var request = new DescribeTableRequest
        {
            TableName = tableName,
        };

        var result = DynamoDBClient.DescribeTableAsync(request);

        var response = result.Result;

        TableDescription description = response.Table;

        Debug.Log("Table Name: " + description.TableName);
        Debug.Log("Item Count: " + description.ItemCount);
    }
}
