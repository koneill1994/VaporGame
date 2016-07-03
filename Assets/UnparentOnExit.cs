using UnityEngine;
using System.Collections;

public class UnparentOnExit : MonoBehaviour {
    void OnTriggerExit(Collider other)
    {
        // Destroy everything that leaves the trigger
        //Destroy(other.gameObject);
        other.gameObject.transform.SetParent(null);
    }

}
