using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // Static instance reference
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        // Check if instance already exists
        if (Instance == null)
        {
            Instance = GetComponent<T>();
        }
        else if (Instance != this)
        {
            // Duplicate found - destroy it
            Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        // Clear instance if this is the current instance
        if (Instance == this)
        {
            Instance = null;
        }
    }
}