using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jongkok : MonoBehaviour
{
    public BoxCollider2D HeadCollider;
    public BoxCollider2D Atas;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Structure") && Atas != null)
        {
            Atas.enabled = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Structure") && Atas != null)
        {
            Atas.enabled = true;
        }
    }
}
