using UnityEngine;

public class ShakeManager : MonoBehaviour
{
    public void ShakeCapsule(GameObject capsule)
    {
        int randomRotation = Random.Range(0, 2) == 0 ? Random.Range(-98, -93) : Random.Range(-87, -82);
        capsule.transform.rotation = Quaternion.Euler(0, 0, randomRotation);
        // Play a sound or something here
    }
}
