using UnityEngine;

namespace Module.InputModule
{
    public class CursorLocker
    {
        /// <summary>
        /// カーソルのロック変更を無効化するか
        /// </summary>
        public bool IsCursorChangeBlock { get; set; } = false;
        
        /// <summary>
        /// カーソルロックを変更します
        /// </summary>
        /// <param name="isLock"></param>
        public void SetCursorLock(bool isLock)
        {
            if (IsCursorChangeBlock)
            {
                return;
            }
            
            Cursor.visible = !isLock;
            Cursor.lockState = isLock ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }
}