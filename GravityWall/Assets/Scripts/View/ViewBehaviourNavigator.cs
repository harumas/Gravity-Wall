using System;
using System.Collections.Generic;
using R3;
using TriInspector;
using UnityEngine;

namespace View
{
    public enum ViewBehaviourState
    {
        None,
        Loading,
        Option,
        Clear,
        Pause,
        License,
        Title,
        Credit,
    }

    public class ViewBehaviourNavigator : MonoBehaviour
    {
        private Dictionary<ViewBehaviourState, ViewBehaviour> viewBehaviours;
        private Stack<ViewBehaviour> activeBehaviours;

        private readonly Subject<ViewBehaviourState> onStateChanged = new();
        public Observable<ViewBehaviourState> OnStateChanged => onStateChanged;

        public ViewBehaviourState CurrentBehaviourState
        {
            get
            {
                if (activeBehaviours.Count > 0)
                {
                    return activeBehaviours.Peek().ViewBehaviourState;
                }

                return ViewBehaviourState.None;
            }
        }

        public void RegisterBehaviours()
        {
            viewBehaviours = new Dictionary<ViewBehaviourState, ViewBehaviour>();
            activeBehaviours = new Stack<ViewBehaviour>();

            var behaviours = transform.GetComponentsInChildren<ViewBehaviour>(true);

            foreach (ViewBehaviour behaviour in behaviours)
            {
                viewBehaviours.Add(behaviour.ViewBehaviourState, behaviour);
            }
        }

        public T GetBehaviour<T>(ViewBehaviourState state) where T : ViewBehaviour
        {
            return GetBehaviour(state) as T;
        }

        public ViewBehaviour GetBehaviour(ViewBehaviourState state)
        {
            if (viewBehaviours.TryGetValue(state, out ViewBehaviour behaviour))
            {
                return behaviour;
            }

            Debug.LogError("指定されたBehaviourが存在しません。");
            return null;
        }

        public T ActivateBehaviour<T>(ViewBehaviourState state) where T : ViewBehaviour
        {
            ActivateBehaviour(state);
            return activeBehaviours.Peek() as T;
        }

        public void ActivateBehaviour(ViewBehaviourState state)
        {
            ViewBehaviour behaviour = GetBehaviour(state);
            
            if (behaviour == null)
            {
                return;
            }

            ViewBehaviourState beforeState = ViewBehaviourState.None;

            if (activeBehaviours.TryPeek(out ViewBehaviour beforeBehaviour))
            {
                beforeBehaviour.Deactivate(state);
                beforeState = beforeBehaviour.ViewBehaviourState;
            }

            behaviour.Activate(beforeState);
            onStateChanged.OnNext(state);
            activeBehaviours.Push(behaviour);
        }

        public void DeactivateBehaviour(ViewBehaviourState state)
        {
            if (activeBehaviours.Count == 0)
            {
                return;
            }
            
            ViewBehaviour behaviour = activeBehaviours.Peek();

            if (state != behaviour.ViewBehaviourState)
            {
                Debug.Log("手前に表示されているViewを閉じてから実行してください。");
                return;
            }

            activeBehaviours.Pop();
            ViewBehaviourState nextState = ViewBehaviourState.None;

            if (activeBehaviours.TryPeek(out var beforeBehaviour))
            {
                nextState = beforeBehaviour.ViewBehaviourState;
            }

            behaviour.Deactivate(nextState);

            if (nextState != ViewBehaviourState.None)
            {
                beforeBehaviour.Activate(state);
            }

            onStateChanged.OnNext(nextState);
        }
    }
}