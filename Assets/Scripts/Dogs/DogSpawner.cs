using System;
using UnityEngine;

public class DogSpawner : MonoBehaviour {
    [Header("References")]
    [SerializeField] private GameObject DogPrefab;
    [SerializeField] private GameObject DogBed;
    
    [Header("Settings")]
    [SerializeField] public float spawnRadius = 5.0f;

    private void OnDrawGizmos() {
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);

        Gizmos.DrawSphere(transform.position, spawnRadius);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
