using UnityEngine;

namespace Vymesy.Player
{
    /// <summary>
    /// Abstract source of a 2D normalised vector — used by <see cref="PlayerController"/> to
    /// merge virtual-joystick input with keyboard input. Implementations: <see cref="TouchJoystick"/>.
    /// </summary>
    public interface ITouchInputSource
    {
        Vector2 Read();
    }
}
