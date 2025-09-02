using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace PleasantlyGames.RPG.Runtime.Core.Balance.Tool
{
     public static class ConfigSubTableHelper
    {
        public static string FromFactoryAsList<T>(List<Dictionary<string, string>> data,
            [NotNull] Func<Dictionary<string, string>, T> factory) =>
            JsonConvert.SerializeObject(data.Select(factory.Invoke).ToList());

        public static string FromFactoryAsSelf<T>(List<Dictionary<string, string>> data,
            [NotNull] Func<List<Dictionary<string, string>>, T> factory) =>
            JsonConvert.SerializeObject(factory.Invoke(data));
        
        /// <summary>
        /// Находит координаты строк начала и конца тега
        /// </summary>
        public static SubTableRect GetSubTableRect(IList<IList<object>> rows, string subStableName)
        {
            int startRow = -1, startCol = -1, endRow = -1, endCol = -1;

            for (var rowNum = 0; rowNum < rows.Count; rowNum++) {
                IList<object> row = rows[rowNum];

                if (IsTagExistInRow(row, StartTag(subStableName), out int colIndex)) {
                    startRow = rowNum + 1;
                    startCol = colIndex + 1;
                    continue;
                }

                if (IsTagExistInRow(row, EndTag(subStableName), out colIndex)) {
                    endRow = rowNum - 1;
                    endCol = colIndex;
                }
            }
            return new SubTableRect(startRow, startCol, endRow, endCol);
        }

        private static string StartTag(string subTableName) => $"start.{subTableName}";
        private static string EndTag(string subTableName) => $"end.{subTableName}";

        private static bool IsTagExistInRow(IList<object> row, string tag, out int colIndex)
        {
            colIndex = row.IndexOf(tag);
            return colIndex != -1;
        }
    }

    public readonly struct SubTableRect
    {
        public readonly int StartTagRowNum;
        public readonly int StartTagColNum;
        public readonly int EndTagRowNum;
        public readonly int EndTagColNum;

        public SubTableRect(int startTagRowNum,
            int startTagColNum,
            int endTagRowNum,
            int endTagColNum)
        {
            StartTagRowNum = startTagRowNum;
            StartTagColNum = startTagColNum;
            EndTagRowNum = endTagRowNum;
            EndTagColNum = endTagColNum;
        }

        public bool IsValid() => !(StartTagRowNum == -1 || StartTagColNum == -1 || EndTagRowNum == -1 || EndTagColNum == -1);

        /// <summary>
        /// Итерирует по сабтаблице игнорируя 0 строку с ключами
        /// </summary>
        public IEnumerable<int> IterateRow()
        {
            for (int rowNum = StartTagRowNum; rowNum <= EndTagRowNum; rowNum++)
                yield return rowNum;
        }

        public IEnumerable<int> IterateCol()
        {
            for (int colNum = StartTagColNum; colNum < EndTagColNum; colNum++)
                yield return colNum;
        }
    }
}