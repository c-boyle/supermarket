using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    public void Move(Vector2 input)
    {
        var motion = new Vector3(input.x, 0f, input.y);
        controller.SimpleMove(motion);
        transform.forward = motion;
    }
}
