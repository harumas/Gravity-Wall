using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace View.View
{
    public class ClearedLevelView : MonoBehaviour
    {
        [SerializeField] private Image[] clearedLevelIcons;
        [SerializeField] private Color clearedColor;
        [SerializeField] private Color notClearedColor;

        public void SetClearedLevels(bool[] clearedLevels)
        {
            for (var i = 0; i < clearedLevelIcons.Length; i++)
            {
                // チュートリアルは0番目なので無視
                if (clearedLevels[i + 1])
                {
                    clearedLevelIcons[i].color = clearedColor;
                }
                else
                {
                    clearedLevelIcons[i].color = notClearedColor;
                }
            }
        }
    }
}