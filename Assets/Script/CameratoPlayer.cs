using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameratoPlayer : MonoBehaviour
{
    public Transform karakter;
    public Vector3 offset;

    void LateUpdate()
    {
        transform.position = new Vector3(karakter.position.x + offset.x, karakter.position.y + offset.y, offset.z);
    }
}
