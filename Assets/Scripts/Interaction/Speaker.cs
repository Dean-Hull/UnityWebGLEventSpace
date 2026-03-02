using UnityEngine;

public class Speaker : MonoBehaviour
{
    public void Select(string id)
    {
        Debug.Log($"Speaker {gameObject.name} selected.");
    }
}