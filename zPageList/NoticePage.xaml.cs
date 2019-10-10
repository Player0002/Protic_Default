using System;
using System.Collections.Generic;
using System.Data;
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
using TeamProgramWithDatabase.CustomControls;

namespace TeamProgramWithDatabase.zPageList
{
    /// <summary>
    /// ToDoListPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class NoticePage : Page
    {
        private string username;
        public NoticePage(string username)
        {
            InitializeComponent();
            this.username = username;
            Scrolls.ScrollChanged += ecentss;
            DoThem();
        }

        public void ecentss(object sender, EventArgs args)
        {
            DoThem();
            MessageBox.Show((Scrolls.ViewportHeight - Scrolls.ScrollableHeight).ToString());
        }
        public void DoThem()
        {
            StackPan.Children.Clear();
            int i = 1;
            double length = 0;
            Dictionary<string, string> collection = DataManager.getNoticeList(username); // Noticeid, NoticeMessage
            if (collection != null)
            {
                foreach (var a in collection)
                {
                    NoticeControl contorl = new NoticeControl();
                    contorl.Height = 65;
                    contorl.setName(a.Value + " / " + a.Key);
                    StackPan.Children.Add(contorl);
                    DataManager.readnotice(username, a.Key);
                    i++;
                    length += contorl.Height;
                }
            }
            if (length >= 718) StackPan.Height = length;
        }
    }
}
