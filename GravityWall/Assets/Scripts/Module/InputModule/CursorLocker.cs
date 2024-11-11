using UnityEngine;

namespace Module.InputModule
{
    public class CursorLocker
    {
        public bool IsCursorChangeBlock { get; set; } = false;
        
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