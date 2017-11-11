using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GameDevCommon.Input
{
    /// <summary>
    /// Handles keyboard input.
    /// </summary>
    public sealed class KeyboardHandler : IGameComponent
    {
        void IGameComponent.Initialize() { }

        private KeyboardState _oldState, _currentState;

        /// <summary>
        /// Updates the KeyboardHandler's states.
        /// </summary>
        internal void Update()
        {
            _oldState = _currentState;
            _currentState = Keyboard.GetState();
        }

        /// <summary>
        /// Returns if a specific key is pressed.
        /// </summary>
        public bool KeyPressed(Keys key)
            => (!_oldState.IsKeyDown(key) && _currentState.IsKeyDown(key));

        /// <summary>
        /// Returns if a specific key is being held down.
        /// </summary>
        public bool KeyDown(Keys key)
            => _currentState.IsKeyDown(key);

        /// <summary>
        /// Returns all keys that pressed right now.
        /// </summary>
        public Keys[] GetPressedKeys()
            => _currentState.GetPressedKeys();
    }
}
