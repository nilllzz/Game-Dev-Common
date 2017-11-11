using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace GameDevCommon.Input
{
    /// <summary>
    /// A class to handle all input methods.
    /// </summary>
    public sealed class ControlsHandler : IGameComponent
    {
        void IGameComponent.Initialize() { }

        /// <summary>
        /// Updates all input handlers.
        /// </summary>
        public void Update()
        {
            GameInstanceProvider.IGame.GetComponentManager().GetComponent<KeyboardHandler>().Update();
            GameInstanceProvider.IGame.GetComponentManager().GetComponent<MouseHandler>().Update();
            GameInstanceProvider.IGame.GetComponentManager().GetComponent<GamePadHandler>().Update();
        }

        /*
        This keeps track of the direction that was last pressed.
        That is important since we want to have a feature where you can hold down a direction,
        and it starts to "press" that direction button at an interval (once per 3 frames).
        The pressedKeyDelay is set to wait for 40 frames until the 3 frame interval starts.
        */

        //We have 4 element-long arrays because there can be four players connected. We initialize them with the standard values.
        private readonly InputDirection[] _lastPressedDirection = { InputDirection.None, InputDirection.None, InputDirection.None, InputDirection.None };
        private readonly float[] _pressedKeyDelay = { 0f, 0f, 0f, 0f };

        /// <summary>
        /// Resets when no direction is pressed.
        /// </summary>
        private void ResetDirectionPressed(PlayerIndex playerIndex, InputDirection direction)
        {
            var i = (int)playerIndex;
            if (direction == _lastPressedDirection[i])
            {
                _pressedKeyDelay[i] = 4f;
                _lastPressedDirection[i] = InputDirection.None;
            }
        }

        /// <summary>
        /// When a different direction is pressed, then reset the wait time to 40 frames.
        /// </summary>
        private void ChangeDirectionPressed(PlayerIndex playerIndex, InputDirection direction)
        {
            var i = (int)playerIndex;
            if (_lastPressedDirection[i] != direction)
            {
                _pressedKeyDelay[i] = 4f;
                _lastPressedDirection[i] = direction;
            }
        }

        /// <summary>
        /// Checks, if the hold-down-pressed feature is active and updates it.
        /// </summary>
        private bool HoldDownPressed(PlayerIndex playerIndex, InputDirection direction)
        {
            var i = (int)playerIndex;
            if (_lastPressedDirection[i] == direction)
            {
                _pressedKeyDelay[i] -= 0.1f;
                if (_pressedKeyDelay[i] <= 0f)
                {
                    _pressedKeyDelay[i] = 0.3f;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if a direction is either pressed or down.
        /// </summary>
        private bool CheckDirectional(bool pressed, PlayerIndex playerIndex, InputDirection direction, Keys WASDKey, Keys arrowKey, Buttons leftThumbStickDirection, Buttons rightThumbStickDirection, Buttons dPadDirection, InputDirectionType[] inputTypes)
        {
            if (pressed)
                return CheckDirectionalPress(playerIndex, direction, WASDKey, arrowKey, leftThumbStickDirection, rightThumbStickDirection, dPadDirection, inputTypes);
            else
                return CheckDirectionalDown(playerIndex, direction, WASDKey, arrowKey, leftThumbStickDirection, rightThumbStickDirection, dPadDirection, inputTypes);
        }

        /// <summary>
        /// Checks if any of the given directions are pressed.
        /// </summary>
        /// <param name="inputTypes">All input types to check.</param>
        private bool CheckDirectionalPress(PlayerIndex playerIndex, InputDirection direction, Keys WASDKey, Keys arrowKey, Buttons leftThumbStickDirection, Buttons rightThumbStickDirection, Buttons dPadDirection, InputDirectionType[] inputTypes)
        {
            //This keeps track if any direction key has been pressed. If not, it will reset the delay at the end.
            var hasInputDirection = false;

            var checkForAll = inputTypes.Contains(InputDirectionType.All);

            //The keyboard is always assigned to player one.
            //This might be changed later, if it does, we need to replace the PlayerIndex.One to the designated player.

            //Check for WASD keys.
            if ((inputTypes.Contains(InputDirectionType.WASD) || checkForAll) && playerIndex == PlayerIndex.One)
            {
                if (GameInstanceProvider.IGame.GetComponentManager().GetComponent<KeyboardHandler>().KeyDown(WASDKey))
                {
                    hasInputDirection = true;
                    if (HoldDownPressed(PlayerIndex.One, direction))
                    {
                        return true;
                    }
                    else
                    {
                        if (GameInstanceProvider.IGame.GetComponentManager().GetComponent<KeyboardHandler>().KeyPressed(WASDKey))
                        {
                            ChangeDirectionPressed(PlayerIndex.One, direction);
                            return true;
                        }
                    }
                }
            }

            //Check for arrow keys.
            if ((inputTypes.Contains(InputDirectionType.ArrowKeys) || checkForAll) && playerIndex == PlayerIndex.One)
            {
                if (GameInstanceProvider.IGame.GetComponentManager().GetComponent<KeyboardHandler>().KeyDown(arrowKey))
                {
                    hasInputDirection = true;
                    if (HoldDownPressed(PlayerIndex.One, direction))
                    {
                        return true;
                    }
                    else
                    {
                        if (GameInstanceProvider.IGame.GetComponentManager().GetComponent<KeyboardHandler>().KeyPressed(arrowKey))
                        {
                            ChangeDirectionPressed(PlayerIndex.One, direction);
                            return true;
                        }
                    }
                }
            }

            // Check for the left thumbstick.
            if (inputTypes.Contains(InputDirectionType.LeftThumbStick) || checkForAll)
            {
                if (GameInstanceProvider.IGame.GetComponentManager().GetComponent<GamePadHandler>().ButtonDown(playerIndex, leftThumbStickDirection))
                {
                    hasInputDirection = true;
                    if (HoldDownPressed(playerIndex, direction))
                    {
                        return true;
                    }
                    else
                    {
                        if (GameInstanceProvider.IGame.GetComponentManager().GetComponent<GamePadHandler>().ButtonPressed(playerIndex, leftThumbStickDirection))
                        {
                            ChangeDirectionPressed(playerIndex, direction);
                            return true;
                        }
                    }
                }
            }

            // Check for the right thumbstick.
            if (inputTypes.Contains(InputDirectionType.RightThumbStick) || checkForAll)
            {
                if (GameInstanceProvider.IGame.GetComponentManager().GetComponent<GamePadHandler>().ButtonDown(playerIndex, rightThumbStickDirection))
                {
                    hasInputDirection = true;
                    if (HoldDownPressed(playerIndex, direction))
                    {
                        return true;
                    }
                    else
                    {
                        if (GameInstanceProvider.IGame.GetComponentManager().GetComponent<GamePadHandler>().ButtonPressed(playerIndex, rightThumbStickDirection))
                        {
                            ChangeDirectionPressed(playerIndex, direction);
                            return true;
                        }
                    }
                }
            }

            //Check for the dpad.
            if (inputTypes.Contains(InputDirectionType.DPad) || checkForAll)
            {
                if (GameInstanceProvider.IGame.GetComponentManager().GetComponent<GamePadHandler>().ButtonDown(playerIndex, dPadDirection))
                {
                    hasInputDirection = true;
                    if (HoldDownPressed(playerIndex, direction))
                    {
                        return true;
                    }
                    else
                    {
                        if (GameInstanceProvider.IGame.GetComponentManager().GetComponent<GamePadHandler>().ButtonPressed(playerIndex, dPadDirection))
                        {
                            ChangeDirectionPressed(playerIndex, direction);
                            return true;
                        }
                    }
                }
            }

            //When no direction button was pressed, reset and return false.
            if (!hasInputDirection)
            {
                ResetDirectionPressed(playerIndex, direction);
            }

            return false;
        }

        /// <summary>
        /// Checks if any of the given directions are down.
        /// </summary>
        /// <param name="inputTypes">All input types to check.</param>
        private bool CheckDirectionalDown(PlayerIndex playerIndex, InputDirection direction, Keys WASDKey, Keys arrowKey, Buttons leftThumbStickDirection, Buttons rightThumbStickDirection, Buttons dPadDirection, InputDirectionType[] inputTypes)
        {
            //When we should check for any input type, check if the handler's inputDown is pressed.

            var checkForAll = inputTypes.Contains(InputDirectionType.All);

            if ((inputTypes.Contains(InputDirectionType.WASD) || checkForAll) && playerIndex == PlayerIndex.One)
            {
                if (GameInstanceProvider.IGame.GetComponentManager().GetComponent<KeyboardHandler>().KeyDown(WASDKey))
                {
                    return true;
                }
            }
            if ((inputTypes.Contains(InputDirectionType.ArrowKeys) || checkForAll) && playerIndex == PlayerIndex.One)
            {
                if (GameInstanceProvider.IGame.GetComponentManager().GetComponent<KeyboardHandler>().KeyDown(arrowKey))
                {
                    return true;
                }
            }
            if (inputTypes.Contains(InputDirectionType.LeftThumbStick) || checkForAll)
            {
                if (GameInstanceProvider.IGame.GetComponentManager().GetComponent<GamePadHandler>().ButtonDown(playerIndex, leftThumbStickDirection))
                {
                    return true;
                }
            }
            if (inputTypes.Contains(InputDirectionType.RightThumbStick) || checkForAll)
            {
                if (GameInstanceProvider.IGame.GetComponentManager().GetComponent<GamePadHandler>().ButtonDown(playerIndex, rightThumbStickDirection))
                {
                    return true;
                }
            }
            if (inputTypes.Contains(InputDirectionType.DPad) || checkForAll)
            {
                if (GameInstanceProvider.IGame.GetComponentManager().GetComponent<GamePadHandler>().ButtonDown(playerIndex, dPadDirection))
                {
                    return true;
                }
            }

            //No handler has the button down? return false.
            return false;
        }

        #region Internal direction checking

        //These methods check the four directions:

        private bool InternalLeft(bool pressed, PlayerIndex playerIndex, InputDirectionType[] inputTypes)
        {
            return CheckDirectional(pressed, playerIndex, InputDirection.Left, Keys.A, Keys.Left, Buttons.LeftThumbstickLeft, Buttons.RightThumbstickLeft, Buttons.DPadLeft, inputTypes);
        }

        private bool InternalRight(bool pressed, PlayerIndex playerIndex, InputDirectionType[] inputTypes)
        {
            return CheckDirectional(pressed, playerIndex, InputDirection.Right, Keys.D, Keys.Right, Buttons.LeftThumbstickRight, Buttons.RightThumbstickRight, Buttons.DPadRight, inputTypes);
        }

        private bool InternalUp(bool pressed, PlayerIndex playerIndex, InputDirectionType[] inputTypes)
        {
            return CheckDirectional(pressed, playerIndex, InputDirection.Up, Keys.W, Keys.Up, Buttons.LeftThumbstickUp, Buttons.RightThumbstickUp, Buttons.DPadUp, inputTypes);
        }

        private bool InternalDown(bool pressed, PlayerIndex playerIndex, InputDirectionType[] inputTypes)
        {
            return CheckDirectional(pressed, playerIndex, InputDirection.Down, Keys.S, Keys.Down, Buttons.LeftThumbstickDown, Buttons.RightThumbstickDown, Buttons.DPadDown, inputTypes);
        }

        #endregion

        #region public interface

        //These methods are the public access to the internal check methods:

        /// <summary>
        /// Checks if up left are down.
        /// </summary>
        public bool LeftDown(PlayerIndex playerIndex = PlayerIndex.One)
        {
            return LeftDown(playerIndex, new InputDirectionType[] { InputDirectionType.All });
        }

        /// <summary>
        /// Checks if left controls are down.
        /// </summary>
        /// <param name="inputTypes">The input types to check.</param>
        public bool LeftDown(PlayerIndex playerIndex = PlayerIndex.One, params InputDirectionType[] inputTypes)
        {
            return InternalLeft(false, playerIndex, inputTypes);
        }

        /// <summary>
        /// Checks if up left are pressed.
        /// </summary>
        public bool LeftPressed(PlayerIndex playerIndex = PlayerIndex.One)
        {
            return LeftPressed(playerIndex, new InputDirectionType[] { InputDirectionType.All });
        }

        /// <summary>
        /// Checks if left controls are pressed.
        /// </summary>
        /// <param name="inputTypes">The input types to check.</param>
        public bool LeftPressed(PlayerIndex playerIndex = PlayerIndex.One, params InputDirectionType[] inputTypes)
        {
            return InternalLeft(true, playerIndex, inputTypes);
        }

        /// <summary>
        /// Checks if right controls are down.
        /// </summary>
        public bool RightDown(PlayerIndex playerIndex = PlayerIndex.One)
        {
            return RightDown(playerIndex, new InputDirectionType[] { InputDirectionType.All });
        }

        /// <summary>
        /// Checks if right controls are down.
        /// </summary>
        /// <param name="inputTypes">The input types to check.</param>
        public bool RightDown(PlayerIndex playerIndex = PlayerIndex.One, params InputDirectionType[] inputTypes)
        {
            return InternalRight(false, playerIndex, inputTypes);
        }

        /// <summary>
        /// Checks if up controls are pressed.
        /// </summary>
        public bool RightPressed(PlayerIndex playerIndex = PlayerIndex.One)
        {
            return RightPressed(playerIndex, new InputDirectionType[] { InputDirectionType.All });
        }

        /// <summary>
        /// Checks if right controls are pressed.
        /// </summary>
        /// <param name="inputTypes">The input types to check.</param>
        public bool RightPressed(PlayerIndex playerIndex = PlayerIndex.One, params InputDirectionType[] inputTypes)
        {
            return InternalRight(true, playerIndex, inputTypes);
        }

        /// <summary>
        /// Checks if up controls are down.
        /// </summary>
        public bool UpDown(PlayerIndex playerIndex = PlayerIndex.One)
        {
            return UpDown(playerIndex, new InputDirectionType[] { InputDirectionType.All });
        }

        /// <summary>
        /// Checks if up controls are down.
        /// </summary>
        /// <param name="inputTypes">The input types to check.</param>
        public bool UpDown(PlayerIndex playerIndex = PlayerIndex.One, params InputDirectionType[] inputTypes)
        {
            return InternalUp(false, playerIndex, inputTypes);
        }

        /// <summary>
        /// Checks if up controls are pressed.
        /// </summary>
        public bool UpPressed(PlayerIndex playerIndex = PlayerIndex.One)
        {
            return UpPressed(playerIndex, new InputDirectionType[] { InputDirectionType.All });
        }

        /// <summary>
        /// Checks if up controls are pressed.
        /// </summary>
        /// <param name="inputTypes">The input types to check.</param>
        public bool UpPressed(PlayerIndex playerIndex = PlayerIndex.One, params InputDirectionType[] inputTypes)
        {
            return InternalUp(true, playerIndex, inputTypes);
        }

        /// <summary>
        /// Checks if down controls are down.
        /// </summary>
        public bool DownDown(PlayerIndex playerIndex = PlayerIndex.One)
        {
            return DownDown(playerIndex, new InputDirectionType[] { InputDirectionType.All });
        }

        /// <summary>
        /// Checks if down controls are down.
        /// </summary>
        /// <param name="inputTypes">The input types to check.</param>
        public bool DownDown(PlayerIndex playerIndex = PlayerIndex.One, params InputDirectionType[] inputTypes)
        {
            return InternalDown(false, playerIndex, inputTypes);
        }

        /// <summary>
        /// Checks if down controls are pressed.
        /// </summary>
        public bool DownPressed(PlayerIndex playerIndex = PlayerIndex.One)
        {
            return DownPressed(playerIndex, new InputDirectionType[] { InputDirectionType.All });
        }

        /// <summary>
        /// Checks if down controls are pressed.
        /// </summary>
        /// <param name="inputTypes">The input types to check.</param>
        public bool DownPressed(PlayerIndex playerIndex = PlayerIndex.One, params InputDirectionType[] inputTypes)
        {
            return InternalDown(true, playerIndex, inputTypes);
        }

        #endregion
    }
}
