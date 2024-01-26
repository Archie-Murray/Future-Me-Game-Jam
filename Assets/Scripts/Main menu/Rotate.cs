using System.Collections;
using System.Collections.Generic;
using System.Threading;

using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] float rotateSpeed =2;
    Quaternion curentRotation ;
    float timeCount = 0.0f;
     float spin;
    // Start is called before the first frame update
    void Start()
    {
        spin = curentRotation.y;
    }

    // Update is called once per frame
    void Update()
    {
        spin = curentRotation.y;
        transform.rotation = new Quaternion(0, spin + rotateSpeed,0,0);
    }
}
