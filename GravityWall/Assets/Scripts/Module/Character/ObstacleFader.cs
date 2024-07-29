using System;
using System.Collections.Generic;
using GravityWall;
using UnityEngine;
using UnityEngine.Rendering;

namespace Module.Character
{
    public class ObstacleFader : MonoBehaviour
    {
        [SerializeField] private Transform playerTarget;
        [SerializeField] private float detectRadius;

        private readonly RaycastHit[] resultsBuffer = new RaycastHit[8];
        private readonly List<GameObject> walls = new List<GameObject>();

        private void Update()
        {
            Vector3 diff = playerTarget.position - transform.position;

            float distance = diff.magnitude;
            Vector3 dir = diff / distance;
            int count = Physics.SphereCastNonAlloc(transform.position, detectRadius, dir, resultsBuffer, distance, Layer.Mask.Base);

            Span<bool> founds = stackalloc bool[count];

            for (int t = 0; t < walls.Count;)
            {
                bool found = false;
                GameObject wall = walls[t];

                for (int i = 0; i < count; i++)
                {
                    GameObject target = resultsBuffer[i].transform.gameObject;

                    if (wall == target)
                    {
                        found = true;
                        t++;
                        founds[i] = true;
                        break;
                    }
                }

                if (!found)
                {
                    walls[t].GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.On;
                    walls.RemoveAt(t);
                }
            }

            for (int i = 0; i < founds.Length; i++)
            {
                if (!founds[i])
                {
                    GameObject gameObj = resultsBuffer[i].transform.gameObject;

                    if (gameObj != null && gameObj.TryGetComponent(out Renderer meshRenderer))
                    {
                        meshRenderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                        walls.Add(gameObj);
                    }
                }
            }
        }
    }
}