using UnityEngine;
using System.Collections.Generic;

public class MainMenuClouds : MonoBehaviour
{
    [SerializeField] private GameObject cloudPrefab;
    [SerializeField] private float minMoveSpeed = 30f;
    [SerializeField] private float maxMoveSpeed = 80f;
    [SerializeField] private float minCloudScale = 0.5f;
    [SerializeField] private float maxCloudScale = 1.5f;
    [SerializeField] private float minSpawnInterval = 1f;
    [SerializeField] private float maxSpawnInterval = 3f;
    [SerializeField] private float minYSpawnPosition = -100f;
    [SerializeField] private float maxYSpawnPosition = 100f;

    private RectTransform parentRect;
    private float spawnTimer;
    private List<CloudInstance> activeCloud = new List<CloudInstance>();

    private class CloudInstance
    {
        public RectTransform rectTransform;
        public float moveSpeed;
    }

    private void Start()
    {
        parentRect = GetComponent<RectTransform>();
        spawnTimer = Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    // Update is called once per frame
    void Update()
    {
        // Spawn new clouds
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f && cloudPrefab != null)
        {
            SpawnCloud();
            spawnTimer = Random.Range(minSpawnInterval, maxSpawnInterval);
        }

        // Move active clouds
        for (int i = activeCloud.Count - 1; i >= 0; i--)
        {
            CloudInstance cloud = activeCloud[i];
            cloud.rectTransform.anchoredPosition += Vector2.right * cloud.moveSpeed * Time.deltaTime;

            // Remove if fully outside right side
            if (cloud.rectTransform.anchoredPosition.x > parentRect.rect.width)
            {
                Destroy(cloud.rectTransform.gameObject);
                activeCloud.RemoveAt(i);
            }
        }
    }

    private void SpawnCloud()
    {
        GameObject newCloud = Instantiate(cloudPrefab, transform);
        RectTransform cloudRect = newCloud.GetComponent<RectTransform>();

        // Random scale
        float randomScale = Random.Range(minCloudScale, maxCloudScale);
        cloudRect.localScale = Vector3.one * randomScale;

        // Random move speed
        float randomSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);

        // Spawn outside left side (far left of parent)
        float spawnX = -parentRect.rect.width * 0.5f - cloudRect.rect.width * randomScale * 0.5f;
        float spawnY = Random.Range(minYSpawnPosition, maxYSpawnPosition);
        cloudRect.anchoredPosition = new Vector2(spawnX, spawnY);

        CloudInstance cloudInstance = new CloudInstance
        {
            rectTransform = cloudRect,
            moveSpeed = randomSpeed
        };

        activeCloud.Add(cloudInstance);
    }
}
