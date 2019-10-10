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

namespace TeamProgramWithDatabase
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool CheckAutologin = true;
        public MainWindow()
        {
            InitializeComponent();
            LogButton.Click += OnPressButton;
            RegisterButton.MouseDown += OnRegisterButton;
            MainScreen screen = new MainScreen("Test");
            screen.Show();
            this.Close();
            /* if (CheckAutologin)
             {
                 DataRow row = DataManager.isAutoLogin(DataManager.getSerialNumber());
                 if(row != null)
                 {
                     MainScreen screen = new MainScreen(DataManager.GetUserInformation(row["Email"].ToString())["Username"]);
                     screen.Show();
                     this.Close();
                 }
             }*/
        }
        public void OnRegisterButton(object sender, RoutedEventArgs args)
        {
            Register register = new Register();
            register.Show();
        }
        public void OnPressButton(object sender, RoutedEventArgs args)
        {
            var bools = DataManager.CheckUser(idBox.Text, pwBox.Password);
            TestLabel.Content = idBox.Text + " / " + pwBox.Password + " / " + bools;
            if(!bools)
            {
                MessageBox.Show("계정을 다시한번 확인해주세요.\n회원가입시 입력한 이메일이 아이디입니다.");
                return;
            }
            var data = DataManager.GetUserInformation(idBox.Text);
            if (DataManager.isUserAccepted(idBox.Text))
            {
                SecLabel.Content = "";
                if (bools)
                {
                    foreach (var s in data)
                    {
                        SecLabel.Content += s.Key + " : " + s.Value + "\n";
                    }
                    MainScreen screen = new MainScreen(DataManager.GetUserInformation(idBox.Text)["Username"]);
                    screen.Show();
                    bool hasValue = AutoLogin.IsChecked.HasValue;
                    if (hasValue && AutoLogin.IsChecked.Value) DataManager.RegisterNew(DataManager.getSerialNumber(), idBox.Text, pwBox.Password);
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("당신은 아직 허용되지 않은 멤버입니다.");
            }
        }
    }
    public class PasswordBoxMonitor : DependencyObject
    {
        public static bool GetIsMonitoring(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsMonitoringProperty);
        }

        public static void SetIsMonitoring(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMonitoringProperty, value);
        }

        public static readonly DependencyProperty IsMonitoringProperty =
            DependencyProperty.RegisterAttached("IsMonitoring", typeof(bool), typeof(PasswordBoxMonitor), new UIPropertyMetadata(false, OnIsMonitoringChanged));



        public static int GetPasswordLength(DependencyObject obj)
        {
            return (int)obj.GetValue(PasswordLengthProperty);
        }

        public static void SetPasswordLength(DependencyObject obj, int value)
        {
            obj.SetValue(PasswordLengthProperty, value);
        }

        public static readonly DependencyProperty PasswordLengthProperty =
            DependencyProperty.RegisterAttached("PasswordLength", typeof(int), typeof(PasswordBoxMonitor), new UIPropertyMetadata(0));

        private static void OnIsMonitoringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pb = d as PasswordBox;
            if (pb == null)
            {
                return;
            }
            if ((bool)e.NewValue)
            {
                pb.PasswordChanged += PasswordChanged;
            }
            else
            {
                pb.PasswordChanged -= PasswordChanged;
            }
        }

        static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            var pb = sender as PasswordBox;
            if (pb == null)
            {
                return;
            }
            SetPasswordLength(pb, pb.Password.Length);
        }
    }

    //IDBox 의존객체
    public class IdBoxMonitor : DependencyObject
    {
        public static bool GetIsMonitoring(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsMonitoringProperty);
        }

        public static void SetIsMonitoring(DependencyObject 
            obj, bool value)
        {
            obj.SetValue(IsMonitoringProperty, value);
        }

        public static readonly DependencyProperty IsMonitoringProperty =
            DependencyProperty.RegisterAttached("IsMonitoring", typeof(bool), typeof(IdBoxMonitor), new UIPropertyMetadata(false, OnIsMonitoringChanged));



        public static int GetIdLength(DependencyObject obj)
        {
            return (int)obj.GetValue(IdLengthProperty);
        }

        public static void SetIdLength(DependencyObject obj, int value)
        {
            obj.SetValue(IdLengthProperty, value);
        }

        public static readonly DependencyProperty IdLengthProperty =
            DependencyProperty.RegisterAttached("IdLength", typeof(int), typeof(IdBoxMonitor), new UIPropertyMetadata(0));

        private static void OnIsMonitoringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pb = d as TextBox;
            if (pb == null)
            {
                return;
            }
            if ((bool)e.NewValue)
            {
                pb.TextChanged += TextChanged;
            }
            else
            {
                pb.TextChanged -= TextChanged;
            }
        }

        static void TextChanged(object sender, RoutedEventArgs e)
        {
            var pb = sender as TextBox;
            if (pb == null)
            {
                return;
            }
            SetIdLength(pb, pb.Text.Length);
        }
    }
}