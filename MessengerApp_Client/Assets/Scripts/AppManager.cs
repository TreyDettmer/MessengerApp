using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages app on client
/// </summary>
public class AppManager : MonoBehaviour
{
    public static AppManager instance;

    public static Dictionary<int, ChatterManager> chatters = new Dictionary<int, ChatterManager>();

    public GameObject chatterPrefab;
    public GameObject localChatterPrefab;


    /// <summary>
    /// Initialize singleton
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);

        }
        else if (instance != this)
        {
            Destroy(this);
        }
        
    }

    /// <summary>
    /// Creates a chatter instance withe the given id and username
    /// </summary>
    /// <param name="_id"></param>
    /// <param name="_username"></param>
    public void AddChatter(int _id,string _username)
    {
        GameObject _chatter;
        if (_id == Client.instance.myId)
        {
            _chatter = Instantiate(localChatterPrefab, Vector3.zero, Quaternion.identity);
        }
        else
        {
            _chatter = Instantiate(chatterPrefab, Vector3.zero, Quaternion.identity);
        }
       
        _chatter.GetComponent<ChatterManager>().id = _id;
        _chatter.GetComponent<ChatterManager>().username = _username;
        chatters.Add(_id, _chatter.GetComponent<ChatterManager>());
    }

    public void Quit()
    {
        Application.Quit();
    }
}
