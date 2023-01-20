using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pivot : MonoBehaviour
{
    [SerializeField] GameObject player;

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;

        difference.Normalize();

        float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);

        transform.localRotation = rotationZ switch // switch expression (expression statement)
        {
            < -90 => player.transform.eulerAngles.y switch
            {
                0   => Quaternion.Euler(180, 0, -rotationZ),
                180 => Quaternion.Euler(180, 180, -rotationZ),
                _   => transform.localRotation
            },
            > 90 => player.transform.eulerAngles.y switch
            {
                0   => Quaternion.Euler(180, 0, -rotationZ),
                180 => Quaternion.Euler(180, 180, -rotationZ),
                _   => transform.localRotation
            },
            _ => transform.localRotation
        };
    }
}