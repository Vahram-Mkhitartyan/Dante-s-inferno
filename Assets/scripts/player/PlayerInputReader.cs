using UnityEngine;

public class PlayerInputReader
{
    public PlayerInputState Read()
    {
        return new PlayerInputState
        {
            MoveX = Input.GetAxisRaw("Horizontal"),
            JumpPressed = Input.GetKeyDown(KeyCode.W),
            DefendHeld = Input.GetKey(KeyCode.LeftShift)
        };
    }
}
