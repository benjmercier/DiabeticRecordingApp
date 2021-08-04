using Amazon;
using Amazon.CognitoIdentity;
using Amazon.Extensions.CognitoAuthentication;

public class AWSManager : MonoSingleton<AWSManager>
{
    public readonly string identityPoolID = "us-east-2:80c540c8-4910-49c9-9693-0b49b202590e";
    public RegionEndpoint regionEndpoint = RegionEndpoint.USEast2;

    public CognitoAWSCredentials cognitoAWSCredentials = null;

    public void GetAWSCredentialsFromCognito(CognitoUser user, string userPoolID, string idToken)
    {
        cognitoAWSCredentials = user.GetCognitoAWSCredentials(identityPoolID, regionEndpoint);
    }
}
