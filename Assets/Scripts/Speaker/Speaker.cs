using UnityEngine;

public class Speaker : MonoBehaviour
{
    [SerializeField] private string id;
    public string Id => id;

    void OnEnable()
    {
        if (SpeakerManager.Instance != null && !string.IsNullOrEmpty(id))
        {
            SpeakerManager.Instance?.Register(this);
        }
    }

    void OnDisable()
    {
        if (SpeakerManager.Instance != null && !string.IsNullOrEmpty(id))
        {
            SpeakerManager.Instance?.Unregister(this);
        }
    }

    private void OnMouseDown()
    {
        if (!string.IsNullOrEmpty(id))
        {
            BrowserBridge.Instance?.ObjectClicked(id);
        }
    }
}