using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace Module.InputModule
{
    public class InputLocker
    {
        public ReadOnlyReactiveProperty<bool> IsLocked => isLocked;
        private ReactiveProperty<bool> isLocked = new(false);

        private readonly List<ReadOnlyReactiveProperty<bool>> conditions = new();

        public void AddCondition(ReadOnlyReactiveProperty<bool> condition, CancellationToken cancellationToken)
        {
            conditions.Add(condition);

            condition.Subscribe(value =>
            {
                if (value)
                {
                    UpdateLockState();
                }
                else
                {
                    isLocked.Value = true;
                }
            }).AddTo(cancellationToken);
            
            UpdateLockState();
        }
        
        private void UpdateLockState()
        {
            isLocked.Value = conditions.Any(property => !property.CurrentValue);
        }
    }
}