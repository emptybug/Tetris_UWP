using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Shapes;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Media3D;

namespace 俄罗斯方块
{
    public class Block
    {
        private int _xPos = 0; // 方块 相对背景 坐标索引
        private int _yPos = 0;
        private int _keyorder; // 方块样式的索引
        private Point[] structArr; // 方块组中 各方块的 相对4*4生成表 的 坐标索引
        public Rectangle[] recArr; // 4个矩形 存矩形的颜色、位置
        public int[] ColumnArr;
        private SolidColorBrush _blockColor; //方块颜色
        private SolidColorBrush disapperColor; // 背景颜色
        //private int rectPix;

        #region 成员属性
        public Point this[int index]
        {
            get { return structArr[index]; }
        }

        public int Length
        {
            get { return structArr.Length; }
        }

        public int XPos
        {
            get
            {
                return _xPos;
            }

            set
            {
                _xPos = value;
            }
        }

        public int YPos
        {
            get
            {
                return _yPos;
            }

            set
            {
                _yPos = value;
            }
        }

        public SolidColorBrush BlockColor
        {
            get
            {
                return _blockColor;
            }

            set
            {
                _blockColor = value;
            }
        }

        public int Keyorder
        {
            get
            {
                return _keyorder;
            }

            set
            {
                _keyorder = value;
            }
        }
        #endregion 成员属性

        public Block(Point[] sa, SolidColorBrush bColor, SolidColorBrush dColor, int keyorder)
        {
            BlockColor = bColor;
            disapperColor = dColor;
            Keyorder = keyorder;
            structArr = sa;
            XPos = 0; YPos = 0;

            int RotateTime = MyRandom.GetRandomNumber(3);
            recArr = new Rectangle[Length];
            if(keyorder != 7) for (int i = 0; i < RotateTime; ++i) this.Rotate(); // 随机旋转
            for (int i = 0; i < Length; ++i)
            {
                Point p = structArr[i];
                recArr[i] = new Rectangle();
                int x = (int)p.X + XPos, y = (int)p.Y + YPos;
                recArr[i].Fill = InfoXml.GetLinearGradientBrushFromSolidBrush(BlockColor);

                recArr[i].Margin = new Thickness(1);
                recArr[i].RadiusX = 1; recArr[i].RadiusY = 1;


                recArr[i].SetValue(Grid.RowProperty, x);
                recArr[i].SetValue(Grid.ColumnProperty, y);
            }
        }
        public Block(Point[] sa, SolidColorBrush bColor, SolidColorBrush dColor, int keyorder, int Any)
        {
            ///<summary>
            /// Pre方块组 Margin为1 Radius为1
            /// </summary>
            BlockColor = bColor;
            disapperColor = dColor;
            Keyorder = keyorder;
            structArr = sa;
            XPos = 0; YPos = 0;

            int RotateTime = MyRandom.GetRandomNumber(3);
            recArr = new Rectangle[Length];
            if (keyorder != 7) for (int i = 0; i < RotateTime; ++i) this.Rotate(); // 随机旋转
            for (int i = 0; i < Length; ++i)
            {
                Point p = structArr[i];
                recArr[i] = new Rectangle();
                int x = (int)p.X + XPos, y = (int)p.Y + YPos;
                recArr[i].Fill = InfoXml.GetLinearGradientBrushFromSolidBrush(BlockColor);
                recArr[i].Margin = new Thickness(1);
                recArr[i].RadiusX = 1; recArr[i].RadiusY = 1;


                recArr[i].SetValue(Grid.RowProperty, x);
                recArr[i].SetValue(Grid.ColumnProperty, y);
            }
        }

        private void AdjustColumn()
        {
            if (ColumnArr == null) ColumnArr = new int[Length];
            for(int i = 0; i < Length; ++i)
            {
                Point p = structArr[i];
                int y = (int)p.Y + YPos;
                ColumnArr[i] = y;
            }
        }

        public void AdjustRec()
        {
            for(int i = 0; i < Length; ++i)
            {
                Point p = structArr[i];
                Rectangle rec = recArr[i];
                int x = (int)p.X + XPos, y = (int)p.Y + YPos;
                rec.SetValue(Grid.RowProperty, x);
                rec.SetValue(Grid.ColumnProperty, y);
            }
        }

        public void Rotate()
        {
            switch(Keyorder)
            {
                case 0:
                    Rotate_Four();
                    break;
                case 1:
                    break;
                case 7:
                    break;
                default:
                    Rotate_Three();
                    break;
            }
        }

        protected void Rotate_Four()
        {
            if((int)structArr[0].X == 0 && (int)structArr[0].Y == 0)
            {
                for(int i = 0; i < Length; ++i)
                {
                    structArr[i].X = i; structArr[i].Y = 3;
                }
            }
            else if((int)structArr[0].X == 0 && (int)structArr[0].Y == 3)
            {
                for (int i = 0; i < Length; ++i)
                {
                    structArr[i].X = 3; structArr[i].Y = 3 - i;
                }
            }
            else if ((int)structArr[0].X == 3 && (int)structArr[0].Y == 3)
            {
                for (int i = 0; i < Length; ++i)
                {
                    structArr[i].X = 3 - i; structArr[i].Y = 0;
                }
            }
            else if ((int)structArr[0].X == 3 && (int)structArr[0].Y == 0)
            {
                for (int i = 0; i < Length; ++i)
                {
                    structArr[i].X = 0; structArr[i].Y = i;
                }
            }
        }
        
        protected void Rotate_Three()
        {
            for(int i = 0; i < Length; ++i)
            {
                int x = (int)structArr[i].X, y = (int)structArr[i].Y;
                if(x == 0)
                {
                    structArr[i].X = structArr[i].Y;
                    structArr[i].Y = 2;
                }
                else if(x == 2)
                {
                    structArr[i].X = structArr[i].Y;
                    structArr[i].Y = 0;
                }
                else if(x == 1 && y == 0)
                {
                    structArr[i].X = 0;
                    structArr[i].Y = 1;
                }
                else if(x == 1 && y == 2)
                {
                    structArr[i].X = 2;
                    structArr[i].Y = 1;
                }
            }
        }









        

        


    }
}
