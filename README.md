# MessengerApp

This is a messenger application that I made in Unity.

## Acknowledgement
I owe a lot of credit to [Tom Weiland's YouTube series on C# networking](https://www.youtube.com/playlist?list=PLXkn83W0QkfnqsK8I0RAz5AbUxfg3bOQ5). I used his packet class as well as his base network solution for the networking in this application.

## How To Use 
1. Clone this repository
2. Open MessengerAppServer/MessengerAppServer/MessengerAppServer.sln
3. In the program.cs file, change the port number (I use 32137) to a TCP port that is available on [this page](https://en.wikipedia.org/wiki/List_of_TCP_and_UDP_port_numbers).
4. Run the solution (This starts the server).
5. Run the MessengerAppClient executable located in the MessengerAppClient/Build folder.
6. Type in a username, the port number you chose, and your local ip address. Then, click the connect button!
7. To connect to the server from another network, use the server network's public ip address. Also, you will have to set up port forwarding on the server network's router.
