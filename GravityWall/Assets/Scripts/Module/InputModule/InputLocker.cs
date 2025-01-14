using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;

namespace Module.InputModule
{
    public class InputLocker
    {
        public bool IsLocked { get; private set; } = false;

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
                    IsLocked = true;
                }
            }).AddTo(cancellationToken);
            
            UpdateLockState();
        }
        
        private void UpdateLockState()
        {
            IsLocked = conditions.All(property => !property.CurrentValue);
        }
    }
}