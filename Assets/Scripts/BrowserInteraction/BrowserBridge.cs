using UnityEngine;
using System.Runtime.InteropServices;

public class BrowserBridge : MonoBehaviour
{
    public static BrowserBridge Instance { get; private set; }

    [DllImport("__Internal")]
    private static extern void NotifyObjectClicked(string name);

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void ObjectClicked(string name)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        NotifyObjectClicked(name);
#else
        Debug.Log($"Editor object clicked: {name}");
#endif
    }
}
