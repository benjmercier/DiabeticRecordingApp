using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class CognitoManager : MonoSingleton<CognitoManager>
{
    private readonly string _cognitoAppClientID = "76vi0oipd6qrpvmmd9o5a6e1g4";
    private readonly string _cognitoUserPoolID = "us-east-2_7tUfYovwK";
    private readonly string _cognitoUserPoolName = "cPsHUqept";

    private string _accessToken;
    private string _idToken;
    private string _refreshToken;
    
    private const string _email = "email";
    private const string _familyName = "family_name";
    private const string _givenName = "given_name";

    private bool _isLoggedIn = false;
    public bool IsLoggedIn { get { return _isLoggedIn; } }

    private AmazonCognitoIdentityProviderClient _cognitoClient = null;
    public AmazonCognitoIdentityProviderClient CognitoClient
    {
        get
        {
            if (_cognitoClient == null)
            {
                _cognitoClient = new AmazonCognitoIdentityProviderClient(new AnonymousAWSCredentials(), AWSManager.Instance.regionEndpoint);
            }

            return _cognitoClient;
        }
    }

    // Sign Up
    public async Task SignUpAsync(string email, string password, string familyName, string givenName, 
        TextMeshProUGUI errorNotification, GameObject emailNotification)
    {
        _cognitoClient = null;

        SignUpRequest request = new SignUpRequest()
        {
            ClientId = _cognitoAppClientID,
            Username = email,
            Password = password
        };

        List<AttributeType> attributeList = new List<AttributeType>()
        {
            new AttributeType(){Name = _email, Value = email},
            new AttributeType(){Name = _familyName, Value = familyName},
            new AttributeType(){Name = _givenName, Value = givenName}
        };

        foreach (var attribute in attributeList)
        {
            request.UserAttributes.Add(attribute);
        }

        try
        {
            SignUpResponse response = await CognitoClient.SignUpAsync(request);

            UIManager.Instance.NavigateToSignIn();
            emailNotification.SetActive(true);

            Debug.Log("Signed Up");
        }
        catch (Exception ex)
        {
            UIManager.Instance.ErrorNotification(errorNotification, ex.Message.ToString());

            Debug.Log("SignUp failed: " + ex.Message);
        }
    }

    // Sign In
    public async Task SignInSRPAsync(string email, string password, TextMeshProUGUI errorNotification)
    {
        _cognitoClient = null;
        _accessToken = null;
        _idToken = null;
        _refreshToken = null;

        CognitoUserPool userPool = new CognitoUserPool(_cognitoUserPoolID, _cognitoAppClientID, CognitoClient);
        CognitoUser user = new CognitoUser(email, _cognitoAppClientID, userPool, CognitoClient);

        InitiateSrpAuthRequest authRequest = new InitiateSrpAuthRequest()
        {
            Password = password
        };

        try
        {
            AuthFlowResponse authResponse = await user.StartWithSrpAuthAsync(authRequest);
            
            _accessToken = authResponse.AuthenticationResult.AccessToken;
            _idToken = authResponse.AuthenticationResult.IdToken;
            _refreshToken = authResponse.AuthenticationResult.RefreshToken;
            
            if (_idToken != null)
            {
                AWSManager.Instance.GetAWSCredentialsFromCognito(user, _cognitoUserPoolID, _idToken);
                Debug.Log("Logged in with tokens!");

                _isLoggedIn = true;

                UIManager.Instance.userID = email;

                DynamoDBManager.Instance.QueryItemsFromDB(email);
                UIManager.Instance.mainMenuTab.SetActive(true);
                UIManager.Instance.borderContainer.SetActive(true);
            }
            else
            {
                Debug.Log("ID Token is NULL.");
            }
        }
        catch (Exception ex)
        {
            _isLoggedIn = false;

            UIManager.Instance.ErrorNotification(errorNotification, ex.Message.ToString());

            Debug.Log("SignIn ex: " + ex.Message);
        }
    }

    // forgot password
    public async Task ForgotPasswordAsync(string email)
    {
        ForgotPasswordRequest request = new ForgotPasswordRequest()
        {
            Username = email,
            ClientId = _cognitoAppClientID
        };

        try
        {
            ForgotPasswordResponse response = await CognitoClient.ForgotPasswordAsync(request);
        }
        catch (Exception ex)
        {
            Debug.Log("Forgot Password ex: " + ex.Message);
        }
    }

    // confirm forgot password
    public async Task ConfirmNewPasswordAsync(string email, string newPassword, string confirmationCode)
    {
        ConfirmForgotPasswordRequest request = new ConfirmForgotPasswordRequest()
        {
            Username = email,
            ClientId = _cognitoAppClientID,
            Password = newPassword,
            ConfirmationCode = confirmationCode
        };

        try
        {
            ConfirmForgotPasswordResponse response = await CognitoClient.ConfirmForgotPasswordAsync(request);
        }
        catch (Exception ex)
        {
            Debug.Log("Confirm Password ex: " + ex.Message);
        }
    }

    // global sign out
    public async Task GlobalUserSignOutAsync()
    {
        GlobalSignOutRequest request = new GlobalSignOutRequest()
        {
            AccessToken = _accessToken
        };

        _accessToken = null;

        try
        {
            GlobalSignOutResponse response = await CognitoClient.GlobalSignOutAsync(request);
        }
        catch (Exception ex)
        {
            Debug.Log("Sign Out Exception: " + ex.Message);
        }
    }



    private void DecodeJSON(string encodedTxt)
    {
        byte[] decodedBytes = Convert.FromBase64String(encodedTxt);
        string decodedTxt = Encoding.UTF8.GetString(decodedBytes);

        Debug.Log("DecodedTxt: " + decodedTxt);
    }
}
