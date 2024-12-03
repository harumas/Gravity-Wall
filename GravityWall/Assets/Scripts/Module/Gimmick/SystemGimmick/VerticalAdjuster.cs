using UnityEngine;

namespace Module.Gimmick.SystemGimmick
{
    public class VerticalAdjuster
    {
        private readonly Vector3[] directions =
        {
            Vector3.up,
            Vector3.down,
            Vector3.right,
            Vector3.left,
            Vector3.forward,
            Vector3.back,
        };
        
        private readonly Vector3[] initialDirections =
        {
            Vector3.right,
            Vector3.left,
            Vector3.forward,
            Vector3.back,
        };

        /// <summary>
        /// 指定したベクトルに近い軸のベクトルを取得します。
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        public Vector3 GetVerticalDirection(Vector3 origin)
        {
            const float error = 0.01f;

            foreach (Vector3 direction in directions)
            {
                if (1f - Vector3.Dot(direction, origin) < error)
                {
                    return direction;
                }
            }

            return Vector3.zero;
        }


        public Vector3 GetNearestDirection(Vector3 origin)
        {
            Vector3 resultDir = Vector3.forward;
            float resultDot = 0f;

            foreach (Vector3 direction in initialDirections)
            {
                float dot = Vector3.Dot(direction, origin);
                if (dot > resultDot)
                {
                    resultDot = dot;
                    resultDir = direction;
                }
            }

            return resultDir;
        }
    }
}