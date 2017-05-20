using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media.Imaging;

namespace 俄罗斯方块
{
    class Palette
    {
        private int _score = 0; // 分数
        private int _target = 500; // 目标分数
        private int _level = 1; // 等级
        private bool _isPause = true; //是否 暂停状态
        private int _row = 20; //界面行数
        private int _column = 12;
        private bool[,] staArr; //二维状态
        private SolidColorBrush disapperColor; // 背景色
        private BlockBuilder bBuilder; 
        private Block preBlock; //预测 方块组
        private Block runBlock; // 正在运行 方块组
        private Grid MainGrid; // 即将绘画的面板
        private Grid PreGrid; // 预测画板
        /// <summary>
        /// 声音组
        /// </summary>
        private MediaElement SoundDown; 
        private MediaElement SoundRemove;
        private MediaElement SoundRotate;
        #region 属性
        public int Row
        {
            get
            {
                return _row;
            }

            set
            {
                _row = value;
            }
        }

        public int Column
        {
            get
            {
                return _column;
            }

            set
            {
                _column = value;
            }
        }
        
        
        public int Score
        {
            get
            {
                return _score;
            }

            set
            {
                _score = value;
            }
        }

        public bool IsPause
        {
            get
            {
                return _isPause;
            }

            set
            {
                _isPause = value;
            }
        }

        public int Level
        {
            get
            {
                return _level;
            }

            set
            {
                _level = value;
            }
        }

        public int Target
        {
            get
            {
                return _target;
            }

            set
            {
                _target = value;
            }
        }
        #endregion
        public void SetSoundDown(ref MediaElement media)
        {
            SoundDown = media;
        }
        public void SetSoundRemove(ref MediaElement media)
        {
            SoundRemove = media;
        }
        public void SetSoundRotate(ref MediaElement media)
        {
            SoundRotate = media;
        }
        public Palette(SolidColorBrush dColor, ref Grid MainGrid, ref Grid PreGrid)
        {
            disapperColor = dColor;
            this.MainGrid = MainGrid;
            this.PreGrid = PreGrid;
            IsPause = true;
            staArr = new bool[Row, Column];
            Score = 0;
            Target = 500;
            Level = 1;
            for (int i = 0; i < Row; ++i)
            {
                for (int j = 0; j < Column; ++j)
                {
                    staArr[i, j] = false;
                }
            }
        }
        
        public void Clear()
        {
            SoundDown = null;
            SoundRemove = null;
            SoundRotate = null;
            disapperColor = null;
            staArr = null;
            runBlock = preBlock = null;
            MainGrid = PreGrid = null;
        }

        public void SpecialPlay()
        {
            BlockBuilder bb = new BlockBuilder(disapperColor);
            preBlock = bb.GetASpecialBlock();
            SetPreBlock();
        }

        #region 方块移动相关

        public bool MoveUp()
        {
            if(CanMove(-1, 0))
            {
                SoundRotate.Position = new TimeSpan(0);
                SoundRotate.Play(); // 播放
                return true;
            }
            return false;
        }

        public bool MoveLeft()
        {
            if (CanMove(0, -1))
            {
                SoundRotate.Position = new TimeSpan(0);
                SoundRotate.Play(); // 播放
                return true;
            }
            return false;
        }

        public bool MoveRight()
        {
            if (CanMove(0, 1))
            {
                SoundRotate.Position = new TimeSpan(0);
                SoundRotate.Play(); // 播放
                return true;
            }
            return false;
        }

        public bool MoveDown()
        {
            if (CanMove(1, 0))
            {
                SoundRotate.Position = new TimeSpan(0);
                SoundRotate.Play(); // 播放
                return true;
            }
            return false;
        }

        public bool MoveDown2() // 无声 版本
        {
            return CanMove(1, 0);
        }

        public bool MoveDownEnd()
        {
            if (runBlock == null) return false;
            int x = runBlock.XPos;
            while (MoveDown2()) ;
            if (x != runBlock.XPos)
            {
                SoundDown.Position = new TimeSpan(0);
                SoundDown.Play();
                return true;
            }
            else return false;
        }
    
        public bool GameMain() // 时间间隔事件
        {
            if (runBlock == null) DrawNewBlock();
            if (MoveDown2())
            {
                return true;
            }
            else
            {
                AddStaArr(); // 使到底部 的 方块组 存入 二维状态数组
                Remove(); // 消行函数
                if (!DrawNewBlock()) //不能绘制 游戏结束
                {
                    IsPause = true;
                    Clear();
                    return false;
                }
                else return true;
            }
        }
        
