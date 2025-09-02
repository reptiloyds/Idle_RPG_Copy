using System.Collections.Generic;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.UnityExtension.Extensions
{
    public static class ClosestExtension
    {
        public static Transform GetClosestTo(this List<Transform> list, Transform target)
        {
            float closestDistanceSquared = float.MaxValue;
            Transform closestTransform = null;

            foreach (Transform element in list)
            {
                float distanceSquared = (element.position - target.position).sqrMagnitude;

                if (distanceSquared < closestDistanceSquared)
                {
                    closestDistanceSquared = distanceSquared;
                    closestTransform = element;
                }
            }

            return closestTransform;
        }
    }
}