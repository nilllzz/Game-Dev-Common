using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace GameDevCommon.Input
{
    /// <summary>
    /// Handles GamePad input.
    /// </summary>
    public class GamePadHandler : IGameComponent
    {
        private readonly GamePadState[] _oldStates = new GamePadState[4];
        private readonly GamePadState[] _currentStates = new GamePadState[4];
        
        void IGameComponent.Initialize() { }

        /// <summary>
        /// Updates the GamePadHandler's states.
        /// </summary>
        internal void Update()
        {
            for (var i = 0; i < _oldStates.Length; i++)
            {
                _oldStates[i] = _currentStates[i];
            }

            _currentStates[0] = GamePad.GetState(PlayerIndex.One);
            _currentStates[1] = GamePad.GetState(PlayerIndex.Two);
            _currentStates[2] = GamePad.GetState(PlayerIndex.Three);
            _currentStates[3] = GamePad.GetState(PlayerIndex.Four);
        }

        /// <summary>
        /// Returns if a specific button on a GamePad is pressed.
        /// </summary>
        public bool ButtonPressed(PlayerIndex playerIndex, Buttons button)
        {
            var index = (int)playerIndex;
            return (!_oldStates[index].IsButtonDown(button) && _currentStates[index].IsButtonDown(button));
        }

        /// <summary>
        /// Returns is a button is currently being held down on a GamePad.
        /// </summary>
        public bool ButtonDown(PlayerIndex playerIndex, Buttons button)
        {
            var index = (int)playerIndex;
            return _currentStates[index].IsButtonDown(button);
        }

        /// <summary>
        /// Returns if the GamePad at the given player index is connected.
        /// </summary>
        public bool IsConnected(PlayerIndex playerIndex)
        {
            var index = (int)playerIndex;
            return _currentStates[index].IsConnected;
        }

        /// <summary>
        /// Returns a value from 0 to 1 how much a thumbstick is pressed in one direction.
        /// </summary>
        public float GetThumbStickDirection(PlayerIndex playerIndex, ThumbStick thumbStick, InputDirection direction)
        {
            Vector2 v;
            var result = 0f;
            var index = (int)playerIndex;

            if (thumbStick == ThumbStick.Left)
                v = _currentStates[index].ThumbSticks.Left;
            else
                v = _currentStates[index].ThumbSticks.Right;

            switch (direction)
            {
                case InputDirection.Up:
                    result = v.Y;
                    break;
                case InputDirection.Left:
                    result = v.X * -1f;
                    break;
                case InputDirection.Down:
                    result = v.Y * -1f;
                    break;
                case InputDirection.Right:
                    result = v.X;
                    break;
            }

            if (result < 0f)
                result = 0f;

            return result;
        }

        public float GetLeftTrigger(PlayerIndex playerIndex)
        {
            var index = (int)playerIndex;
            return _currentStates[index].Triggers.Left;
        }

        public float GetRightTrigger(PlayerIndex playerIndex)
        {
            var index = (int)playerIndex;
            return _currentStates[index].Triggers.Right;
        }

        public Vector2 GetLeftStick(PlayerIndex playerIndex)
        {
            var index = (int)playerIndex;
            return _currentStates[index].ThumbSticks.Left;
        }

        public Vector2 GetRightStick(PlayerIndex playerIndex)
        {
            var index = (int)playerIndex;
            return _currentStates[index].ThumbSticks.Right;
        }
    }
}
