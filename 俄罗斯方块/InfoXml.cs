using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media;

namespace 俄罗斯方块
{
    static public class InfoXml
    {

        static private int _blockTypeNum = 8;
        static XDocument xdoc = XDocument.Load(@"Xml\BlockData.xml");
        #region 成员属性
        static public int BlockTypeNum
        {
            get
            {
                return _blockTypeNum;
            }
            
        }
        #endregion
        static public Color GetRandomColor() // 获得一个随机颜色
        {
            byte alp = Convert.ToByte(255);
            byte red = Convert.ToByte(MyRandom.GetRandomNumber(128, 200));
            byte green = Convert.ToByte(MyRandom.GetRandomNumber(128, 200));
            byte blue = Convert.ToByte(MyRandom.GetRandomNumber(256));
            return Color.FromArgb(alp, red, green, blue);
        }

            ///<summary>
            /// 图形绘制 线性渐变笔刷
            /// </summary>
        static public LinearGradientBrush GetLinearGradientBrushFromSolidBrush(SolidColorBrush BlockColor)
        {
            LinearGradientBrush Brush = new LinearGradientBrush();
            Brush.StartPoint = new Point(0.2, 0);
            Brush.EndPoint = new Point(0.8, 1);
            GradientStop item1 = new GradientStop(), item2 = new GradientStop();
            item1.Offset = 1; item1.Color = BlockColor.Color;
            item2.Offset = 0; item2.Color = Colors.White;
            GradientStopCollection t = new GradientStopCollection();
            t.Add(item1);
            t.Add(item2);
            Brush.GradientStops = t;
            return Brush;
        }

        static public LinearGradientBrush Get2LinearGradientBrushFromSolidBrush(SolidColorBrush BlockColor)
        {
            LinearGradientBrush Brush = new LinearGradientBrush();
            Brush.StartPoint = new Point(0.2, 0);
            Brush.EndPoint = new Point(0.8, 1);
            GradientStop item1 = new GradientStop(), item2 = new GradientStop();
            item1.Offset = 1; item1.Color = BlockColor.Color;
            item2.Offset = 0; item2.Color = Colors.White;
            GradientStopCollection t = new GradientStopCollection();
            t.Add(item1);
            t.Add(item2);
            Brush.GradientStops = t;
            return Brush;
        }

        static public string[] GetTypeFromXml() // 从Xml文件中读取方块样式，保存进str字符串数组中
        {
            try
            {
                var byts = from bstr in xdoc.Descendants("Type") select bstr.Value;
                string[] str = new string[BlockTypeNum];
                int i = 0;
                foreach (var item in byts)
                {
                    str[i++] = item.ToString();
                }
                return str;
            }
            catch(Exception ex)
            {
                string s = ex.ToString();
            }
            return null;
        }
    }
}
