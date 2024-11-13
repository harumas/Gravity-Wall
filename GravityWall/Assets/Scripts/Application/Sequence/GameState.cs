using R3;

namespace Application.Sequence
{
    public class GameState
    {
        public enum State
        {
            StageSelect,
            Playing
        }

        private readonly ReactiveProperty<State> current = new();
        public ReadOnlyReactiveProperty<State> Current => current;

        public void SetState(State state)
        {
            current.Value = state;
        }
    }
}