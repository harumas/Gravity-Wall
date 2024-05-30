using System;
using System.Collections.Generic;
using GravityWall;
using UnityEngine;

namespace Module.Character
{
    public class ObstacleFader : MonoBehaviour
    {
        [SerializeField] private Transform playerTarget;
        [SerializeField] private float detectRadius;

        private RaycastHit[] resultsBuffer = new RaycastHit[8];
        private List<GameObject> walls = new List<GameObject>();

        private void Update()
        {
            Vector3 diff = playerTarget.position - transform.position;

            float distance = diff.magnitude;
            Vector3 dir = diff / distance;
            int count = Physics.SphereCastNonAlloc(transform.position, detectRadius, dir, resultsBuffer, distance, Layer.Mask.Gravity);

            Span<bool> founds = stackalloc bool[count];

            for (var t = 0; t < walls.Count;)
            {
                bool found = false;
                var wall = walls[t];

                for (int i = 0; i < count; i++)
                {
                    GameObject target = resultsBuffer[i].transform.gameObject;

                    if (!target.CompareTag("Wall"))
                    {
                        return;
                    }

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
                    walls[t].GetComponent<Renderer>().enabled = true;
                    walls.RemoveAt(t);
                }
            }


            for (int i = 0; i < founds.Length; i++)
            {
                if (!founds[i])
                {
                    GameObject gameObj = resultsBuffer[i].transform.gameObject;
                    
                    if (!gameObj.CompareTag("Wall"))
                    {
                        return;
                    }


                    gameObj.GetComponent<Renderer>().enabled = false;
                    walls.Add(gameObj);
                }
            }
        }
    }
}