using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.UnityExtension.Extensions
{
    public class StringExtension
    {
        private static StringExtension _instance;
        private static StringBuilder _stringBuilder;

        private readonly List<string> _postfixes = new();

        public static StringExtension Instance => _instance;

        public static void Build(List<string> postfixes)
        {
            _instance = new StringExtension();
            _stringBuilder = new StringBuilder();
            _instance._postfixes.Add("");
            _instance._postfixes.AddRange(postfixes);
            for (var i = 'A'; i <= 'Z'; i++) 
                _instance._postfixes.Add(i.ToString());
            
            for (var first = 'A'; first <= 'Z'; first++)
            for (var second = 'A'; second <= 'Z'; second++) 
                _instance._postfixes.Add($"{first}{second}");   
        }
        
        public string CutDouble(double number, bool roundFirstGrade = false)
        {
            int index = 0;
            while (number >= 1000 && index < _postfixes.Count - 1)
            {
                number /= 1000;
                index++;
            }

            _stringBuilder.Clear();
            var intNumber = (int)number;
            var afterDot = number - intNumber;
            
            if (afterDot < 0.01f || index == 0 && roundFirstGrade)
                _stringBuilder.Append(intNumber);
            else
            {
                var result = $"{number.ToString("F2")}";
                _stringBuilder.Append(result);   
            }
            
            _stringBuilder.Append(_postfixes[index]);

            return _stringBuilder.ToString();
        }

        public string CutBigDouble(BigDouble.Runtime.BigDouble number, bool roundFirstGrade = false)
        {
            if (IsZero(number.Mantissa)) return "0";

            var suffixIndex = (int)(number.Exponent / 3);
            var scaledMantissa = number.Mantissa * Math.Pow(10, number.Exponent % 3);

            if (suffixIndex == 0 && roundFirstGrade)
                return $"{(int)scaledMantissa}";

            if (suffixIndex >= _postfixes.Count)
                return $"{scaledMantissa:F2}e{number.Exponent}";

            var delta = scaledMantissa - (int)scaledMantissa;
            if (delta < 0.01f)
            {
                if (delta > 0.009f)
                    return $"{RoundUpToTwoDecimals(scaledMantissa):F2}{_postfixes[suffixIndex]}";
                return $"{(int)scaledMantissa}{_postfixes[suffixIndex]}";   
            }

            return $"{scaledMantissa:F2}{_postfixes[suffixIndex]}";
        }
        
        public string RoundBigDouble(BigDouble.Runtime.BigDouble number) => 
            Mathf.CeilToInt((float)number.ToDouble()).ToString();

        public static double RoundUpToTwoDecimals(double value) => 
            Math.Ceiling(value * 100) / 100;

        public string TypeToString<T>() => typeof(T).Name.Split(".")[^1];
        
        private static bool IsZero(double value) => 
            Math.Abs(value) < double.Epsilon;
    }
}