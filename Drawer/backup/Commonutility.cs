using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    static class Commonutility
    {
        public static void Reverse<T>(this T[] array)
        {
            for(int i = 0; i < array.Length; i++)
            {
                var tmp = array[i];
                array[i] = array[array.Length - 1 - i];
                array[array.Length - 1 - i] = tmp;
            }
        }
    }
}
