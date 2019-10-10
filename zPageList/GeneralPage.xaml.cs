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
using TeamProgramWithDatabase.GeneralPageContent;

namespace TeamProgramWithDatabase.zPageList
{
    /// <summary>
    /// GeneralPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class GeneralPage : Page
    {
        private string username;
        public GeneralPage(string username)
        {
            InitializeComponent();

            this.username = username;
            Teamname.Content = "Team ";// + DataManager.GetTeamName(DataManager.getUserTeamCode(username));


            General_Notice generalnotice = new General_Notice(username);
            GeneralNotice.Source = new Uri("GeneralPageContent/General_Notice.xaml", UriKind.Relative);
            GeneralNotice.NavigationService.Navigate(generalnotice); 
        }
    }
}
