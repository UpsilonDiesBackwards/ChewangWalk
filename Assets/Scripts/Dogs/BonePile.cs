using System.Collections.Generic;
using UnityEngine;

public class BonePile : MonoBehaviour
{
    private readonly List<string> bones = new List<string>();
    
    public IReadOnlyList<string> boneList => bones;
    public bool empty => bones.Count == 0;
    Renderer[] renderers;

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>(true);
        UpdateVisibility();
    }

    public void AddBone(string bone)
    {
        if (bone == null) return;
        bones.Add(bone);
        UpdateVisibility();
    }

    public bool TakeBone(out string bone)
    {
        if (empty)
        {
            bone = null;
            return false;
        }
        else
        {
            bone = bones[bones.Count - 1];
            bones.RemoveAt(bones.Count - 1);
            UpdateVisibility();
            return true;
        }
    }
    
    void UpdateVisibility()
    {
        foreach (Renderer renderer in renderers) renderer.enabled = !empty;
    }
}
