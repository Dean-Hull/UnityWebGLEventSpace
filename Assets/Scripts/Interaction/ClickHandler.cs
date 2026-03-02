using UnityEngine;

public class ClickHandler : MonoBehaviour
{
    [SerializeField] private string id;

    private void OnMouseDown()
    {
        BrowserBridge.Instance?.ObjectClicked(id);
    }
}