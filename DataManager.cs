using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace TeamProgramWithDatabase
{
    public enum TeamCodeVerify
    {
        TeamNameContains, TeamCodeContains, None, Empty
    }
    partial class DataManager
    {
        private static MySqlConnection connection = new MySqlConnection("Server=localhost;Database=userdata;Uid=root;Pwd=password;");

        public static bool CheckUser(string id, string pasw)
        {
            try
            {
                DataSet ds = new DataSet();
                
                string sql = "SELECT Email,Password FROM userdatas";
                MySqlDataAdapter adpt = new MySqlDataAdapter(sql, connection);
                adpt.Fill(ds, "userdatas");
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        bool isID = r["Email"].Equals(id);
                        bool isPW = r["Password"].Equals(pasw);
                        if (isID && isPW)
                            return true;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            return false;
        }
        public static string getUsernameFromUserId(string id)
        {
            string sql = "select Username From userdatas where Email='" + id + "';";
            DataSet set = new DataSet();
            MySqlDataAdapter adpa = new MySqlDataAdapter(sql, connection);
            adpa.Fill(set, "userdatas");
            if (set.Tables.Count > 0) if (set.Tables[0].Rows.Count > 0)  return set.Tables[0].Rows[0]["Username"].ToString();
            return null;
        }
        public static bool isUserAccepted(string username)
        {
            string sql = "SELECT isAccepted FROM userdatas WHERE Email='" + username + "';";
            MySqlDataAdapter adp = new MySqlDataAdapter(sql, connection);
            DataSet set = new DataSet();
            adp.Fill(set, "userdatas");
            if (set.Tables.Count > 0) if(set.Tables[0].Rows.Count > 0 ) return Convert.ToBoolean(set.Tables[0].Rows[0]["isAccepted"].ToString());
            return false;
        }
        public static void RegisterUser(string name, string id, string password, string team,TeamCodeVerify verify)
        {
            if (verify == TeamCodeVerify.None) MakeNewTeam(team, name);
            int newId = GetUserList() + 1;
            string code = (verify == TeamCodeVerify.TeamCodeContains ? team : GetTeamCode(team));

            string sql = "INSERT INTO userdatas VALUES(" + newId + ",\""  + name + "\",\"" + id + "\",\"" + password + "\",\"" + code + "\",\"" + GetTeamName(code) + "\",\"false" + "\");";

            connection.Open();
            MySqlCommand command = new MySqlCommand(sql, connection);
            try
            {
                if (command.ExecuteNonQuery() != 1) MessageBox.Show("오류가 발생했습니다. 0- 회원가입\n관리자에게 문의해주세요.");
            }catch(Exception e) { Debug.WriteLine(e.ToString()); }
            connection.Close();

            if (verify == TeamCodeVerify.None) UserToCaptain(team, name);
        }

        private static string makeRandomChars(int length)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            for(int i = 0; i < length; i++)
            {
                builder.Append(Convert.ToChar((random.Next(25) + 97)));
            }
            return builder.ToString();
        }
        public static void UserToCaptain(string name, string captain)
        {
            string sql = "UPDATE userdata.userdatas SET isAccepted =\"true\" WHERE Username=\"" + captain + "\"";
            connection.Open();
            MySqlCommand command = new MySqlCommand(sql, connection);
            try
            {
                if (command.ExecuteNonQuery() != 1)
                {
                    MessageBox.Show("1오류가 발생하였습니다. 관리자에게 문의해주세요.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            connection.Close();
        }

        public static void MakeNewTeam(string name, string captain)
        {
            string NewId = makeRandomChars(16);
            while (GetTeamName(NewId) != null) NewId = makeRandomChars(16);
            string sql = "INSERT INTO teaminformation VALUES(\"" + name + "\", " + "\"" + NewId + "\", \"" + captain + "\", \"" + "1" + "\", \"" + DateTime.Now.ToString("yyyy/MM/dd(HH:mm:ss)") + "\");CREATE SCHEMA `" + NewId + "`;";
            connection.Open();
            MySqlCommand command = new MySqlCommand(sql, connection);
            try
            {
                if (command.ExecuteNonQuery() != 2)
                {
                    MessageBox.Show("2오류가 발생하였습니다. 관리자에게 문의해주세요.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            connection.Close();
            makeDefaultTeam(NewId, captain);
        }
        private static void makeDefaultTeam(string id, string username) //ID = TeamCode
        {
            using (MySqlConnection connection = new MySqlConnection("Server=localhost;Database=" + id + ";Uid=root;Pwd=password;"))
            {
                var map = Properties.Resources.UserprofileImg;
                ImageConverter converter = new ImageConverter();
                byte[] newbuffer = (byte[])converter.ConvertTo(map, typeof(byte[]));

                string sql = 
                    "CREATE TABLE `" + id + "`.`generalsettings` (`TeamImg` MEDIUMBLOB NULL,  `seeRepeatNotice` TINYINT NULL);" +
                    "CREATE TABLE `" + id + "`.`noticelist` (`When` INT NULL,  `NoticeMessage` VARCHAR(270) NULL,  `IsDeleted` TINYINT NULL);" +
                    "CREATE TABLE '" + id + "'.'notice_" + username + "' ('NoticeId' INT NULL, 'noticemessage' VARCHAR(270) NULL, 'IsSeen' BOOL NULL);" +
                    "INSERT INTO generalsettings VALUES(@image, 0);";
                connection.Open();
                MySqlCommand command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@image", newbuffer);
                try { if (command.ExecuteNonQuery() != 1) MessageBox.Show("Default Error 관리자에게 문의해주세요."); } catch (Exception e) { MessageBox.Show(e.Message); }
                connection.Close();
            }
        }
        
        
        public static ImageSource getTeamImage(string id)
        {
            using (MySqlConnection connection = new MySqlConnection("Server=localhost;Database=" + id + ";Uid=root;Pwd=password;"))
            {
                DataSet set = new DataSet();
                string sql = "SELECT * FROM generalsettings";
                MySqlDataAdapter adap = new MySqlDataAdapter(sql, connection);
                adap.Fill(set, "generalsettings");
                if (set.Tables.Count > 0)
                {
                    foreach (DataRow r in set.Tables[0].Rows)
                    {
                        byte[] buffer = r["TeamImg"] as byte[];
                        using (Stream stream = new MemoryStream(buffer))
                        {
                            BitmapImage image = new BitmapImage();
                            stream.Position = 0;
                            image.BeginInit();
                            image.CacheOption = BitmapCacheOption.OnLoad;
                            image.StreamSource = stream;
                            image.EndInit();
                            image.Freeze();
                            return image;
                        }
                    }
                }
                return null;
            }
        }

        public static void AddNewUserTeam(string username, string teamcode)
        {
            //Update membercount
            int teammembers = GetTeamMembers(teamcode);
            string sql = "UPDATE userdata.teaminformation SET TeamMember = \"" + (teammembers+1) + "\" WHERE TeamCode = \"" + teamcode + "\";UPDATE userdata.userdatas SET isAccepted=\"true\" WHERE Username=\"" + username + "\"";
            connection.Open();
            MySqlCommand command = new MySqlCommand(sql, connection);
            try
            {
                if (command.ExecuteNonQuery() != 1)
                {
                    MessageBox.Show("3오류가 발생하였습니다. 관리자에게 문의해주세요.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            connection.Close();
        }
        private static int GetTeamMembers(string code)
        {
            try
            {
                DataSet set = new DataSet();
                string sql = "SELECT TeamCode, TeamMember FROM teaminformation";
                MySqlDataAdapter adap = new MySqlDataAdapter(sql, connection);
                adap.Fill(set, "teaminformation");
                if(set.Tables.Count > 0)
                {
                    foreach(DataRow r in set.Tables[0].Rows)
                    {
                        if (r["TeamCode"].Equals(code)) return Convert.ToInt32(r["TeamMember"].ToString());
                    }
                }
            }catch(Exception e) { MessageBox.Show("ERROR \n" + e.Message); }

            return -1;
        }
        private static int GetUserList()
        {
            try
            {
                DataSet set = new DataSet();
                string sql = "SELECT * FROM userdatas";
                MySqlDataAdapter adap = new MySqlDataAdapter(sql, connection);
                adap.Fill(set, "userdatas");
                if (set.Tables.Count > 0)
                {
                    return set.Tables[0].Rows.Count;
                }
            }
            catch (Exception) { }
            return 0;
        }
        public static bool isCanUseUsername(string username)
        {
            try
            {
                DataSet set = new DataSet();
                string sql = "SELECT Username FROM userdatas";
                MySqlDataAdapter adap = new MySqlDataAdapter(sql, connection);
                adap.Fill(set, "userdatas");
                if(set.Tables.Count > 0)
                {
                    foreach (DataRow r in set.Tables[0].Rows)
                        if (r["Username"].ToString().ToLower().Equals(username)) return false;
                }
            }catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return true;
        }
        public static string GetTeamName(string str)
        {
            try
            {
                DataSet set = new DataSet();
                string sql = "SELECT * FROM teaminformation";
                MySqlDataAdapter adap = new MySqlDataAdapter(sql, connection);
                adap.Fill(set, "teaminformation");
                if(set.Tables.Count > 0)
                {
                    foreach (DataRow r in set.Tables[0].Rows)
                    {
                        if (r["TeamCode"].Equals(str)) return r["TeamName"].ToString();
                    }
                }
            }catch(Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            return null;
        }

        public static string GetTeamCode(string str)
        {
            try
            {
                DataSet set = new DataSet();
                string sql = "SELECT * FROM teaminformation";
                MySqlDataAdapter adap = new MySqlDataAdapter(sql, connection);
                adap.Fill(set, "teaminformation");
                if (set.Tables.Count > 0)
                {
                    foreach (DataRow r in set.Tables[0].Rows)
                    {
                        if (r["TeamName"].Equals(str)) return r["TeamCode"].ToString();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            return null;
        }



        public static TeamCodeVerify CheckStr(string str)
        {
            if (str.Equals("")) return TeamCodeVerify.Empty;
            try
            {
                DataSet set = new DataSet();
                string sql = "SELECT * FROM teaminformation";
                MySqlDataAdapter adap = new MySqlDataAdapter(sql, connection);
                adap.Fill(set, "teaminformation");
                if(set.Tables.Count > 0)
                {
                    foreach(DataRow r in set.Tables[0].Rows)
                    {
                        if (r["TeamName"].Equals(str)) return TeamCodeVerify.TeamNameContains;
                        if (r["TeamCode"].Equals(str)) return TeamCodeVerify.TeamCodeContains;
                    }
                }
            }catch(Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            return TeamCodeVerify.None;
        }
        public static bool isCanUseEmail(string email)
        {
            try
            {
                DataSet set = new DataSet();
                string sql = "SELECT Email FROM userdatas";
                MySqlDataAdapter adap = new MySqlDataAdapter(sql, connection);
                adap.Fill(set, "userdatas");
                if (set.Tables.Count > 0)
                {
                    foreach (DataRow r in set.Tables[0].Rows)
                        if (r["Email"].ToString().ToLower().Equals(email)) return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return true;
        }
        public static string getUserTeamCode(string username)
        {
            string sql = "SELECT * FROM userdata.userdatas";
            DataSet set = new DataSet();
            MySqlDataAdapter adap = new MySqlDataAdapter(sql, connection);
            adap.Fill(set, "userdatas");
            if (set.Tables.Count > 0) foreach (DataRow r in set.Tables[0].Rows) if (r["Username"].Equals(username)) return r["CheckCode"].ToString();
            return null;
        }
        public static Dictionary<string, string> GetUserInformation(string id)
        {
            Dictionary<string, string> userinfo = new Dictionary<string, string>();

            DataSet ds = new DataSet();
            string sql = "SELECT * FROM userdatas";
            MySqlDataAdapter apd = new MySqlDataAdapter(sql, connection);
            apd.Fill(ds, "userdatas");
            if(ds.Tables.Count > 0)
            {
                foreach(DataRow r in ds.Tables[0].Rows)
                {
                    if (r["Email"].Equals(id))
                    {
                        userinfo.Add("USERID", r["USERID"].ToString());
                        userinfo.Add("Username", r["Username"].ToString());
                        userinfo.Add("Email", r["Email"].ToString());
                        userinfo.Add("Password", r["Password"].ToString());
                        userinfo.Add("CheckCode", r["CheckCode"].ToString());
                        userinfo.Add("Team", r["TeamName"].ToString());
                        return userinfo;
                    }
                }
            }
            return null;
        }

        private static bool isContainsData(string email)
        {
            DataSet ds = new DataSet();
            string sql = "SELECT * FROM imsidata";
            MySqlDataAdapter apd = new MySqlDataAdapter(sql, connection);
            apd.Fill(ds, "imsidata");
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    if (r["Email"].Equals(email))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static void EndVerifyCode(string email)
        {
            string insertquery = "UPDATE imsidata SET Verifycode=\"ABCDEFGHIJ\" WHERE Email=\"" + email + "\"";
            connection.Open();
            MySqlCommand command = new MySqlCommand(insertquery, connection);
            try
            {
                if (command.ExecuteNonQuery() == 1)
                {
                    MessageBox.Show("인증코드가 만료되었습니다.");
                }
                else
                {
                    MessageBox.Show("4오류가 발생하였습니다. 관리자에게 문의해주세요.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            connection.Close();
        }
        public static bool CheckVerifyCode(string email, string code)
        {
            DataSet set = new DataSet();
            string sql = "SELECT * FROM imsidata";
            MySqlDataAdapter ada = new MySqlDataAdapter(sql, connection);
            ada.Fill(set, "imsidata");
            if(set.Tables.Count > 0)
            {
                foreach(DataRow r in set.Tables[0].Rows)
                {
                    if (r["Email"].Equals(email)) return r["Verifycode"].Equals(code);
                }
            }
            return false;
        }
        public static string VerifyCode(string email)
        {
            Random rnd = new Random();
            char[] data = {
                Convert.ToChar((rnd.Next() % 25) + 65),
                Convert.ToChar((rnd.Next() % 25) + 65),
                Convert.ToChar((rnd.Next() % 25) + 65),
                Convert.ToChar((rnd.Next() % 25) + 65),
                Convert.ToChar((rnd.Next() % 25) + 65),
                Convert.ToChar((rnd.Next() % 25) + 65),
            };
            string datas = "";
            foreach(char a in data) datas += a;

            string insertQuery = "INSERT INTO userdata.imsidata VALUES(\"" + email + "\",\"" + datas + "\")";
            if (isContainsData(email))
                insertQuery = "UPDATE userdata.imsidata SET Verifycode = \"" + datas + "\" WHERE Email = \"" + email + "\"";
            connection.Open();
            MySqlCommand command = new MySqlCommand(insertQuery, connection);

            try
            {
                if (command.ExecuteNonQuery() == 1)
                {
                    MessageBox.Show("이메일 발송을 요청하였습니다!\n이메일 발송은 최대 30초까지 소요될 수 있습니다.");
                }
                else
                {
                    MessageBox.Show("5오류가 발생하였습니다. 관리자에게 문의해주세요.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            connection.Close();
            return datas;
        }

        public static string getSerialNumber()
        {
            ManagementObjectSearcher MOS = new ManagementObjectSearcher("Select * From Win32_BaseBoard");
            foreach (ManagementObject obj in MOS.Get()) return obj["SerialNumber"].ToString();
            return null;
        }
        public static DataRow isAutoLogin(string serial)
        {
            DataSet set = new DataSet();
            string getQuery = "SELECT * FROM autologin";
            MySqlDataAdapter adpa = new MySqlDataAdapter(getQuery, connection);
            adpa.Fill(set, "autologin");
            if(set.Tables.Count > 0) foreach(DataRow r in set.Tables[0].Rows)
                    if (r["PCName"].Equals(serial))
                        return r;
            return null;
        }

        public static void removeAutologin(string pcserial)
        {
            if (null == isAutoLogin(pcserial)) return;
            string query = "DELETE FROM userdata.autologin WHERE PCName=\"" + pcserial + "\";";
            connection.Open();
            MySqlCommand command = new MySqlCommand(query, connection);
            try
            {
                if (command.ExecuteNonQuery() != 1) MessageBox.Show("7오류가 발생하였습니다. 관리자에게 문의해주세요.");
            }catch (Exception e) { MessageBox.Show(e.Message); }
            connection.Close();
        }

        public static void RegisterNew(string pcserial, string email, string password)
        {
            string query;
            if (isAutoLogin(pcserial) != null) query = "UPDATE userdata.autologin SET Email=\"" + email + "\", Password=\"" + password + "\" WHERE PCName=\"" + pcserial + "\"";
            else query = "INSERT INTO userdata.autologin VALUES(\"" + pcserial + "\", \"" + email + "\",\"" + password + "\")";

            connection.Open();
            MySqlCommand command = new MySqlCommand(query, connection);
            try
            {
                if (command.ExecuteNonQuery() != 1)
                {
                    MessageBox.Show("6오류가 발생하였습니다. 관리자에게 문의해주세요.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            connection.Close();
        }
    }
}
