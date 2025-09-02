using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;

namespace PleasantlyGames.RPG.Runtime.UnityExtension.Extensions
{
    public static class MouseButtonExtension
    {
        public static ButtonControl GetButtonControl(this MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return Mouse.current.leftButton;
                case MouseButton.Middle:
                    return Mouse.current.middleButton;
                case MouseButton.Right:
                    return Mouse.current.rightButton;
                case MouseButton.Forward:
                    return Mouse.current.forwardButton;
                case MouseButton.Back:
                    return Mouse.current.backButton;
                default:
                    return null;
            }
        }
    }
}