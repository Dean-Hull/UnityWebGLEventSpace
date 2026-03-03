using UnityEngine;

public class Speaker : MonoBehaviour
{
    [SerializeField] private string id;
    public string Id => id;

    private bool IsValid => !string.IsNullOrEmpty(id);

    void OnEnable()
    {
        if (!IsValid) return;
        SelectionManager.Instance?.Register(this);
    }

    void OnDisable()
    {
        if (!IsValid) return;
        SelectionManager.Instance?.Unregister(this);
    }

    private void OnMouseDown()
    {
        if (!IsValid) return;
        BrowserBridge.Instance?.ObjectClicked(id);
    }
}