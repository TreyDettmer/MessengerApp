using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppManager : MonoBehaviour
{
    public static AppManager instance;

    public static Dictionary<int, ChatterManager> chatters = new Dictionary<int, ChatterManager>();

    public GameObject chatterPrefab;
    public GameObject localChatterPrefab;

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
}
