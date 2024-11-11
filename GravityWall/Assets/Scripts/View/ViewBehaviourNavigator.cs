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
        [SerializeField] private ViewBehaviourType initialViewBehaviour;

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

            if (initialViewBehaviour != ViewBehaviourType.None)
            {
                ActivateBehaviour(initialViewBehaviour);
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
            ActivateBehaviour(type);
            return activeBehaviours.Peek() as T;
        }

        public void ActivateBehaviour(ViewBehaviourType type)
        {
            ViewBehaviour behaviour = GetBehaviour(type);
            if (behaviour == null)
            {
                return;
            }

            ViewBehaviourType beforeType = ViewBehaviourType.None;

            if (activeBehaviours.TryPeek(out ViewBehaviour beforeBehaviour))
            {
                beforeBehaviour.Deactivate(type);
                beforeType = beforeBehaviour.ViewBehaviourType;
            }

            behaviour.Activate(beforeType);
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
            ViewBehaviourType nextType = ViewBehaviourType.None;

            if (activeBehaviours.TryPeek(out var beforeBehaviour))
            {
                nextType = beforeBehaviour.ViewBehaviourType;
            }

            behaviour.Deactivate(nextType);

            if (nextType != ViewBehaviourType.None)
            {
                beforeBehaviour.Activate(type);
            }
        }
    }
}