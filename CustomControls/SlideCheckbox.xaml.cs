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

namespace TeamProgramWithDatabase.CustomControls
{
    /// <summary>
    /// SlideCheckbox.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SlideCheckbox : UserControl
    {
        private SoundPlayer click_sound_player = new SoundPlayer(Properties.Resources.Beep);
        public delegate void onChangeCheckBox();
        public event onChangeCheckBox changeevent;
        public bool isChecked {
            get
            {
                return ischecked;
            }
        }

        private bool ischecked { get; set; }
        private bool isAnimated;
        public SlideCheckbox()
        {
            InitializeComponent();
            Circle.MouseDown += events;
            BackgroundRect.MouseDown += events;
            
        }
        public void update(string username)
        {

            ischecked = DataManager.GetTeamNoticeRepeat(DataManager.getUserTeamCode(username));
            if (isChecked)
            {
                Circle.Fill = new SolidColorBrush(Color.FromArgb(255, 0, 200, 0));
                Circle.Margin = new Thickness(25, 0, 0, 0);
            }
            else
            {
                Circle.Fill = new SolidColorBrush(Color.FromArgb(255, 200, 0, 0));
                Circle.Margin = new Thickness(0, 0, 0, 0);
            }
            
            
        }
        private void events(object sender, EventArgs arg)
        {
            if (!isAnimated)
            {
                isAnimated = true;
                ischecked = !ischecked;
                click_sound_player.Play();
                changeevent();
                new Task(() =>
                {
                    int i = 1;
                    while (i <= 25)
                    {
                        i++;
                        Dispatcher.Invoke(() =>
                        {
                            Circle.Margin = new Thickness(isChecked ? i : 25 - i, 0, 0, 0);
                            if (isChecked) Circle.Fill = new SolidColorBrush(Color.FromArgb(255, Convert.ToByte(201 - 8 * i), Convert.ToByte(8 * i), 0));
                            else Circle.Fill = new SolidColorBrush(Color.FromArgb(255, Convert.ToByte(8 * i), Convert.ToByte(201 - 8 * i), 0));
                        });
                        if (i == 25) isAnimated = false;
                        System.Threading.Thread.Sleep(10);
                    }
                }).Start();
            }
        }
    }
}
