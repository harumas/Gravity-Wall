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
            // チュートリアルは0番目なので無視
            for (var i = 1; i < clearedLevelIcons.Length; i++)
            {
                if (clearedLevels[i])
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