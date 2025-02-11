using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NotMov : MonoBehaviour
{
    public UnityEvent Starts;
    public UnityEvent Stops;

    public void CanStart()
    {
        Starts?.Invoke();
    }
    public void CantStart()
    {
        Stops?.Invoke();
    }
}
