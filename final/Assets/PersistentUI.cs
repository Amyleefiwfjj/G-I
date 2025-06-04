using UnityEngine;

public class PersistentUI : MonoBehaviour
{
    private static PersistentUI instance;

    void Awake()
    {
        // If there's already an instance and it's not this one, destroy this duplicate
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Mark this as the only instance
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
