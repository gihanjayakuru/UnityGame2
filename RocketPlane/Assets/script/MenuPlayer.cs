using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPlayer : MonoBehaviour
{
    private void Update()
    {
        transform.position += Vector3.forward * 6 * Time.deltaTime;
    }

}
