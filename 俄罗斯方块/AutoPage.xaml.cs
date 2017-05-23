using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
        private void Btn_Stop_Click(object sender, RoutedEventArgs e) // 暂停
        {
            Btn_Continue.IsEnabled = true;
            Btn_Stop.IsEnabled = false;

            Btn_Continue2.IsEnabled = true;
            Btn_Stop2.IsEnabled = false;
            Game_Stop();
        }
        private void Btn_Continue_Click(object sender, RoutedEventArgs e) // 继续
        {
            Btn_Continue.IsEnabled = false;
            Btn_Stop.IsEnabled = true;
            Btn_Continue2.IsEnabled = false;
            Btn_Stop2.IsEnabled = true;
            Game_Start();
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
            Btn_Continue.IsEnabled = false;
            Btn_Stop.IsEnabled = true;
            Btn_Continue2.IsEnabled = false;
            Btn_Stop2.IsEnabled = true;
            xaml_Sound_Background.Position = new TimeSpan(0);
            xaml_Sound_Background.Play();
        }
        private void Btn_Sound_Click(object sender, RoutedEventArgs e) // 静音
        {
            if(Btn_Sound.Label == "音乐")
            {
                xaml_Sound_Background.Pause();
                Btn_Sound2.Label = "静音";
                Btn_Sound2.Icon = new SymbolIcon(Symbol.Mute);
                Btn_Sound.Label = "静音";
                Btn_Sound.Icon = new SymbolIcon(Symbol.Mute);
            }
            else
            {
                xaml_Sound_Background.Play();
                Btn_Sound2.Label = "音乐";
                Btn_Sound2.Icon = new SymbolIcon(Symbol.Volume);
                Btn_Sound.Label = "音乐";
                Btn_Sound.Icon = new SymbolIcon(Symbol.Volume);
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
            Btn_Continue.IsEnabled = false;
            Btn_Stop.IsEnabled = false;
            Display.IsPause = true;
            xaml_Sound_Background.Stop();
            xaml_Sound_Win.Play();
            await new MessageDialog("你赢了！！！").ShowAsync();
        }

        private async void Game_Over()
        {
            timer.Stop();
            Btn_Continue.IsEnabled = false;
            Btn_Stop.IsEnabled = false;
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

        private void xaml_MainGrid_KeyDown(object sender, KeyRoutedEventArgs e) // 键盘事件 处理
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
    }
}
