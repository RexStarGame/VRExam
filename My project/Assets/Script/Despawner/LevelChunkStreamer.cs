using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class LevelChunkStreamer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    // Ændret fra én Transform til et array af Transforms, så du kan tilføje flere parents
    [SerializeField] private Transform[] chunkParents;

    [Header("Streaming Distances (X axis)")]
    [SerializeField] private float activateAheadDistance = 80f;
    [SerializeField] private float keepBehindDistance = 25f;
    [SerializeField] private float deactivateAheadDistance = 120f;

    [Header("Checks")]
    [SerializeField] private float checkInterval = 0.1f;
    [SerializeField] private bool refreshStatesOnAwake = true;

    private readonly List<Transform> chunks = new List<Transform>(128);
    private float checkTimer;

    private void Awake()
    {
        if (player == null)
            player = transform;

        CacheChunks();

        if (refreshStatesOnAwake)
            RefreshChunkStates(true);
    }

    private void Update()
    {
        checkTimer += Time.deltaTime;

        if (checkTimer < checkInterval)
            return;

        checkTimer = 0f;
        RefreshChunkStates(false);
    }

    [ContextMenu("Cache Chunks From Parents")]
    public void CacheChunks()
    {
        chunks.Clear();

        // Tjekker om listen er tom
        if (chunkParents == null || chunkParents.Length == 0)
            return;

        int totalChildCount = 0;

        // Først tæller vi det samlede antal children på tværs af alle parents
        foreach (Transform parent in chunkParents)
        {
            if (parent != null)
                totalChildCount += parent.childCount;
        }

        if (chunks.Capacity < totalChildCount)
            chunks.Capacity = totalChildCount;

        // Derefter tilføjer vi alle children til vores ene store liste
        foreach (Transform parent in chunkParents)
        {
            if (parent == null)
                continue;

            for (int i = 0; i < parent.childCount; i++)
            {
                Transform chunk = parent.GetChild(i);

                if (chunk != null)
                    chunks.Add(chunk);
            }
        }
    }

    private void RefreshChunkStates(bool forceUpdate)
    {
        if (player == null)
            return;

        float playerX = player.position.x;

        for (int i = 0; i < chunks.Count; i++)
        {
            Transform chunk = chunks[i];

            if (chunk == null)
                continue;

            GameObject chunkObject = chunk.gameObject;
            float deltaX = chunk.position.x - playerX;

            bool shouldBeActive;

            if (chunkObject.activeSelf)
            {
                // Already active: let it stay active a bit farther ahead to avoid flicker
                shouldBeActive = deltaX >= -keepBehindDistance && deltaX <= deactivateAheadDistance;
            }
            else
            {
                // Inactive: only activate when close enough
                shouldBeActive = deltaX >= -keepBehindDistance && deltaX <= activateAheadDistance;
            }

            if (forceUpdate || chunkObject.activeSelf != shouldBeActive)
                chunkObject.SetActive(shouldBeActive);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (activateAheadDistance < 0f)
            activateAheadDistance = 0f;

        if (keepBehindDistance < 0f)
            keepBehindDistance = 0f;

        if (deactivateAheadDistance < activateAheadDistance)
            deactivateAheadDistance = activateAheadDistance;

        if (checkInterval < 0.02f)
            checkInterval = 0.02f;
    }
#endif
}