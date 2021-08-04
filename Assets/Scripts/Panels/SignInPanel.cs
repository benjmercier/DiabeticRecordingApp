using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignInPanel : MonoBehaviour, IPanel
{
    public InputField emailInput, passwordInput;

    [SerializeField]
    private TextMeshProUGUI _signInErrorNotificationTMP;
    private string _emailError = "Please input email address.";
    private string _passwordError = "Please input password.";
    private string _nullInputError = "Please input email address and password.";

    private void OnEnable()
    {
        _signInErrorNotificationTMP.gameObject.SetActive(false);
    }

    public void AttemptSignIn()
    {
        if (string.IsNullOrEmpty(emailInput.text) || string.IsNullOrEmpty(passwordInput.text))
        {
            if (string.IsNullOrEmpty(emailInput.text))
            {
                if (!string.IsNullOrEmpty(passwordInput.text))
                {
                    UIManager.Instance.ErrorNotification(_signInErrorNotificationTMP, _emailError);

                    return;
                }
            }
            
            if (string.IsNullOrEmpty(passwordInput.text))
            {
                if (!string.IsNullOrEmpty(emailInput.text))
                {
                    UIManager.Instance.ErrorNotification(_signInErrorNotificationTMP, _passwordError);

                    return;
                }
            }
            
            UIManager.Instance.ErrorNotification(_signInErrorNotificationTMP, _nullInputError);
        }
        else
        {
            ProcessInfo();
        }
    }

    public async void ProcessInfo()
    {
        await CognitoManager.Instance.SignInSRPAsync(emailInput.text, passwordInput.text, _signInErrorNotificationTMP);

        if (CognitoManager.Instance.IsLoggedIn)
        {
            _signInErrorNotificationTMP.gameObject.SetActive(false);
            emailInput.text = null;
            passwordInput.text = null;
        }
    }
}
