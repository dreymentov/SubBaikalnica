using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckGround : MonoBehaviour
{
    public PlayerControlles playerControlles;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == playerControlles.gameObject)
        {
            return;
        }

        if (other.gameObject.CompareTag("Ground"))
        {
            playerControlles.SetGrounded(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == playerControlles.gameObject)
        {
            return;
        }

        playerControlles.SetGrounded(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == playerControlles.gameObject)
        {
            return;
        }

        if (other.gameObject.CompareTag("Ground"))
        {
            playerControlles.SetGrounded(true);
        }
    }
}
