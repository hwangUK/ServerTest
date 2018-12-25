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
        static byte[] buffer = new byte[1024];
        static MySqlManager mysql = new MySqlManager();
        static Socket listening_ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static List<Socket> clientSocketList = new List<Socket>();
        //static ManualResetEvent allDone = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            Console.Title = "UkHwangServer";
            //서버 Listen 쓰레드
            Thread th_Listen = new Thread(new ThreadStart(Socket_Listen));
            th_Listen.Start();
            //Socket_Listen();
        }

        static void Socket_Listen()
        {
            Console.WriteLine("이것은 서버!");
            listening_ServerSocket.Bind(new IPEndPoint(IPAddress.Any, 9913));
            listening_ServerSocket.Listen(5);
            Console.WriteLine("접속을 기다리는중...");

            listening_ServerSocket.BeginAccept(new AsyncCallback(AcceptFromClient), null);
  
            Console.ReadLine();
        }

        static public void AcceptFromClient(IAsyncResult ar)
        {
            Thread th = new Thread(new ParameterizedThreadStart(th_acc));
            th.IsBackground = true;
            th.Start(ar);
        }

        static void th_acc(object Ar)
        {
            IAsyncResult ar = (IAsyncResult)Ar;
            Socket newSock = listening_ServerSocket.EndAccept(ar);
            Console.WriteLine("클라이언트가 접속하였습니다 ip주소는?? =>  "+ newSock.RemoteEndPoint.ToString());
           
            clientSocketList.Add(newSock);
            listening_ServerSocket.BeginAccept(new AsyncCallback(AcceptFromClient), null);

            newSock.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None,
                               new AsyncCallback(Receive_Callback), newSock);
        }

        static void Receive_Callback(IAsyncResult ar)
        {
            Thread th = new Thread(new ParameterizedThreadStart(th_re));
            th.IsBackground = true;
            th.Start(ar);
        }

        static void th_re(object Ar)
        {
            IAsyncResult ar = (IAsyncResult)Ar;
            Socket newSocket_receive = (Socket)ar.AsyncState;
            int inputLength = newSocket_receive.EndReceive(ar);
            //byte[] dataBuffer = new byte[inputLength];

            if (inputLength > 0)
            {
                Console.WriteLine("전체 문자열 스트림 : " + Encoding.Default.GetString(buffer));
                string str = Encoding.Default.GetString(buffer);
                string[] token = str.Split(';');
                if (token[0] == "NewAccount")
                {
                    try
                    {
                        //SQL에 삽입 성공하면 
                        //클라에게 전송 계정생성 결과
                        if (mysql.Mysql_Insert_NewID(token[1], token[2]))
                        {
                            SendToClient(ref newSocket_receive, "NewAccount;true;");
                        }
                        else
                        {
                            SendToClient(ref newSocket_receive, "NewAccount;false;");
                        }
                    }
                    catch
                    {
                        SendToClient(ref newSocket_receive, "NewAccount;false;");
                    }
                }
                else if (token[0] == "Login")
                {
                    string userData = "";
                    //SQL에서 아이디와 비밀번호 대조 결과값과 데이터 전송
                    //클라에게 전송 로그인성공여부 및 데이터
                    if ((userData = mysql.Mysql_CheckLogin_Return_Userdata(token[1], token[2])) != "false")
                    {
                        SendToClient(ref newSocket_receive, "Login;true;" + token[1] + ";"+ userData + ";");
                    }
                    else
                    {
                        SendToClient(ref newSocket_receive, "Login;false;");
                    }

                }
                else if (token[0] == "Data")
                {
                    //받은 데이터값으로 디비에 적용 후 데이터 전송
                    try
                    {
                        mysql.Mysql_Update_UserData(token[1], token[2]);
                        SendToClient(ref newSocket_receive, "Data;" + token[2] + ";");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("예외: " + e.ToString());
                    }
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
    }
}
