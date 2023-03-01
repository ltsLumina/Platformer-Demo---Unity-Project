using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sawblade : MonoBehaviour
{
    void Update() => transform.Rotate(new Vector3(0, 0, -200) * Time.deltaTime);
}