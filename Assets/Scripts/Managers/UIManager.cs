using System;
using UnityEngine;
using TMPro;

public class UIManager : MonoSingleton<UIManager>
{
    public GameObject signUpPanel,
        signInPanel,
        mainMenuTab,
        recordTab,
        graphTab,
        borderContainer;

    public TextMeshProUGUI today;

    public string userID = null; // change with login info
    private string _testEmail = "benjmercier11@gmail.com";
    private string _testPassword = "Password2020!";
    private string _testFamilyName = "Mercier";
    private string _testGivenName = "Ben";

    public override void Init()
    {
        base.Init();
        today.text = DateTime.Now.ToString("M / d / yyyy");
    }

    private void ControlActivePanel(GameObject setInactive, GameObject setActive)
    {
        setActive.SetActive(true);
        setInactive.SetActive(false);
    }

    public void NavigateToSignUp()
    {
        ControlActivePanel(signInPanel, signUpPanel);
    }

    public void NavigateToSignIn()
    {
        ControlActivePanel(signUpPanel, signInPanel);
    }

    public void ErrorNotification(TextMeshProUGUI errorTMP, string errorTxt)
    {
        errorTMP.rectTransform.gameObject.SetActive(true);
        Debug.Log(errorTMP.rectTransform.gameObject.name);
        errorTMP.text = errorTxt;
    }

    public async void Logout()
    {
        await CognitoManager.Instance.GlobalUserSignOutAsync();

        mainMenuTab.SetActive(false);
        recordTab.SetActive(false);
        graphTab.SetActive(false);
        borderContainer.SetActive(false);
        signUpPanel.SetActive(false);
    }

    /*
    public async void OnClickSignUp()
    {
        await CognitoManager.Instance.SignUpAsync(_testEmail, _testPassword, _testFamilyName, _testGivenName);
    }*/

    /*
    public async void OnClickSignIn()
    {
        //await CognitoManager.Instance.SignInSRPAsync(_testEmail, _testPassword);
    }*/
}
