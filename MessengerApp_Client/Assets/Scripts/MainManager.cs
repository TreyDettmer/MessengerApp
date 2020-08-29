using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public static MainManager instance;
    public InputField messageInputField;
    public GameObject chatPanel, messageObject;
    public GameObject lobbyPanel, lobbyTextObject;
    public int maxMessages = 40;
    
    public List<GameObject> messageGameobjects = new List<GameObject>();

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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            SendMessageToChat();
        }
    }

    public void SendMessageToChat()
    {
        if (messageInputField.text.Length > 0 && !string.IsNullOrWhiteSpace(messageInputField.text))
        {
            if (Client.instance.SendMessageToChat(messageInputField.text))
            {
                messageInputField.text = "";
            }
        }
    }

    public void AddMessageToChatPanel(int _id, string _message, ChatterManager _chatter)
    {
        GameObject _messageObject = Instantiate(messageObject, chatPanel.transform);
        string _chatMessage;
        if (_id > 0)
        {

            _chatMessage = $"<b>{_chatter.username}</b>: {_message}";

            
            
        }
        else
        {
            _chatMessage = $"<b>SERVER</b>: {_message}";
            _messageObject.GetComponent<Text>().color = new Color(0.46f, 0f, 0f);
        }
        

        if (messageGameobjects.Count >= maxMessages)
        {

            Destroy(messageGameobjects[0].gameObject);
            messageGameobjects.RemoveAt(0);

        }
        _messageObject.GetComponent<Text>().text = _chatMessage;
        messageGameobjects.Add(_messageObject);

    }

    public void ClientLeaveChat()
    {
        Client.instance.LeaveChat();
    }

    public void UpdateLobbyPanel()
    {
        foreach  (Transform child in lobbyPanel.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (ChatterManager chatter in AppManager.chatters.Values)
        {
            GameObject _lobbyTextObject = Instantiate(lobbyTextObject, lobbyPanel.transform);
            _lobbyTextObject.GetComponent<Text>().text = chatter.username;
        }
    }
}
