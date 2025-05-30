using UnityEngine;

public class Track : MonoBehaviour
{
    public Transform SpawnPoint;
    public Hole[] Holes;

    private void Awake()
    {
        if (SpawnPoint == null)
        {
            SpawnPoint = transform.Find("SpawnPoint");
            if (SpawnPoint == null)
            {
                Debug.LogWarning($"[Track] No SpawnPoint found on {gameObject.name}");
            }
        }
    }
}
