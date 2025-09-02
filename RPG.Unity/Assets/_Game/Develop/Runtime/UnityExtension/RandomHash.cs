using System;
using System.Text;

namespace PleasantlyGames.RPG.Runtime.UnityExtension
{
    public static class RandomHash
    {
        public static string New(int length = 5) => 
            Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString())).Remove(length);
    }
}
