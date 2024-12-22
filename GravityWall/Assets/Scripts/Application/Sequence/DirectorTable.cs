using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Application.Sequence
{
    [Serializable]
    public struct PlayableDirectorData 
    {
        [SerializeField] private string key;
        [SerializeField] private PlayableDirector director;

        public string Key => key;
        public PlayableDirector Director => director;
    }
    
    [Serializable]
    public class DirectorTable
    {
        [SerializeField] private List<PlayableDirectorData> directorTable;
        
        public PlayableDirector GetDirector(string key)
        {
            foreach (var data in directorTable)
            {
                if (data.Key == key)
                {
                    return data.Director;
                }
            }

            return null;
        }
    }
}