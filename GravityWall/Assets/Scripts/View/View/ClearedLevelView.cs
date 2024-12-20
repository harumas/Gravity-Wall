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