using System;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

namespace View
{
    public enum ViewBehaviourType
    {
        None,
        Loading,
        Option,
        Clear,
        Pause,
        License,
        Title,
    }

    public class ViewBehaviourNavigator : MonoBehaviour
    {
        private Dictionary<ViewBehaviourType, ViewBehaviour> viewBehaviours;
        private Stack<ViewBehaviour> activeBehaviours;

        public ViewBehaviourType CurrentBehaviourType
        {
            get
            {
                if (activeBehaviours.Count > 0)
                {
                    return activeBehaviours.Peek().ViewBehaviourType;
                }

                return ViewBehaviourType.None;
            }
        }

        public void RegisterBehaviours()
        {
            viewBehaviours = new Dictionary<ViewBehaviourType, ViewBehaviour>();
            activeBehaviours = new Stack<ViewBehaviour>();

            var behaviours = transform.GetComponentsInChildren<ViewBehaviour>(true);

            foreach (ViewBehaviour behaviour in behaviours)
            {
                viewBehaviours.Add(behaviour.ViewBehaviourType, behaviour);
            }
        }

        public T GetBehaviour<T>(ViewBehaviourType type) where T : ViewBehaviour
        {
            return GetBehaviour(type) as T;
        }

        public ViewBehaviour GetBehaviour(ViewBehaviourType type)
        {
            if (viewBehaviours.TryGetValue(type, out ViewBehaviour behaviour))
            {
                return behaviour;
            }

            Debug.LogError("指定されたBehaviourが存在しません。");
            return null;
        }

        public T ActivateBehaviour<T>(ViewBehaviourType type) where T : ViewBehaviour
        {
            ViewBehaviour behaviour = GetBehaviour(type);
            if (behaviour == null)
            {
                return null;
            }

            behaviour.Activate();
            activeBehaviours.Push(behaviour);

            return behaviour as T;
        }

        public void ActivateBehaviour(ViewBehaviourType type)
        {
            ViewBehaviour behaviour = GetBehaviour(type);
            if (behaviour == null)
            {
                return;
            }

            behaviour.Activate();
            activeBehaviours.Push(behaviour);
        }

        public void DeactivateBehaviour(ViewBehaviourType type)
        {
            ViewBehaviour behaviour = activeBehaviours.Peek();

            if (type != behaviour.ViewBehaviourType)
            {
                Debug.Log("手前に表示されているViewを閉じてから実行してください。");
                return;
            }

            activeBehaviours.Pop();
            behaviour.Deactivate();
        }
    }
}