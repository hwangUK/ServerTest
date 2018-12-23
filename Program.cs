using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

//using System
namespace MyServer
{  
    class HostSocket
    {

    }
    class Program
    {
        static byte[] buffer = new byte[1024];
        static MySqlManager mysql = new MySqlManager();
        static Socket listening_ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static List<Socket> clientSocketList = new List<Socket>();
        static ManualResetEvent allDone = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            Socket_Listen();
        }

        static void Socket_Listen()
        {
            Console.WriteLine("이것은 서버!");
            listening_ServerSocket.Bind(new IPEndPoint(IPAddress.Any, 9913));
            listening_ServerSocket.Listen(5);
            Console.WriteLine("Listining...");
            listening_ServerSocket.BeginAccept(new AsyncCallback(AcceptFromClient), null);

            Console.ReadLine();
        }
 
        static public void AcceptFromClient(IAsyncResult ar)
        {            
            allDone.Set();
            Console.WriteLine("Client Accept");
            Socket newSock = listening_ServerSocket.EndAccept(ar);
            clientSocketList.Add(newSock);
            listening_ServerSocket.BeginAccept(new AsyncCallback(AcceptFromClient), null);

            newSock.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None,
                         new AsyncCallback(Receive_Callback), newSock);
        }

        static void Receive_Callback(IAsyncResult ar)
        {
            Socket newSocket_receive = (Socket)ar.AsyncState;
            int inputLength = newSocket_receive.EndReceive(ar);
            //byte[] dataBuffer = new byte[inputLength];

            if (inputLength > 0)
            {
                Console.WriteLine("Get_FullString : " + Encoding.Default.GetString(buffer));
                string str = Encoding.Default.GetString(buffer);
                string[] token = str.Split(';');
                if (token[0] == "NewAccount")
                {
                    //SQL에 삽입
                    if (mysql.Mysql_Insert_NewID(token[1], token[2]))
                    {
                        SendToClient(ref newSocket_receive, "NewAccount;true;");
                    }
                    else
                    {
                        SendToClient(ref newSocket_receive, "NewAccount;false;");
                    }
                }
                else if (token[0] == "Login")
                {
                    string userData;
                    //SQL에서 아이디와 비밀번호 대조
                    if ((userData = mysql.Mysql_CheckLogin_Return_Userdata(token[1], token[2])) != "false")
                    {
                        SendToClient(ref newSocket_receive, "Login;true;" + token[1] + ";");
                        newSocket_receive.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None,
                            new AsyncCallback(Receive_Callback), newSocket_receive);
                        SendToClient(ref newSocket_receive, userData);
                    }
                    else
                    {
                        SendToClient(ref newSocket_receive, "Login;false;");
                    }
                }
                else if (token[0] == "Data")
                {
                    mysql.Mysql_Update_UserData(token[1], token[2]);
                    SendToClient(ref newSocket_receive, "Data;"+ token[2]+";");
                }

                newSocket_receive.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None,
                             new AsyncCallback(Receive_Callback), newSocket_receive);
            }
        }

        static public void SendToClient(ref Socket soc, string data)
        {
            byte[] transferStr = Encoding.Default.GetBytes(data);
            soc.BeginSend(transferStr, 0, transferStr.Length, SocketFlags.None, //0부터transferStr.Length까지
                         new AsyncCallback(Send_Callback_Server), soc);
        }

        // 비동기 클라전송 소켓 콜백함수
        static void Send_Callback_Server(IAsyncResult ar)
        {
            Socket newSock_send = (Socket)ar.AsyncState;
            newSock_send.EndSend(ar);
        }

        void AddToMySql(string ID, string PW)
        {
            //mysql.Mysql_Insert(ID, PW);
        }

    }
}
