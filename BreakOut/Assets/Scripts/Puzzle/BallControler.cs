using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallControler : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 1.0f;
    CharacterController controller = null;
    Vector2 currentDir = Vector2.zero;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        UpdateMovement();
    }

    void UpdateMovement()
    {
        Vector2 currentDir = new Vector2(Input.GetAxisRaw("BallHorizontal"), Input.GetAxisRaw("BallVertical"));
        currentDir.Normalize(); //żeby nie chodzil szybciej na skos

        Vector3 velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * walkSpeed;

        controller.Move(velocity);
    }
}
