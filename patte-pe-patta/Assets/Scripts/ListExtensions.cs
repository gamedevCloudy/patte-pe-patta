using System; 
using System.Collections;
using System.Collections.Generic;


namespace Extensions{
    public static class ListExtensions
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            Random rnd = new Random();
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = rnd.Next(i + 1);
                // Swap elements at indices i and j
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }
    }

}


