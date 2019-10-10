using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
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
using TeamProgramWithDatabase.zPageList;

namespace TeamProgramWithDatabase.Popup
{
    /// <summary>
    /// PopupMenu_Userprofile.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PopupMenu_Userprofile : Page
    {

        private SoundPlayer click_sound_player = new SoundPlayer(Properties.Resources.Mouse_Click_Sound);
        public PopupMenu_Userprofile(Dashboard dashboard)
        {
            InitializeComponent();
            registerButton(Logout);
            registerButton(GeneralSettings);
            Logout.MouseDown += (sender, eventargs) =>
            {
                click_sound_player.Play();
                DataManager.removeAutologin(DataManager.getSerialNumber());
                MainWindow maindow = new MainWindow();
                maindow.Show();
                foreach (Window window in Application.Current.Windows)
                {
                    if (window != maindow) window.Close();
                }
            };
            GeneralSettings.MouseDown += (sender, eventargs) =>
            {
                if (DataManager.getTeamCaptiainName(DataManager.getUserTeamCode(dashboard.username)) == dashboard.username)
                {
                    SettingsPage page = new SettingsPage(dashboard.username, dashboard);
                    dashboard.TestFrame.Source = new Uri("zPageList/SettingsPage.xaml", UriKind.Relative);
                    dashboard.TestFrame.Navigate(page);
                    dashboard.Popup.Visibility = Visibility.Hidden;
                }
                else
                {
                    MessageBox.Show("Error 팀장만이 이 페이지에 접근이 가능합니다.");
                }
            };
        }
        private void registerButton(Canvas canvas)
        {
            canvas.MouseEnter += (sender, args) =>
            {
                canvas.Background = new SolidColorBrush(Color.FromArgb(255, 247, 247, 247));
            };
            canvas.MouseLeave += (sender, eventargs) =>
            {
                canvas.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            };
            
        }
    }
}
