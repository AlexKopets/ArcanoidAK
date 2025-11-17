using UnityEngine;
using System.Linq;

public class LevelSpawner : MonoBehaviour
{
    public GameObject[] brickPrefabs;  // <- сюда перетащить 3 префаба
    public int rows = 4;
    public int cols = 8;
    public Vector2 start = new Vector2(-7f, 3.2f);
    public Vector2 step = new Vector2(1.8f, -0.7f);
    public Transform parentForBricks;

    // Возвращает массив созданных объектов
    public GameObject[] SpawnAll()
    {
        if (brickPrefabs == null || brickPrefabs.Length == 0)
        {
            Debug.LogError("LevelSpawner: brickPrefabs не назначены!");
            return new GameObject[0];
        }

        var list = Enumerable.Range(0, rows * cols).Select(i =>
        {
            int r = i / cols;
            int c = i % cols;
            Vector2 pos = new Vector2(start.x + c * step.x, start.y + r * step.y);

            // Выбор префаба: пример — чередование по индексу
            int prefabIndex = (r * cols + c) % brickPrefabs.Length;
            GameObject prefab = brickPrefabs[prefabIndex];

            GameObject b = Instantiate(prefab, pos, Quaternion.identity, parentForBricks);
            return b;
        }).ToArray();

        return list;
    }
}