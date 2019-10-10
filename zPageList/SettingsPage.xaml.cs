using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TeamProgramWithDatabase.zPageList
{
    /// <summary>
    /// SettingsPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingsPage : Page
    {
        private string username;
        public SettingsPage(string username, Dashboard board)
        {
            InitializeComponent();
            this.username = username;
            CheckBoxx.changeevent += () =>
            {
                DataManager.SetTeamNoticeRepeat(DataManager.getUserTeamCode(board.username), CheckBoxx.isChecked);
            };
            CheckBoxx.update(username);
        }
        
    }
}
