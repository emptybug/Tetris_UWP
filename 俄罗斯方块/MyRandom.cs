using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace 俄罗斯方块
{
    static class MyRandom
    {
        //把随机数实例放到ThreadLocal中来保证每个线程一个Randonm实例
        private static readonly ThreadLocal<Random> appRandom = new ThreadLocal<Random>(() => new Random());

        public static int GetRandomNumber() // 非负整数
        {
            return appRandom.Value.Next();
        }

        public static int GetRandomNumber(int MaxValue) // 不大于该数的非负整数
        {
            return appRandom.Value.Next(MaxValue);
        }

        public static int GetRandomNumber(int MinValue, int MaxValue) // 有下限、上限的整数
        {
            return appRandom.Value.Next(MinValue, MaxValue);
        }

    }
}
