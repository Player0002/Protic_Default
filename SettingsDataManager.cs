using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MySql;
using MySql.Data.MySqlClient;
namespace TeamProgramWithDatabase
{
    partial class DataManager
    {
		public static bool GetTeamNoticeRepeat(string teamcode)
        {
            using (MySqlConnection connection = new MySqlConnection("Server=localhost;Database=" + teamcode + ";Uid=root;Pwd=password;"))
            {
                string sql = "Select seeRepeatNotice From generalsettings";
                DataSet set = new DataSet();
                MySqlDataAdapter adap = new MySqlDataAdapter(sql, connection);
                adap.Fill(set, "generalsettings");
                if (set.Tables.Count > 0) if (set.Tables[0].Rows.Count > 0) return set.Tables[0].Rows[0]["seeRepeatNotice"].ToString() == "1";
                
                return false;
            }
        }
        public static void SetTeamNoticeRepeat(string teamcode, bool bol)
        {
            using(MySqlConnection connection = new MySqlConnection("Server=localhost;Database=" + teamcode + ";Uid=root;Pwd=password"))
            {
                string sql = "UPDATE generalsettings SET seeRepeatNotice=" + bol.ToString() + ";";
                connection.Open();
                MySqlCommand command = new MySqlCommand(sql, connection);
                try { if (command.ExecuteNonQuery() != 1) MessageBox.Show("2_1 오류가 발생하였습니다. 관리자에게 문의해주세요."); }catch(Exception e) { MessageBox.Show(e.Message); }
                connection.Close();
            }
        }
    }
}
