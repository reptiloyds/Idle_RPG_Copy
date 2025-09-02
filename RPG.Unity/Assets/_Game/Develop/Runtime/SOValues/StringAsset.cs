using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.SOValues
{
    [CreateAssetMenu(menuName = "SO/StringAsset", fileName = "StringAsset")]
    public class StringAsset : ScriptableObject
    {
        [SerializeField] private string _value;
        public string Value => _value;
    }
}