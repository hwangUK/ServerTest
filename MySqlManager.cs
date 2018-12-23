using System;

using MySql.Data.MySqlClient;
using System.Data;

namespace MyServer
{
    class MySqlManager
    {
        static string str_con =
            "Server=localhost;Database=test;Uid=root;Pwd=12159913";

        static MySqlConnection sql_con = new MySqlConnection(str_con);

        public bool Mysql_Insert_NewID(string name, string pw)
        {
            try
            {
                sql_con.Open();
                Console.WriteLine("MySql Connected!");

                string str_cmd = "INSERT INTO login (name, password) " +
                            "VALUES ('" + name + "','" + pw + "')";

                MySqlCommand cmd = new MySqlCommand(str_cmd, sql_con);
                cmd.ExecuteNonQuery();
                sql_con.Close();
                Console.WriteLine("MySql Disconnected!");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Already Exist ID");

                Console.WriteLine(e.StackTrace);
                sql_con.Close();
                return false;
            }
        }

        public bool Mysql_Find_ID()
        {
            try
            {
                DataSet ds = new DataSet();

                //MySqlDataAdapter 클래스를 이용하여 비연결 모드로 데이타 가져오기
                string sql = "SELECT * FROM login";
                MySqlDataAdapter adpt = new MySqlDataAdapter(sql, sql_con);
                adpt.Fill(ds, "login");
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        Console.WriteLine(r["name"]);
                        // return r["name"];
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return false;
            }

        }

        public void CheckLogin()
        {
            if (Mysql_Find_ID())
            {
                //로그인성공
                // 해당 json 데이터 클라에게 전송
            }
            else //실패
                ;
        }
    }
}
