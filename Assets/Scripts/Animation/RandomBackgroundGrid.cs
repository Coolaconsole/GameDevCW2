using UnityEngine;

[ExecuteInEditMode]
public class RandomBackgroundGrid : MonoBehaviour
{
    [Header("Tile Settings")]
    public Sprite[] grassTiles;
    
    [Header("Grid Dimensions")]
    public int width = 30;
    public int height = 20;
    public float spacing = 1.0f;

    [Header("Seam Fixes")]
    [Range(1f, 1.05f)] 
    public float scaleBuffer = 1.01f; 
    public bool enablePixelSnapping = true;

    [Header("Randomization")]
    public int seed = 42;

    [Header("Rendering")]
    public string sortingLayer = "Background";
    public int orderInLayer = -10;

    private void OnValidate()
    {
        #if UNITY_EDITOR
    
        if (UnityEditor.EditorUtility.IsPersistent(this)) return;
        if (UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null) return;

        UnityEditor.EditorApplication.delayCall += () => {
            if (this != null) GenerateGrid();
        };
        #endif
    }

    public void GenerateGrid()
    {
        if (grassTiles == null || grassTiles.Length == 0) return;

        ClearGrid();
        Random.InitState(seed);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                CreateTile(x, y);
            }
        }
    }

    private void CreateTile(int x, int y)
    {
        GameObject tile = new GameObject($"Grass_{x}_{y}");
        tile.transform.SetParent(this.transform);

        float posX = (x - width / 2f) * spacing;
        float posY = (y - height / 2f) * spacing;
        tile.transform.localPosition = new Vector3(posX, posY, 0);
        tile.transform.localScale = Vector3.one * scaleBuffer;

        SpriteRenderer sr = tile.AddComponent<SpriteRenderer>();
        sr.sprite = grassTiles[Random.Range(0, grassTiles.Length)];
        
        if (enablePixelSnapping)
        {
           
            Material tempMat = new Material(Shader.Find("Sprites/Default"));
            tempMat.EnableKeyword("PIXELSNAP_ON");
            sr.sharedMaterial = tempMat;
        }

        sr.sortingLayerName = sortingLayer;
        sr.sortingOrder = orderInLayer;
    }

    public void ClearGrid()
    {
        
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}