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
using System.Threading;
using System.Windows.Threading;

namespace TeamProgramWithDatabase.GeneralPageContent
{
    /// <summary>
    /// General_Notice.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class General_Notice : Page
    {
        DispatcherTimer timer = new DispatcherTimer();
        ThreadCurrent current = new ThreadCurrent();
        string username;
        List<string> noticelist = new List<string>();
        public General_Notice(string username)
        {
            this.username = username;
            
            InitializeComponent();
            //noticelist = DataManager.getTeamNoticeMessage(DataManager.getUserTeamCode(username));
            /*if (noticelist.Count >= 1)
            {
                BackwardMessage.Content = noticelist.ToArray()[new Random().Next(noticelist.Count)];
                CurrentMessage.Content = noticelist.ToArray()[getNextMessage(BackwardMessage.Content.ToString())];
                current.timerevent += (i) =>
                {
                    Dispatcher.Invoke(()=> {
                        Canvass.Margin = new Thickness(0, i * -1, 0, 0);
                        if(i == -1)
                        {
                            //noticelist = DataManager.getTeamNoticeMessage(DataManager.getUserTeamCode(username));
                            Canvass.Margin = new Thickness(0, 0, 0, 0);
                            BackwardMessage.Content = CurrentMessage.Content;
                            CurrentMessage.Content = noticelist.ToArray()[getNextMessage(BackwardMessage.Content.ToString())];
                        }
                    });
                };
            }*/
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += TimerEvent;
            //timer.Start();
        }
        private int getNextMessage(string msg)
        {
            if (noticelist.Count <= 1) return 0;
            else
            {
                Random rnd = new Random();
                int i = rnd.Next(noticelist.Count);
                if (!DataManager.GetTeamNoticeRepeat(DataManager.getUserTeamCode(username)))
                {
                    string[] array = noticelist.ToArray();
                    while (array[i] == msg) i = rnd.Next(noticelist.Count);
                }
                return i;
            }
        }
        public void TimerEvent(object sender, EventArgs e)
        {
            if(current.t.Status != TaskStatus.Running) current.Start();
        } 

        private class ThreadCurrent
        {

            public delegate void TimerDelegate(int time);
            public event TimerDelegate timerevent;
            private bool isStarted;
            private bool canstart = false;
            public Task t;
            public ThreadCurrent()
            {
                t = new Task(() =>
                {
                    int i = 0;
                    while (i < 45)
                    {
                        if (timerevent == null) return;
                        i++;
                        timerevent(i);
                        Thread.Sleep(10);
                    }
                    timerevent(-1);
                    isStarted = false;
                    return;
                });
            }
            public void Start()
            {
                if(t != null)
                    t = new Task(() =>
                    {
                        int i = 0;
                        while (i < 45)
                        {
                            if (timerevent == null) return;
                            i++;
                            timerevent(i);
                            Thread.Sleep(10);
                        }
                        timerevent(-1);
                        isStarted = false;
                        return;
                    });
                if (!isStarted)
                {
                    isStarted = true;
                    t.Start();
                }
            }
        }
    }
}
