using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace TeamProgramWithDatabase
{
    partial class DataManager
    {
        public static void testFileUpdate(string id)
        {
            using (MySqlConnection connection = new MySqlConnection("Server=localhost;Database=" + id + ";Uid=root;Pwd=password;"))
            {
                var map = Properties.Resources._210_M고딕030_OTF;
                byte[] newbuffer = map;
                string sql = "INSERT INTO testtable VALUES(\"testimg\", \"otf\", @image);";
                connection.Open();
                MySqlCommand command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@image", newbuffer);
                try { if (command.ExecuteNonQuery() != 1) MessageBox.Show("Default Error 관리자에게 문의해주세요."); } catch (Exception e) { MessageBox.Show(e.Message); }
                connection.Close();
            }
        }
        public static void testfiledownload(string id)
        {
            using (MySqlConnection connection = new MySqlConnection("Server=localhost;Database=" + id + ";Uid=root;Pwd=password;"))
            {
                DataSet set = new DataSet();
                string sql = "SELECT * FROM testtable";
                MySqlDataAdapter adap = new MySqlDataAdapter(sql, connection);
                adap.Fill(set, "testtable");
                if (set.Tables.Count > 0)
                {
                    foreach (DataRow r in set.Tables[0].Rows)
                    {
                        byte[] buffer = r["FileData"] as byte[];

                        using (FileStream file = File.OpenWrite(@"C:\Users\KYJ\Desktop\" + r["FileName"].ToString() + "." + r["FileType"].ToString()))
                        {
                            file.Write(buffer, 0, buffer.Length);
                        }
                    }
                }
            }
        }
        public static List<string> getTeamNoticeMessage(string code)
        {
            using (MySqlConnection connection = new MySqlConnection("Server=localhost;Database=" + code + ";Uid=root;Pwd=password;"))
            {
                string MySql = "select * FROM "+ code + ".noticelist order by 'When'";
                MySqlDataAdapter adap = new MySqlDataAdapter(MySql, connection);
                DataSet set = new DataSet();
                adap.Fill(set, "noticelist");
                if (set.Tables.Count > 0)
                {
                    DataRow[] rows = new DataRow[set.Tables[0].Rows.Count];
                    List<string> result = new List<string>();
                    for (int i = 0; i < rows.Length; i++) rows[i] = set.Tables[0].Rows[i];
                    var a = from str in rows
                            where Convert.ToInt32(str["IsDeleted"]) != 1
                            orderby str["When"] descending
                            select str;
                    foreach (var c in a) result.Add(c["NoticeMessage"].ToString());
                    return result;
                }

            }
            return null;
        }
        public static string getTeamCaptiainName(string code)
        {
            using(MySqlConnection connection = new MySqlConnection("Server=localhost;Database=" + code + ";Uid=root;Pwd=password"))
            {
                string sql = "select * from userdata.teaminformation where TeamCode='" + code + "';";
                MySqlDataAdapter adap = new MySqlDataAdapter(sql, connection);
                DataSet set = new DataSet();
                adap.Fill(set, "teaminformation");
                if (set.Tables.Count > 0) return set.Tables[0].Rows[0]["TeamCaptain"].ToString();
                else return null;
            }
        }

        public static int getNoticeAmount(string username)
        {
            using (MySqlConnection connection = new MySqlConnection("Server=localhost;Database=" + DataManager.getUserTeamCode(username) + ";Uid=root;Pwd=password"))
            {
                int res = 0;
                string sql = "select * from notice_"  + username + ";";
                MySqlDataAdapter adpa = new MySqlDataAdapter(sql, connection);
                DataSet set = new DataSet();
                adpa.Fill(set, "notice_" + username);
                if (set.Tables.Count > 0) foreach (DataRow r in set.Tables[0].Rows) if (r["IsSeen"].ToString() == "False") res++;
                return res;
            }
        }
        public static Dictionary<string, string> getNoticeList(string username)
        {
            using (MySqlConnection connection = new MySqlConnection("Server=localhost;Database=" + DataManager.getUserTeamCode(username) + ";Uid=root;Pwd=password"))
            {
                string sql = "select * from notice_" + username + ";";
                MySqlDataAdapter adpa = new MySqlDataAdapter(sql, connection);
                DataSet set = new DataSet();
                Dictionary<string, string> dic = new Dictionary<string, string>();
                adpa.Fill(set, "notice_" + username);
                if (set.Tables.Count > 0) if (set.Tables[0].Rows.Count > 0) foreach (DataRow row in set.Tables[0].Rows) dic.Add(row["NoticeId"].ToString(), row["noticemessage"].ToString());
                return dic;
            }
        }
        public static void readnotice(string username, string noticeid)
        {
            string code = DataManager.getUserTeamCode(username);
            using (MySqlConnection connection = new MySqlConnection("Server=localhost;Database=" + code + ";Uid=root;Pwd=password"))
            {
                string sql = "update " + "notice_" + username + " set IsSeen=true where NoticeId=" + noticeid + ";";
                connection.Open();
                MySqlCommand command = new MySqlCommand(sql, connection);
                try { if (command.ExecuteNonQuery() != 1) MessageBox.Show("예외가 발생하였습니다. 관리자에게 문의하여주세요\nErrorCode - 2.4"); } catch (Exception e) { MessageBox.Show(e.Message); }
                connection.Close();
            }
        }
    }
}