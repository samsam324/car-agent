using UnityEngine;

public class Sphere : MonoBehaviour
{
    [SerializeField]
    private GameObject car;

    private void OnTriggerEnter(Collider other)
    {
        car.transform.GetComponent<CarAgent>().CheckPointTriggered(other);
    }
}
