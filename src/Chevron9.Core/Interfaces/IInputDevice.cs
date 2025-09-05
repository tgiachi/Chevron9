using Chevron9.Core.Data.Input;
using Chevron9.Core.Types;
using Chevron9.Shared.Primitives;

namespace Chevron9.Core.Interfaces;

/// <summary>
///     Interface for input device handling keyboard, mouse, and modifier keys
///     Provides frame-based input polling for game/interactive applications
/// </summary>
public interface IInputDevice
{
    /// <summary>
    ///     Updates input state for current frame - call once per frame before checking input
    /// </summary>
    void Poll();

    /// <summary>
    ///     Checks if a key is currently pressed down (held)
    /// </summary>
    /// <param name="key">The key to check</param>
    /// <returns>True if key is currently down</returns>
    bool IsDown(InputKey key);

    /// <summary>
    ///     Checks if a key was pressed this frame (transition from up to down)
    /// </summary>
    /// <param name="key">The key to check</param>
    /// <returns>True if key was pressed this frame</returns>
    bool WasPressed(InputKey key);

    /// <summary>
    ///     Checks if a key was released this frame (transition from down to up)
    /// </summary>
    /// <param name="key">The key to check</param>
    /// <returns>True if key was released this frame</returns>
    bool WasReleased(InputKey key);

    /// <summary>
    ///     Checks if a key is currently pressed down with specific modifiers
    /// </summary>
    /// <param name="key">The key to check</param>
    /// <param name="modifiers">Required modifier keys (Ctrl, Shift, Alt, etc.)</param>
    /// <returns>True if key and all specified modifiers are down</returns>
    bool IsDown(InputKey key, InputKeyModifierType modifiers);

    /// <summary>
    ///     Checks if a key with modifiers was pressed this frame
    /// </summary>
    /// <param name="key">The key to check</param>
    /// <param name="modifiers">Required modifier keys</param>
    /// <returns>True if key combo was pressed this frame</returns>
    bool WasPressed(InputKey key, InputKeyModifierType modifiers);

    /// <summary>
    ///     Checks if a key with modifiers was released this frame
    /// </summary>
    /// <param name="key">The key to check</param>
    /// <param name="modifiers">Required modifier keys</param>
    /// <returns>True if key combo was released this frame</returns>
    bool WasReleased(InputKey key, InputKeyModifierType modifiers);

    /// <summary>
    ///     Gets currently active modifier keys (Ctrl, Shift, Alt, Super)
    /// </summary>
    /// <returns>Bitfield of active modifiers</returns>
    InputKeyModifierType GetActiveModifiers();

    /// <summary>
    ///     Gets current mouse cursor position in screen/window coordinates
    /// </summary>
    /// <returns>Mouse position (coordinate system depends on renderer)</returns>
    Position MousePosition();

    /// <summary>
    ///     Checks if a mouse button is currently pressed down
    /// </summary>
    /// <param name="button">Mouse button to check</param>
    /// <returns>True if button is currently down</returns>
    bool MouseDown(MouseButtonType button);

    /// <summary>
    ///     Checks if a mouse button was clicked this frame (pressed and released)
    /// </summary>
    /// <param name="button">Mouse button to check</param>
    /// <returns>True if button was clicked this frame</returns>
    bool MouseClicked(MouseButtonType button);

    /// <summary>
    ///     Gets mouse wheel movement delta for current frame
    /// </summary>
    /// <returns>Wheel delta (X=horizontal, Y=vertical scroll)</returns>
    Position MouseWheelDelta();

    /// <summary>
    ///     Gets mouse movement delta since last Poll() call
    /// </summary>
    /// <returns>Mouse delta (X=horizontal, Y=vertical movement)</returns>
    Position MouseDelta();

    /// <summary>
    ///     Checks if a mouse button was pressed this frame (transition from up to down)
    /// </summary>
    /// <param name="button">Mouse button to check</param>
    /// <returns>True if button was pressed this frame</returns>
    bool MousePressed(MouseButtonType button);

    /// <summary>
    ///     Checks if a mouse button was released this frame (transition from down to up)
    /// </summary>
    /// <param name="button">Mouse button to check</param>
    /// <returns>True if button was released this frame</returns>
    bool MouseReleased(MouseButtonType button);

    /// <summary>
    ///     Checks if the mouse pointer is currently inside the application window
    /// </summary>
    /// <returns>True if mouse pointer is inside the window</returns>
    bool IsPointerInside();

    /// <summary>
    ///     Checks if the application window currently has focus
    /// </summary>
    /// <returns>True if application has focus</returns>
    bool HasFocus();
}
