using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject car;

    void Start()
    {
        for (int i = 0; i < 50; i++)
        {
            Instantiate(car, Vector3.right * (2.7f + (Random.value * (8.0f - 2.7f))) + Vector3.up * 0.55f + Vector3.forward * (20.0f + (Random.value * (37.0f - 20.0f))), Quaternion.identity);
        }
    }
}