        public bool DrawNewBlock() // 判断 下一个方块 能否 绘制
        {
            GetRunBlock();
            SetPreBlock();
            if(CanShowBlock())
            {

                return true;
            }
            // 输了
            else 
            {
                
                return false;
            }
        }
        
        public void Rotate() // 提供给外部的 旋转
        {
            if(CanRotate())
            {
                SoundRotate.Position = new TimeSpan(0);
                SoundRotate.Play(); // 播放
            }
        } 

        protected bool CanRotate() // 判断 是否能够 旋转
        {
            if (IsPause || runBlock == null || runBlock.Keyorder == 7 || runBlock.Keyorder == 1) return false;
            Block b = runBlock;
            b.Rotate();
            if(CanMove(0, 0))
            {
                return true;
            }
            else
            {
                bool canRotate = false;
                if (!InGrid(b))
                {
                    for (int i = 1; i < 4; ++i)
                    {
                        if (CanMove(0, -i))
                        {
                            canRotate = true;
                            break;
                        }
                        if (CanMove(0, i))
                        {
                            canRotate = true;
                            break;
                        }
                    }
                }
                
                if (!canRotate)
                {
                    for (int i = 0; i < 3; ++i) b.Rotate();
                    return false;
                }
                return true;
            }
        }

        public void Remove() // 提供给外部的 清除
        {
            int n = RemoveLine();
            if(n > 0) 
            {
                CalculateScore(n);
                SoundRemove.Position = new TimeSpan(0);
                SoundRemove.Play();
            }
            
        }

        private void CalculateScore(int n) // 计算得分
        {
            int s = 1;
            for(int i = 1; i <= n; ++i)
            {
                s *= 2;
            }
            Score += (s - 1) * 100;
            if(Score > 0) CalculateLevel();
        }
        
        private void LevelChange() // 等级 变化时 发生事件
        {
            //一次获得的分数<=1500
            if(500 <= Score && Score < 1000)
            {
                Level = 2;
                Target = 1000;
            }
            else if(1000 <= Score && Score < 2000)
            {
                Level = 3;
                Target = 2000;
            }
            else if(2000 <= Score && Score <= 4000)
            {
                Level = 4;
                Target = 4000;
            }
            else
            {
                Level++;
                Target *= 2;
            }
            
        }

        private void CalculateLevel() // 计算等级
        {
            if(Score >= Target)
            {
                LevelChange();
            }
        }

        protected bool CanMove(int _x, int _y) // 判断 是否能够移动 x，y 的坐标
        {
            if (IsPause || runBlock == null) return false;
            Block b = runBlock;
            for (int i = 0; i < b.Length; ++i)
            {
                Point p = b[i];
                int x = (int)p.X + b.XPos + _x, y = (int)p.Y + b.YPos + _y;
                if (!InGrid(x, y) || Exist(x, y)) return false; 
            }

            runBlock.XPos += _x; runBlock.YPos += _y;
            b.AdjustRec(); // 方块组类 的 调整矩形位置并重新绘制
            return true;
        }
        

        #endregion

