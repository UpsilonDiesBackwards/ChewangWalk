using System.Collections.Generic;
using UnityEngine;

public class PlayerScent : MonoBehaviour
{
    public float lifetime = 120f;
    
    readonly List<ScentTrail> trail = new List<ScentTrail>();
    public IReadOnlyList<ScentTrail> scentTrail => trail;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LeaveScent();
    }

    // Update is called once per frame
    void Update()
    {
        FadeScent();
        LeaveScent();
    }

    void LeaveScent()
    {
        trail.Add(new ScentTrail { position = transform.position, time = Time.time });
    }

    void FadeScent()
    {
        // Counts how many points in the trail have expired
        int expired = 0;
        while (expired < trail.Count && Time.time - trail[expired].time > lifetime)
            expired++;
        
        // Removes the expired points
        if (expired > 0)
            trail.RemoveRange(0, expired);
    }
}
