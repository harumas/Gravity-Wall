using System;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

namespace View
{
    public enum BehaviourType
    {
        Loading,
        Option,
        Clear,
        Pause
    }

    public class ViewBehaviourNavigator : MonoBehaviour
    {
        private Dictionary<BehaviourType, ViewBehaviour> viewBehaviours;
        private Stack<ViewBehaviour> activeBehaviours;

        private void Awake()
        {
            viewBehaviours = new Dictionary<BehaviourType, ViewBehaviour>();
            activeBehaviours = new Stack<ViewBehaviour>();

            var behaviours = transform.GetComponentsInChildren<ViewBehaviour>(true);

            foreach (ViewBehaviour behaviour in behaviours)
            {
                viewBehaviours.Add(behaviour.BehaviourType, behaviour);
            }
        }


        public T ActivateBehaviour<T>(BehaviourType type) where T : ViewBehaviour
        {
            ActivateBehaviour(type);
            return viewBehaviours[type] as T;
        }

        public void ActivateBehaviour(BehaviourType type)
        {
            ViewBehaviour behaviour = viewBehaviours[type];
            behaviour.Activate();
            activeBehaviours.Push(behaviour);
        }

        public void DeactivateBehaviour(BehaviourType type)
        {
            ViewBehaviour behaviour = activeBehaviours.Peek();

            if (type != behaviour.BehaviourType)
            {
                Debug.Log("手前に表示されているViewを閉じてから実行してください。");
                return;
            }
            
            activeBehaviours.Pop();
            behaviour.Deactivate();
        }
    }
}