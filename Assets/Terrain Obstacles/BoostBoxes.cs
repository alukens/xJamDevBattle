using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostBoxes : MonoBehaviour
{
    public Vector3 force;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<Rigidbody>().velocity *= 1.05f;
            other.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);

        }
    }
}
