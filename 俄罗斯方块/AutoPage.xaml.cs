using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace 俄罗斯方块
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AutoPage : Page
    {
        private Palette Display;
        private DispatcherTimer timer;
        private TimeSpan TimeInitLevel;
        private int _time = 0;

        public AutoPage()
        {
            this.InitializeComponent();
            Init();
        }
       
        #region GM工具
        private void Btn_Left_Click(object sender, RoutedEventArgs e)
        {
            Display.MoveLeft();
        }

        private void Btn_Down_Click(object sender, RoutedEventArgs e)
        {
            Display.MoveDown();
        }
        
        private void Btn_DownEnd_Click(object sender, RoutedEventArgs e)
        {
            Display.MoveDownEnd();
        }

        private void Btn_Right_Click(object sender, RoutedEventArgs e)
        {
            Display.MoveRight();
        }

        private void Btn_Rotate_Click(object sender, RoutedEventArgs e)
        {
            Display.Rotate();
        }

        private void Btn_CreateBackground_Click(object sender, RoutedEventArgs e)
        {
            Layout();
        }

        private void Btn_CreateNewBlock_Click(object sender, RoutedEventArgs e)
        {
            Display.DrawNewBlock();
        }

        private void Btn_ClearOneRow_Click(object sender, RoutedEventArgs e)
        {
            Display.Remove();
        }
        #endregion

        private void Btn_Control_Click(object sender, RoutedEventArgs e) // 控制暂停继续
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            if (Btn_Control.Label.Equals(loader.GetString("Button_Pause")))
            {
                Btn_Control.Label = loader.GetString("Button_Start");
                Btn_Control.Icon = new SymbolIcon(Symbol.Play);
                Game_Stop();
            }
            else
            {
                Btn_Control.Label = loader.GetString("Button_Pause");
                Btn_Control.Icon = new SymbolIcon(Symbol.Pause);
                Game_Start();
                xaml_appbar.IsOpen = false;
            }

        }

        private void Btn_Back_Click(object sender, RoutedEventArgs e) // 退出
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void Btn_Refresh_Click(object sender, RoutedEventArgs e) // 重新开始
        {
            Game_Stop();
            Layout();
            timer.Interval = TimeInitLevel;
            Game_Start();
            Btn_Control.IsEnabled = true;

            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            Btn_Control.Label = loader.GetString("Button_Pause");
            Btn_Control.Icon = new SymbolIcon(Symbol.Pause);

            xaml_Sound_Background.Position = new TimeSpan(0);
            xaml_Sound_Background.Play();
            xaml_appbar.IsOpen = false;
        }
        private void Btn_Sound_Click(object sender, RoutedEventArgs e) // 静音
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var temp_str = loader.GetString("Button_Music");
            if (Btn_Sound.Label.Equals(temp_str))
            {
                xaml_Sound_Background.Pause();
                Btn_Sound.Label = loader.GetString("Button_Mute");
                Btn_Sound.Icon = new SymbolIcon(Symbol.Mute);
            }
            else
            {
                xaml_Sound_Background.Play();
                Btn_Sound.Label = loader.GetString("Button_Music");
                Btn_Sound.Icon = new SymbolIcon(Symbol.Volume);
            }
            
        }

        /// <summary>
        /// 开启左上角返回键
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            string myPages = "";
            foreach (PageStackEntry page in rootFrame.BackStack)
            {
                myPages += page.SourcePageType.ToString() + "\n";
            }
            //stackCount.Text = myPages;

            if (rootFrame.CanGoBack)
            {
                // Show UI in title bar if opted-in and in-app backstack is not empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            }
            else
            {
                // Remove the UI from the title bar if in-app back stack is empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }
        }

        /// <summary>
        /// 控件初始化
        ///
        /// </summary>
        private void Init()
        {
            _time = 0;
            timer = new DispatcherTimer();
            TimeInitLevel = new TimeSpan(0, 0, 0, 0, 480); // 500ms
            Layout(); // 布局函数
            /// <summary>
            /// 时间
            /// </summary>
            timer.Tick += Timer_Tick;
            timer.Interval = TimeInitLevel;
            timer.Start();

            Display.IsPause = false;

            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            Btn_Sound.Label = loader.GetString("Button_Music");
            Btn_Control.Label = loader.GetString("Button_Pause");

            SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;
            //    (sender, e) =>
            //{
            //    Game_Stop();
            //    this.Frame.Navigate(typeof(MainPage));
            //};

            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
            {
                xaml_Control.Visibility = Visibility.Visible;
            }
            else
            {
                xaml_Control.Visibility = Visibility.Collapsed;
            }

            //UpdateMainGrid(Data_RP.ActualHeight);
        }

        /// <summary>
        /// 设置返回键内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_BackRequested(object sender, Windows.UI.Core.BackRequestedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null) return;

            // Navigate back if possible, and if the event has not 
            // already been handled .
            if (rootFrame.CanGoBack
                && e.Handled == false
                )
   {
                e.Handled = true;
                Game_Stop();
                rootFrame.GoBack();
            }
        }

        private void Timer_Tick(object sender, object e)
        {
            _time++;
            if (Display.GameMain()) // 游戏可以正常进行
            {
                if(_time % 84 == 0)
                {
                    //Display.SpecialPlay();
                }

                if (Is_LevelChange()) // 若 等级改变
                {
                    int mm = timer.Interval.Milliseconds;
                    if(Display.Level == 2)
                    {
                        timer.Interval = new TimeSpan(0, 0, 0, 0, 400);
                    }
                    else if (Display.Level == 3)
                    {
                        timer.Interval = new TimeSpan(0, 0, 0, 0, 320);
                    }
                    else if (Display.Level == 4)
                    {
                        timer.Interval = new TimeSpan(0, 0, 0, 0, 240);
                    }
                    else if (Display.Level == 5)
                    {
                        timer.Interval = new TimeSpan(0, 0, 0, 0, 160);
                    }
                    else if (Display.Level == 6)
                    {
                        timer.Interval = new TimeSpan(0, 0, 0, 0, 80);
                    }
                    else if (Display.Level == 7)
                    {
                        Game_Win();
                    }
                    // 播放
                    xaml_Sound_LevelUp.Position = new TimeSpan(0);
                    xaml_Sound_LevelUp.Play();
                }
                TickBinding(); // 实例数据更新到页面
            }
            else // 游戏结束
            {
                Game_Over();
            }
        }

        private bool Is_LevelChange()
        {
            int level = Convert.ToInt32(xaml_Level.Text);
            if (level != Display.Level)
            {
                return true;
            }
            else return false;
        }

        private void TickBinding() // 游戏界面 分数 等级 目标分数 显示
        {
            xaml_Score.Text = Display.Score.ToString();
            xaml_Level.Text = Display.Level.ToString();
            xaml_Target.Text = Display.Target.ToString();
        }

        private void Game_Start()
        {
            timer.Start();
            xaml_Sound_Background.Play();
            Display.IsPause = false;
        }

        private void Game_Stop()
        {
            timer.Stop();
            xaml_Sound_Background.Pause();
            Display.IsPause = true;
        }

        private async void Game_Win()
        {
            timer.Stop();
            Btn_Control.IsEnabled = false;
            Display.IsPause = true;
            xaml_Sound_Background.Stop();
            xaml_Sound_Win.Play();
            await new MessageDialog("你赢了！！！").ShowAsync();
        }

        private async void Game_Over()
        {
            timer.Stop();
            Btn_Control.IsEnabled = false;
            Display.IsPause = true;
            xaml_Sound_Background.Stop();
            xaml_Sound_Over.Play();
            await new MessageDialog("游戏结束！").ShowAsync();
        }
        
        private void Layout()
        {
            if(Display != null)
            {
                Display.Clear();
            }
            Display = new Palette((SolidColorBrush)xaml_MainGrid.Background, ref xaml_MainGrid, ref xaml_PreGrid);
                
            ///<summary>
            /// 设置音效控件
            /// </summary>
            Display.SetSoundDown(ref xaml_Sound_DownEnd);
            Display.SetSoundRemove(ref xaml_Sound_Remove);
            Display.SetSoundRotate(ref xaml_Sound_Rotate);

            ///<summary>
            /// 设置数字界面
            /// </summary>
            xaml_Score.Text = "0";
            xaml_Level.Text = "1";
            xaml_Target.Text = "500";

            xaml_MainGrid.Children.Clear();
            xaml_MainGrid.RowDefinitions.Clear();
            xaml_MainGrid.ColumnDefinitions.Clear();
            xaml_PreGrid.Children.Clear();
            xaml_PreGrid.RowDefinitions.Clear();
            xaml_PreGrid.ColumnDefinitions.Clear();
            #region 主游戏界面布局
            for (int i = 0; i < Display.Row; ++i) //行
            {
                RowDefinition r = new RowDefinition();
                xaml_MainGrid.RowDefinitions.Add(r);
            }
            for (int i = 0; i < Display.Column; ++i) //列
            {
                ColumnDefinition c = new ColumnDefinition();
                xaml_MainGrid.ColumnDefinitions.Add(c);
            }
#endregion
            #region 预测界面布局
            for(int i = 0; i < 4; ++i)
            {
                xaml_PreGrid.RowDefinitions.Add(new RowDefinition());
                xaml_PreGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
#endregion

        }

        private void Control_KeyDown(object sender, KeyRoutedEventArgs e) // 键盘事件 处理
        {
            switch ((int)e.Key)
            {
                case 32:
                    Display.MoveDownEnd();
                    break;
                case 37:  // 左
                    Display.MoveLeft();
                    break;
                case 38:  // 上
                    Display.Rotate();
                    break;
                case 39: // 右
                    Display.MoveRight();
                    break;
                case 40: // 下
                    Display.MoveDownEnd();
                    break;
                default:
                    break;
            }
        }

        private void xaml_appbar_Opened(object sender, object e)
        {

        }

        private void Page_KeyDown(object sender, KeyRoutedEventArgs e)
        {

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
        }

        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                //case VirtualKey.Down:
                //    Display.MoveDownEnd();
                //    break;
                case VirtualKey.Left:  // 左
                    Display.MoveLeft();
                    break;
                case VirtualKey.Up:  // 上
                    Display.Rotate();
                    break;
                case VirtualKey.Right: // 右
                    Display.MoveRight();
                    break;
                case VirtualKey.Down: // 下
                    Display.MoveDownEnd();
                    break;
                default:
                    break;
            }
        }

        private void xaml_MainGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateMainGrid(e.NewSize.Height);
        }

        private void UpdateMainGrid(double height)
        {
            xaml_MainGrid.Height = height;
            xaml_MainGrid.Width = height * 3 / 5;
            xaml_PreGrid.Width = xaml_PreGrid.Height = height / 6;
            
            //实时调整显示面板的长宽
            if (xaml_MainGrid.Width >= Data_RP.ActualWidth / 2)
            {
                xaml_MainGrid.SetValue(RelativePanel.AlignRightWithPanelProperty, true);
                xaml_MainGrid.SetValue(RelativePanel.AlignHorizontalCenterWithPanelProperty, false);
                xaml_MainGrid.Margin = new Thickness(0, 12, 12, 0);

                Sp_Pre.SetValue(RelativePanel.AlignLeftWithPanelProperty, true);
                Sp_Pre.SetValue(RelativePanel.AlignHorizontalCenterWithPanelProperty, false);
                double temp = ((Data_RP.ActualWidth / 2 - (xaml_MainGrid.Width - Data_RP.ActualWidth / 2)) - Sp_Pre.ActualWidth) / 2;
                Sp_Pre.Margin = new Thickness(temp, 0, 0, 0);
            }
            else
            {
                xaml_MainGrid.SetValue(RelativePanel.AlignRightWithPanelProperty, false);
                xaml_MainGrid.SetValue(RelativePanel.AlignHorizontalCenterWithPanelProperty, true);
                xaml_MainGrid.Margin = new Thickness(xaml_MainGrid.Width  + 12, 12, 12, 0);

                Sp_Pre.SetValue(RelativePanel.AlignLeftWithPanelProperty, false);
                Sp_Pre.SetValue(RelativePanel.AlignHorizontalCenterWithPanelProperty, true);
                Sp_Pre.Margin = new Thickness(0, 0, Sp_Pre.ActualWidth + 12, 0);
            }



            double a = Sp_Pre.ActualWidth;
            double b = xaml_MainGrid.ActualWidth;
            double c = Root_Grid.ActualWidth;
        }

        private void Data_RP_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateMainGrid(e.NewSize.Height - 24);
        }
    }
}
