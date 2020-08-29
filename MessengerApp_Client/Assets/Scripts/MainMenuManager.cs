using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager instance;

    public GameObject errorMessageObject;
    public InputField usernameField;
    public InputField ipAddressField;
    public InputField portField;
    public static string errorMessage = "";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            

        }
        else if (instance != this)
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        DisplayErrorMessage(errorMessage);
    }

    public void ConnectToServer()
    {
        if (!string.IsNullOrWhiteSpace(ipAddressField.text) && !string.IsNullOrWhiteSpace(portField.text))
        {
            if (string.IsNullOrWhiteSpace(usernameField.text) || usernameField.text.IndexOf(' ') > -1)
            {
                DisplayErrorMessage("Error: Invalid username (includes whitespace).");
            }
            else if (usernameField.text.Length > 17)
            {
                DisplayErrorMessage("Error: Invalid username (too long).");
            }
            else
            {
                Client.instance.ConnectToServer(usernameField.text, ipAddressField.text, portField.text);

                SceneManager.LoadScene(1);
            }
        }
        else
        {
            DisplayErrorMessage("Error: Invalid server ip address or port number.");
        }

    }

    public void DisplayErrorMessage(string _msg)
    {
        if (errorMessageObject != null)
        {
            errorMessageObject.GetComponent<Text>().text = _msg;
            errorMessageObject.SetActive(true);
            StartCoroutine(ErrorMessageRoutine());
        }


    }

    IEnumerator ErrorMessageRoutine()
    {
        yield return new WaitForSeconds(3f);
        if (errorMessageObject)
        {
            errorMessageObject.SetActive(false);
        }

    }

    
}
