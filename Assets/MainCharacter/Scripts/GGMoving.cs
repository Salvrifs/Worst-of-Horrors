using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GGMoving : MonoBehaviour
{
    public float move_speed = 5.0f;
    public int nitro = 2;
    public float max_endurance = 100f;
    public float endurance = 100f;
    public float energy_speed = 10f;
    private bool flag = true;

    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float horizontal_input = Input.GetAxis("Horizontal");
        float vertical_input = Input.GetAxis("Vertical");


        Vector3 move_direction = transform.forward * vertical_input + transform.right * horizontal_input;

        move_direction.y -= 9000.81f * Time.deltaTime;
        if (Input.GetKey(KeyCode.LeftShift) && (endurance > 0f) && ((move_direction.x != 0) || (move_direction.z != 0)))
        {
            controller.Move(move_direction * move_speed * Time.deltaTime * nitro);
            endurance -= energy_speed * Time.deltaTime;
            if (endurance <= 0)
                flag = false;
        }
        else
        {
            if (!flag)
            {
                endurance = -50f;
                flag = true;
            }
            if (endurance < max_endurance)
                endurance += energy_speed * Time.deltaTime;
            if (endurance < 0f)
                controller.Move(move_direction * move_speed * Time.deltaTime / nitro);
            else
                controller.Move(move_direction * move_speed * Time.deltaTime);
        }
    }
}
