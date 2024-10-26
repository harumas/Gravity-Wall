using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Application.Sequence
{
    public class UISequencer : MonoBehaviour
    {
        [SerializeField] private float respawnDelay = 1.5f;
        [SerializeField] private float clearDelay = 3f;
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private GameObject clearPanel;

        public async UniTask SequenceRespawn()
        {
            loadingScreen.SetActive(true);
            await UniTask.Delay(TimeSpan.FromSeconds(respawnDelay));
            loadingScreen.SetActive(false);
        }
    }
}