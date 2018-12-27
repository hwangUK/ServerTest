using MySql.Data.MySqlClient;
using System;

namespace MyServer
{
    class MySqlManager
    {
        static string str_connectAdress ="Server=localhost;Database=test;Uid=root;Pwd=12159913";
        static MySqlConnection sql_Connection = new MySqlConnection(str_connectAdress);

        public bool Mysql_InsertNewID(string name, string pw)
        {
            try
            {
                sql_Connection.Open();
                Console.WriteLine("MySQL 접속!");
                string query_insert = "INSERT INTO userdb (name, password) VALUES ('" + name + "','" + pw + "')";
                MySqlCommand cmdSql = new MySqlCommand(query_insert, sql_Connection);
                cmdSql.ExecuteNonQuery();
                sql_Connection.Close();
                Console.WriteLine("MySql 새로운 유저 저장 완료!");
                return true;
            }
            catch (MySqlException e)
            {
                Console.WriteLine("이미 있는 아이디");
                Console.WriteLine(e.StackTrace);
                sql_Connection.Close();
                return false;
            }
        }

        public string Mysql_CheckLogin_ReturnUserdata(string ID, string PW)
        {
            sql_Connection.Open();
            string query_select = "SELECT name,password,data FROM userdb WHERE name='" + ID + "'";
            MySqlCommand cmdSql = new MySqlCommand(query_select, sql_Connection);

            //MySqlDataReader를 통하여 sql데이터 가져온 후 저장하기
            MySqlDataReader sqlDataReader = cmdSql.ExecuteReader();
            sqlDataReader.Read();
            try
            {
                if (sqlDataReader.GetString(0) == ID && sqlDataReader.GetString(1) == PW)
                {
                    Console.WriteLine("비밀번호일치");
                    string userDataLoadBuffer;
                    
                    userDataLoadBuffer = sqlDataReader.GetString(2);
                    sqlDataReader.Close();
                    sql_Connection.Close();
                    return userDataLoadBuffer;                   
                }
            }
            catch
            {
                Console.WriteLine("아이디,비밀번호 오류");
            }
            sqlDataReader.Close();
            sql_Connection.Close();
            return "false";
        }

        public void Mysql_Update_UserData(string ID, string data)
        {
            sql_Connection.Open();
            string query_update = "UPDATE userdb SET data ='" +data+"' WHERE name='"+ID+"'";
            MySqlCommand cmdSql = new MySqlCommand(query_update, sql_Connection);
            cmdSql.ExecuteNonQuery();
            sql_Connection.Close();
            Console.WriteLine("MySql 유저 데이터 업데이트 완료: "+ID+" : "+data);
        }
    }
}
