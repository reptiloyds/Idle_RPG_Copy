using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.UnityExtension
{
    [HideMonoScript, DisallowMultipleComponent]
    public class GridTool : MonoBehaviour
    {
        [SerializeField] private int rows;
        [SerializeField] private int columns;
        [SerializeField] private Vector2 _cellSize;
        [SerializeField] private List<Transform> _cells;

        [Button]
        private void Replace()
        {
            int id = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if(id >= _cells.Count) break;
                    Vector3 position = new Vector3(j * _cellSize.x, 0, i * _cellSize.y);
                    _cells[id].localPosition = position;
                    id++;
                }
            }
        }
    }
}