        protected int RemoveLine() // 清除满方块的多行
        {

            int RemoveLineCount = 0;
            int[] LineBlockCount = new int[Row];
            for (int i = 0; i < LineBlockCount.Length; ++i) LineBlockCount[i] = 0;

            foreach (var rec in MainGrid.Children)
            {
                if (rec is Rectangle)
                {
                    int row = Convert.ToInt32((rec as Rectangle).GetValue(Grid.RowProperty));
                    LineBlockCount[row]++;
                }
            }
            for (int r = Row - 1; r >= 0; --r)
            {
                if (LineBlockCount[r] == Column)
                {
                    // 消去一行
                    for (int i = 0; i < MainGrid.Children.Count; ++i)
                    {
                        if (MainGrid.Children[i] is Rectangle)
                        {
                            int row = Convert.ToInt32((MainGrid.Children[i] as Rectangle).GetValue(Grid.RowProperty));
                            if (row == r + RemoveLineCount)
                            {
                                int column = Convert.ToInt32((MainGrid.Children[i] as Rectangle).GetValue(Grid.ColumnProperty));
                                MainGrid.Children.Remove(MainGrid.Children[i] as Rectangle);
                                i--;
                            }
                        }
                    }
                    // 使上方的 方块 下落
                    for (int i = 0; i < MainGrid.Children.Count; ++i)
                    {
                        if (MainGrid.Children[i] is Rectangle)
                        {
                            int row = Convert.ToInt32((MainGrid.Children[i] as Rectangle).GetValue(Grid.RowProperty));
                            if (row < r + RemoveLineCount)
                            {
                                int column = Convert.ToInt32((MainGrid.Children[i] as Rectangle).GetValue(Grid.ColumnProperty));
                                (MainGrid.Children[i] as Rectangle).SetValue(Grid.RowProperty, row + 1);
                            }
                        }
                    }
                    RemoveLineCount++;
                }
            }
            for (int i = 0; i < Row; ++i)
            {
                for (int j = 0; j < Column; ++j)
                {
                    staArr[i, j] = false;
                }
             }
             foreach (var rec in MainGrid.Children)
             {
                if (rec is Rectangle)
                {
                    int row = Convert.ToInt32((rec as Rectangle).GetValue(Grid.RowProperty));
                    int column = Convert.ToInt32((rec as Rectangle).GetValue(Grid.ColumnProperty));
                    staArr[row, column] = true;
                }
            }
            return RemoveLineCount;
        }

        protected bool InGrid(int _x, int _y) // 判断 点 是否在画板内
        {
            if (0 <= _x && _x < Row && 0 <= _y && _y < Column) return true;
            return false;
        }

        protected bool InGrid(Block b)
        {
            for (int i = 0; i < b.Length; ++i)
            {
                Point p = b[i];
                int x = (int)p.X + b.XPos, y = (int)p.Y + b.YPos;
                if (!(0 <= x && x < Row && 0 <= y && y < Column)) return false;
            }
            return true;
        }

        protected bool Exist(int _x, int _y) // 判断 点 是否已被占用
        {
            if (staArr[_x, _y]) return true;
            return false;
        } 

        protected bool Exist(Block b)
        {
            for (int i = 0; i < b.Length; ++i)
            {
                Point p = b[i];
                int x = (int)p.X + b.XPos , y = (int)p.Y + b.YPos ;
                if (!staArr[x, y]) return false;
            }
            return true;
        }
        
        private void AddStaArr() // 给 二维状态数组 附加状态
        {
            if (runBlock == null) return;
            for(int i = 0; i < runBlock.Length; ++i)
            {
                Point p = runBlock[i];
                int x = (int)p.X + runBlock.XPos, y = (int)p.Y + runBlock.YPos;
                staArr[x, y] = true;
            }
        }

        private bool CanShowBlock()
        {
            if(CanMove(0, 0)) // 新的方块是否能在画面中出现
            {
                for(int i = 0; i < runBlock.Length; ++i)
                {
                    MainGrid.Children.Add(runBlock.recArr[i]);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private void GetRunBlock() //得到一个运动的方块
        {
            bBuilder = new BlockBuilder(disapperColor);
            if(runBlock == null)
            {
                Block New_Block = bBuilder.GetABlock();
                New_Block.XPos = 0; New_Block.YPos = (Column - 1) / 2 - 1;
                runBlock = New_Block;
            }
            else
            {
                runBlock = preBlock;
                runBlock.XPos = 0; runBlock.YPos = (Column - 1) / 2 - 1;
            }
            preBlock = bBuilder.GetABlock();
        }

        private void SetPreBlock() // 设置 预测方块布局中 方块属性
        {
            Block b = preBlock;
            PreGrid.Children.Clear();
            PreGrid.RowDefinitions.Clear();
            PreGrid.ColumnDefinitions.Clear();
            for (int i = 0; i < 4; ++i)
            {
                PreGrid.RowDefinitions.Add(new RowDefinition());
                PreGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for(int i = 0; i < b.Length; ++i)
            {
                PreGrid.Children.Add(b.recArr[i]);
            }
        }

        public void AddCanvas(ref Grid grid, int row, int column, SolidColorBrush color) //给grid添加Canvas
        {
            Canvas canvas = new Canvas();
            canvas.Background = color;
            canvas.SetValue(Grid.RowProperty, row);
            canvas.SetValue(Grid.ColumnProperty, column);
            grid.Children.Add(canvas);
        }
        
        public bool[,] TestGetStaArr()
        {
            return staArr;
        }
    }
}
