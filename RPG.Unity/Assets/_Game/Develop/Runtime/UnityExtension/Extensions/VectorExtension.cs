using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.UnityExtension.Extensions
{
    public static class VectorExtension
    {
        public static float Random(this Vector2 vector) =>
            UnityEngine.Random.Range(vector.x, vector.y);

        public static int Random(this Vector2Int vector) =>
            UnityEngine.Random.Range(vector.x, vector.y + 1);

        public static void ModifyXZ(this Transform original, Vector3 target)
        {
            original.position = new Vector3(target.x,
                original.position.y, target.z);
        }
        
        public static Vector3 Parabola(Vector3 start, Vector3 end, float height, float time)
        {
            var mid = Vector3.Lerp(start, end, time);

            return new Vector3(mid.x, ParabolaCalculation(height, time) + Mathf.Lerp(start.y, end.y, time), mid.z);
        }
        
        public static float ApproximateParabolaLength(Vector3 start, Vector3 end, float height, int steps = 10)
        {
            float length = 0f;
            Vector3 prevPoint = start;

            for (int i = 1; i <= steps; i++)
            {
                float t = (float)i / steps;
                Vector3 currentPoint = Parabola(start, end, height, t);
                length += Vector3.Distance(prevPoint, currentPoint);
                prevPoint = currentPoint;
            }

            return length;
        }

        private static float ParabolaCalculation(float height, float time) =>
            -4 * height * time * time + 4 * height * time;
    }
}