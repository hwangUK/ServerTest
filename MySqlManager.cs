using System;

using MySql.Data.MySqlClient;
using System.Data;

namespace MyServer
{
    class MySqlManager
    {
        static string str_dataBuffer = "";
        static string str_con =
            "Server=localhost;Database=test;Uid=root;Pwd=12159913";

        static MySqlConnection sql_con = new MySqlConnection(str_con);

        public bool Mysql_Insert_NewID(string name, string pw)
        {
            try
            {
                sql_con.Open();
                Console.WriteLine("MySql Connected!");

                string query_insert = "INSERT INTO userdb (name, password) " +
                            "VALUES ('" + name + "','" + pw + "')";

                MySqlCommand cmd = new MySqlCommand(query_insert, sql_con);
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

        public string Mysql_CheckLogin_Return_Userdata(string ID, string PW)
        {
            sql_con.Open();
            string query_select = "SELECT name,password,data FROM userdb WHERE name='" + ID + "'";
            MySqlCommand cmd = new MySqlCommand(query_select, sql_con);
            //MySqlDataReader를 통하여 sql데이터 가져온 후 저장하기
            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            if (reader.GetString(0) == ID && reader.GetString(1) == PW)
            {
                Console.WriteLine("비밀번호일치");
                string str = "Data;"+reader. GetString(2);
                reader.Close();
                sql_con.Close();
                return str;
            }
            Console.WriteLine("아이디,비밀번호 오류");
            reader.Close();
            sql_con.Close();
            return "false";
        }

        public void Mysql_Update_UserData(string ID, string data)
        {
            sql_con.Open();
            string query_update = "UPDATE userdb SET data ='" +data+"' WHERE name='"+ID+"'";
            MySqlCommand cmd = new MySqlCommand(query_update, sql_con);
            cmd.ExecuteNonQuery();
            sql_con.Close();
            Console.WriteLine("MySql 유저 데이터 업데이트 완료: "+ID+" : "+data);
        }
    }
}
