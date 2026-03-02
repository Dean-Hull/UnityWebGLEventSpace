using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public void SelectObject(string id)
    {
        Debug.Log("Selected object with ID: " + id);

        GameObject target = GameObject.Find(id);

        if (target != null)
        {
            Debug.Log("Object found: " + target.name);
        }
        else
        {
            Debug.LogWarning("No object found with ID: " + id);
        }
    }
}
