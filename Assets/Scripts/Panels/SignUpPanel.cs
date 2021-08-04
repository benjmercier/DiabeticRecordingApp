using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignUpPanel : MonoBehaviour, IPanel
{
    public InputField firstNameInput,
        lastNameInput,
        emailInput,
        passwordInput,
        verifyPasswordInput;

    [SerializeField]
    private InputField[] _requiredInput = new InputField[5];

    [SerializeField]
    private TextMeshProUGUI _signUpErrorNotificationTMP;
    private string _inputError = "Please fill out all fields.";
    private string _passwordError = "Passwords do not match.";

    [SerializeField]
    private GameObject _confirmEmailNotification;

    private void OnEnable()
    {
        _signUpErrorNotificationTMP.gameObject.SetActive(false);

        foreach (var input in _requiredInput)
        {
            input.text = null;
        }
    }

    public void AttemptSignUpUser()
    {
        if (_requiredInput.Any(i => string.IsNullOrEmpty(i.text)))
        {
            UIManager.Instance.ErrorNotification(_signUpErrorNotificationTMP, _inputError);

            return;
        }
        else
        {
            if (passwordInput.text != verifyPasswordInput.text)
            {
                UIManager.Instance.ErrorNotification(_signUpErrorNotificationTMP, _passwordError);

                return;
            }
            else
            {
                ProcessInfo();
            }
        }
    }

    public async void ProcessInfo()
    {
        await CognitoManager.Instance.SignUpAsync(emailInput.text,
            verifyPasswordInput.text,
            lastNameInput.text,
            firstNameInput.text,
            _signUpErrorNotificationTMP,
            _confirmEmailNotification);
    }
}
