using System;
using System.Collections.Generic;
using System.Text;

namespace MessengerAppServer
{
    class Chatter
    {
        public int id;
        public string username;

        public string message;


        public Chatter(int _id, string _username)
        {
            id = _id;
            username = _username;
        }

        public void SetMessage(string _msg)
        {

            ServerSend.ChatterMessage(id, _msg);
        }
    }
}
