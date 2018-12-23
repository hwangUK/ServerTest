using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;


//using System
namespace MyServer
{  
    class Program
    {
        //Thread thread_db = new Thread();
        static MySqlManager mysql = new MySqlManager();
        
        //static Dictionary<string, string> KeyOf_ID_PW = new Dictionary<string, string>();
        static Socket ListeningSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        static byte[] receiveBytes = new byte[1080];

        static void Main(string[] args)
        {
            //소켓 서버 설정         
            ListeningSock.Bind(new IPEndPoint(IPAddress.Any, 9913));
            ListeningSock.Listen(100);
            Socket_Listen();
        }

        static void Socket_Listen()
        {
            List<Socket> newClientSock = new List<Socket>();
            while (true)
            {
                Console.WriteLine("Listening...");
                newClientSock.Add(ListeningSock.Accept());
                Console.WriteLine("Client Accept");
                ReceiveFromClient(newClientSock.);

                //SendToServer(newClientSock);

            }
        }
        static public void SendToClient(Socket soc, string data)
        {
            byte[] transferStr = Encoding.Default.GetBytes(data);
            soc.BeginSend(transferStr, 0, transferStr.Length, SocketFlags.None, //0부터transferStr.Length까지
                         new AsyncCallback(Send_Callback_Server), soc);
        }
        static public void ReceiveFromClient(Socket soc)
        {
            soc.BeginReceive(receiveBytes, 0, 1080, SocketFlags.None,
                         new AsyncCallback(Receive_Callback_Server), soc);
        }

        // 비동기 클라전송 소켓 콜백함수
        static void Send_Callback_Server(IAsyncResult ar)
        {
            Socket transferSock = (Socket)ar.AsyncState;
            int strLength = transferSock.EndSend(ar);
        }

        static void Receive_Callback_Server(IAsyncResult ar)
        {
            Socket newSock = (Socket)ar.AsyncState;
            int strLength = newSock.EndReceive(ar);
            Console.WriteLine("Get_FullString : "+Encoding.Default.GetString(receiveBytes));
            string str = Encoding.Default.GetString(receiveBytes);
            string[] token = str.Split(';');
            if (token[0] == "Login")
            {
                //SQL에 삽입
                mysql.Mysql_Insert_NewID(token[1], token[2]);
                SendToClient(newSock, "Login;true;");
                Socket_Listen();
                //
                //Console.WriteLine(KeyOf_ID_PW[token[1]]);
                //if(비밀번호가 맞다면)

                //else
                //SendToServer(newSock, "Login:0");
                //sql
            }
        }
        void AddToMySql(string ID, string PW)
        {
            //mysql.Mysql_Insert(ID, PW);
        }

    }
}
