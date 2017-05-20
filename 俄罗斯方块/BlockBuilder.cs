using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace 俄罗斯方块
{
    class BlockBuilder
    {
        private SolidColorBrush disapperColor;
        private string[] strArr;
        public BlockBuilder(SolidColorBrush Background)
        {
            disapperColor = Background;
            strArr = InfoXml.GetTypeFromXml();
        }

        public Block GetABlock()
        {
            int keyOrder = MyRandom.GetRandomNumber(strArr.Length - 1);
            int P_num = 0;
            string str = strArr[keyOrder];
            for(int i = 0; i < str.Length; ++i)
            {
                if (str[i] == '1') P_num++;
                
            }
            Point[] P_Arr = new Point[P_num];
            int k = 0;
            for (int i = 0; i < str.Length; ++i)
            {
                if (str[i] == '1')
                {
                    P_Arr[k].X = i / 4;
                    P_Arr[k].Y = i % 4;
                    ++k;
                }
            }
            
            return new Block(P_Arr, new SolidColorBrush(InfoXml.GetRandomColor()), disapperColor, keyOrder);
        }


        public Block GetASpecialBlock()
        {
            int keyOrder = 7;
            int P_num = 0;
            string str = strArr[keyOrder];
            for (int i = 0; i < str.Length; ++i)
            {
                if (str[i] == '1') P_num++;

            }
            Point[] P_Arr = new Point[P_num];
            int k = 0;
            for (int i = 0; i < str.Length; ++i)
            {
                if (str[i] == '1')
                {
                    P_Arr[k].X = i / 4;
                    P_Arr[k].Y = i % 4;
                    ++k;
                }
            }

            return new Block(P_Arr, new SolidColorBrush(InfoXml.GetRandomColor()), disapperColor, keyOrder);
        }
    }
}
