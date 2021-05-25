using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnimalManagement;

public class AnimationsHandler : MonoBehaviour
{
    public void Die()
    {
        transform.parent.GetComponent<Animal>().Die();
    }
}
