using UnityEngine;

namespace _Game.Scripts.Systems.Nutaku
{
    public static class NutakuJsBridge
    {
        public static void CreatePayment(string id, string name, string imageUrl, float price, int count, string description)
        {
            
            Application.ExternalCall("createPayment", id, 
                name, 
                imageUrl,
                price,
                count,
                description);
        }

        public static void GetProfileData()
        {
            Application.ExternalCall("getProfile");
        }
        
        public static void WebGameHandshake()
        {
            Application.ExternalCall("game_handshake");
        }
    }
}