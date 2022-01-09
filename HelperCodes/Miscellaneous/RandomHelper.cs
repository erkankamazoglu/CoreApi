
using System;

namespace CoreApi.HelperCodes.Miscellaneous
{
    public static class RandomHelper
    {
        private static readonly Random Random = new Random();
        private static object syncObj = new object();

        public static int RealRandom(int maxVal = int.MaxValue, int minVal = int.MinValue)
        { 
            lock (syncObj)
            {
                return Random.Next(minVal, maxVal);
            }
        }
    }
}