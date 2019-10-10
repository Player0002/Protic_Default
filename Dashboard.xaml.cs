using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Windows.Threading;
using TeamProgramWithDatabase.GeneralPageContent;
using TeamProgramWithDatabase.Popup;
using TeamProgramWithDatabase.zPageList;
namespace TeamProgramWithDatabase
{
    /// <summary>
    /// Dashboard.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Dashboard : Page
    {
        public string username;
        public double lengthofusername;
        private Canvas checkedobj;
        private Canvas horvredcanvas;
        DispatcherTimer timer = new DispatcherTimer();
        private List<Canvas> canvaslist = new List<Canvas>();

        private SoundPlayer click_sound_player = new SoundPlayer(Properties.Resources.Mouse_Click_Sound);

        public Dashboard(string username)
        {
            InitializeComponent();
            DataContext = this;

            timer.Interval = TimeSpan.FromSeconds(0.01);
            timer.Tick += timer_tickevent;
            timer.Start();
            this.username = username;

            if (isOnlyEnglish(username)) UsernameInfo.Content = username.Length > 7 ? username.Substring(0, 7) + "..." : username;
            else UsernameInfo.Content = username.Length > 12 ? username.Substring(0, 12) + "..." : username;


            var label = new Label() { Content = UsernameInfo.Content, FontFamily = new System.Windows. Media.FontFamily("D2Coding"), FontSize = 18 };
            label.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
            lengthofusername = label.DesiredSize.Width;

            UseinfoCanvas.MouseEnter += (sender, eventargs) =>
            {
                UseinfoCanvas.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 247, 247, 247));
            };
            UseinfoCanvas.MouseLeave += (sender, eventargs) =>
            {
                UseinfoCanvas.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 255, 255));
            };

            UseinfoCanvas.MouseDown += (sender, eventargs) =>
            {
                click_sound_player.Play();
                Popup.Visibility = Popup.Visibility == Visibility.Hidden ? Visibility.Visible : Visibility.Hidden;
            };

            PopupMenu_Userprofile popup = new PopupMenu_Userprofile(this);
            Popup.NavigationService.Navigate(popup);

            checkedobj = General;
            //General Event Register
            canvaslist.Add(General);
            canvaslist.Add(ToDoList);
            canvaslist.Add(Notice);
            canvaslist.Add(Chat);
            canvaslist.Add(SharedFile);
            var page = new GeneralPage(username);
            TestFrame.Source = new Uri("zPageList/"+ page.Name + ".xaml", UriKind.Relative);
            TestFrame.NavigationService.Navigate(page);

            General.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 186, 224, 241));

            foreach (Canvas c in canvaslist) registerObjectToButton(c);
            //string id = DataManager.getUserTeamCode(username);
            //UserinfoImg.Source = DataManager.getTeamImage(id);
            /*DataManager.testFileUpdate(id);
            DataManager.testfiledownload(id);*/
            //NoticeCount.Content = DataManager.getNoticeAmount(username);

        }
        private void registerObjectToButton(Canvas obj)
        {
            obj.MouseEnter += (sender, eventargs) =>
            {
                horvredcanvas = obj;
            };
            obj.MouseLeave += (sender, eventargs) =>
            {
                horvredcanvas = null;
            };
            obj.MouseDown += (sender, eventargs) =>
            {
                checkedobj = obj;
                string name = obj.Name;
                Page p = null;
                if (name == "General") p = new GeneralPage(username);
                if (name == "Chat") p = new ChatPage(username);
                if (name == "Notice") p = new NoticePage(username);
                if (name == "SharedFile") p = new SharedFilePage(username);
                if (name == "ToDoList") p = new ToDoListPage(username);
                TestFrame.Source = new Uri("zPageList/" + name + "Page.xaml", UriKind.Relative);
                
                TestFrame.NavigationService.Navigate(p);

                click_sound_player.Play();
            };
        }
        private bool isOnlyEnglish(string str)
        {
            return Regex.IsMatch(str, @"/^[a-zA-Z]+$/");
        }
        public void timer_tickevent(object sender, EventArgs args)
        {

            //NoticeCount.Content = DataManager.getNoticeAmount(username);
            string today = DateTime.Now.ToString("yyyy / MM / dd");
            string currenttime = DateTime.Now.ToString("HH : mm");

            Today.Content = today;
            CurrentTime.Content = currenttime;

            foreach(var c in canvaslist)
                c.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 255, 255, 255));

            if (horvredcanvas != null) horvredcanvas.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 156, 214, 167));
            checkedobj.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 121, 191, 142));

        }
    }
}
