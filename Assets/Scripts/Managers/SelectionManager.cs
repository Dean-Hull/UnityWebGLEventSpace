using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance { get; private set; }

    private Dictionary<string, Speaker> _speakerRegistry = new();
    private readonly object _lock = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Register(Speaker speaker)
    {
        if (speaker == null || string.IsNullOrEmpty(speaker.Id)) return;

        lock (_lock)
        {
            _speakerRegistry[speaker.Id] = speaker;
        }
    }

    public void Unregister(Speaker speaker)
    {
        if (speaker == null || string.IsNullOrEmpty(speaker.Id)) return;

        lock (_lock)
        {
            _speakerRegistry.Remove(speaker.Id);
        }
    }

    public Speaker Get(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;

        lock (_lock)
        {
            _speakerRegistry.TryGetValue(id, out var speaker);
            return speaker;
        }
    }

    public bool HasSpeaker(string id)
    {
        return _speakerRegistry.ContainsKey(id);
    }

    public void Select(string id)
    {
        Debug.Log("Unity received selection: " + id);

        Speaker speaker = Get(id);

        if (speaker == null)
        {
            Debug.LogWarning("Speaker not found: " + id);
            return;
        }

        speaker.OnClick();
    }
}