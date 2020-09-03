using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


/// <summary>
/// Managages the main menu UI
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager instance;

    public GameObject errorMessageObject;
    public InputField usernameField;
    public InputField ipAddressField;
    public InputField portField;
    public static string errorMessage = "";

    /// <summary>
    /// Initialize singleton
    /// </summary>
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
        // if there's an error message, display it.
        DisplayErrorMessage(errorMessage);
    }

    /// <summary>
    /// Ensure that all input fields are filled out, then tell the client to connect to the server.
    /// </summary>
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

    /// <summary>
    /// Displays an error message to the screen.
    /// </summary>
    /// <param name="_msg"></param>
    public void DisplayErrorMessage(string _msg)
    {
        if (errorMessageObject != null)
        {
            errorMessageObject.GetComponent<Text>().text = _msg;
            errorMessageObject.SetActive(true);
            StartCoroutine(ErrorMessageRoutine());
        }


    }

    /// <summary>
    /// Remove the error message after 3 seconds.
    /// </summary>
    /// <returns></returns>
    IEnumerator ErrorMessageRoutine()
    {
        yield return new WaitForSeconds(3f);
        if (errorMessageObject)
        {
            errorMessageObject.SetActive(false);
        }

    }

    
}
