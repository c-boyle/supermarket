using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private float speed = 1f;
    public void Move(Vector2 input)
    {
        var motion = new Vector3(input.x * speed, 0f, input.y * speed);
        controller.SimpleMove(motion);
        transform.forward = motion;
    }
}
