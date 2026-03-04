using UnityEngine;
using System.Runtime.InteropServices;

public class CameraClickEvent : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void NotifyObjectClicked(string objectName);
#endif

    [SerializeField] private bool interactive = true;
    [SerializeField] private float focusDistance = 10f;

    private CameraController _cam;

    void Start() => _cam = CameraController.Instance;

    public void OnClick()
    {
        if (!interactive || _cam == null) return;

        _cam.SetCameraTarget(transform, focusDistance);
    }
}
