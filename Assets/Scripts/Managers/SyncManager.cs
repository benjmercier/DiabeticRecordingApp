using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Amazon.CognitoSync;
using Amazon.CognitoSync.Model;
using System.Threading.Tasks;
using Amazon.AppSync;

public class SyncManager : MonoSingleton<SyncManager>
{
    private Dataset _dataInfo;

    private AmazonAppSyncClient _appSyncClient;
    public AmazonAppSyncClient AppSyncClient
    {
        get
        {
            if (_appSyncClient == null)
            {
                _appSyncClient = new AmazonAppSyncClient(AWSManager.Instance.cognitoAWSCredentials, AWSManager.Instance.regionEndpoint);
            }

            return _appSyncClient;
        }
    }

    public async Task AppSyncObj()
    {
        AmazonAppSyncRequest request = new AmazonAppSyncRequest()
        {
            
        };

        
    }


    private AmazonCognitoSyncClient _cognitoSyncClient;
    public AmazonCognitoSyncClient CognitoSyncClient
    {
        get
        {
            if (_cognitoSyncClient == null)
            {
                _cognitoSyncClient = new AmazonCognitoSyncClient(AWSManager.Instance.cognitoAWSCredentials, AWSManager.Instance.regionEndpoint);
            }

            return _cognitoSyncClient;
        }
    }

    public async Task CognitoSyncObj()
    {
        UpdateRecordsRequest request = new UpdateRecordsRequest()
        {
            IdentityId = "ID",
            RecordPatches = {},
            DatasetName = "Name",
            IdentityPoolId = AWSManager.Instance.identityPoolID,
            
        };

        UpdateRecordsResponse response = await CognitoSyncClient.UpdateRecordsAsync(request);

        AmazonCognitoSyncRequest syncRequest = new AmazonCognitoSyncRequest();
    }
}
