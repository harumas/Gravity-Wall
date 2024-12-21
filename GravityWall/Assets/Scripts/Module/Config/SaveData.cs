using System.Collections.Generic;
using CoreModule.Save;
using UnityEngine;

namespace Module.Config
{
    public class SaveData : ICloneable<SaveData>
    {
        private const int MaxStageCount = 16;
        public bool[] ClearedStageList = new bool[16];

        public SaveData Clone()
        {
            return new SaveData()
            {
                ClearedStageList = ClearedStageList.Clone() as bool[]
            };
        }
    }
}