using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;



[DefaultExecutionOrder(-1)]
public class Board : MonoBehaviour
{


    
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }
    public SpecialBlockData specialBlockData { get; private set; } 

    public TetrominoData[] tetrominoes;
    public TetrominoData[] specialBlockVariants;
    public Vector2Int boardSize = new Vector2Int(10, 20);
    public Vector3Int spawnPosition = new Vector3Int(-1, 8, 0);
    public TMP_Text multiplierText;


    [Header("Special Block Settings")]
    [Range(0f, 1f)]
    public float specialBlockChance = 0.15f;

    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<Piece>();

        specialBlockData = gameObject.AddComponent<SpecialBlockData>();

        for (int i = 0; i < tetrominoes.Length; i++)
        {
            tetrominoes[i].Initialize();
        }

        if (specialBlockVariants != null)
        {
            for (int i = 0; i < specialBlockVariants.Length; i++)
            {
                specialBlockVariants[i].Initialize();
            }
        }
    }
        
        
      
    private void Start()
    {
        SpawnPiece();


    
    }

    //public void SpawnPiece()
    //{
    //    TetrominoData data;

    //    if (Random.value < specialBlockChance && specialBlockVariants != null && specialBlockVariants.Length > 0)
    //    {
    //        int randomSpecial = Random.Range(0, specialBlockVariants.Length);
    //        data = specialBlockVariants[randomSpecial];
    //        Debug.Log($"Spawning special block: {data.specialType}");
    //    }
    //    else
    //    {
    //        int random = Random.Range(0, tetrominoes.Length);
    //        data = tetrominoes[random];
    //    }

    //    activePiece.Initialize(this, spawnPosition, data);

    //    if (IsValidPosition(activePiece, spawnPosition))
    //    {
    //        Set(activePiece);
    //    }
    //    else
    //    {
    //        GameOver();
    //    }
    //}
    public void SpawnPiece()
    {
        TetrominoData data;

        int level = 1;
        if (LevelManager.Instance != null)
            level = LevelManager.Instance.level;

        bool canSpawnSpecial = level >= 3;

        float chance = canSpawnSpecial
            ? Mathf.Clamp(0.1f + level * 0.02f, 0f, 0.4f)
            : 0f;

        if (Random.value < chance && specialBlockVariants.Length > 0)
        {
            data = specialBlockVariants[Random.Range(0, specialBlockVariants.Length)];
        }
        else
        {
            data = tetrominoes[Random.Range(0, tetrominoes.Length)];
        }

        activePiece.Initialize(this, spawnPosition, data);

        if (IsValidPosition(activePiece, spawnPosition))
            Set(activePiece);
        else
            GameOver();
    }

    public GameOverUI gameOverUI;

    public void GameOver()
    {
        tilemap.ClearAllTiles();
        specialBlockData.Clear();

        if (gameOverUI != null)
        {
            gameOverUI.ShowGameOver();
        }
        else
        {
            Debug.LogError("gameOverUI НЕ назначен в инспекторе!");
        }
    }

 
    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);

            if (piece.data.specialType != SpecialBlockType.None)
            {
                specialBlockData.RegisterSpecialBlock(tilePosition, piece.data.specialType);
            }
        }
    }

    public void Clear(Piece piece)
    {
        if (piece.cells == null)
            return;

        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;

        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            if (!bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }

            if (tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }

        return true;
    }

    public void ClearLines()
    {
        RectInt bounds = Bounds;
        int row = bounds.yMin;

        while (row < bounds.yMax)
        {
            if (IsLineFull(row))
            {
                bool hasSpecialBlock = HasSpecialBlockInRow(row);

                if (hasSpecialBlock)
                {
                    Debug.Log($"Special block triggered in row {row}");
                    TriggerSpecialBlocksInRow(row);

                    if (ScoreManager.Instance != null)
                    {
                        ScoreManager.Instance.AddScore(5);
                    }

                  
                }
                else
                {
                    Debug.Log($"Normal line clear at row {row}");
                    LineClear(row);

                    if (ScoreManager.Instance != null)
                    {
                        ScoreManager.Instance.AddScore(10);
                    }

                }
            }
            else
            {
                row++;
            }
        }
    }


    private bool HasSpecialBlockInRow(int row)
    {
        foreach (var kvp in specialBlockData.GetAllSpecialBlocks())
        {
            if (kvp.Key.y == row)
            {
                return true;
            }
        }
        return false;
    }

public void OpenSettings()
{
    SceneManager.LoadScene("settings"); 
}

    private void TriggerSpecialBlocksInRow(int row)
    {
        List<Vector3Int> toRemove = new List<Vector3Int>();

        foreach (var kvp in specialBlockData.GetAllSpecialBlocks())
        {
            Vector3Int pos = kvp.Key;
            SpecialBlockType type = kvp.Value;

            if (pos.y == row)
            {
                Debug.Log($"Triggering special block at {pos}: {type}");
                TriggerSpecialBlock(pos, type);
                toRemove.Add(pos);
            }
        }

        foreach (var pos in toRemove)
        {
            specialBlockData.RemoveSpecialBlock(pos);
        }
    }



    private void CheckAndTriggerSpecialBlocks()
    {
        var specialBlocks = specialBlockData.GetAllSpecialBlocks();
        List<Vector3Int> toRemove = new List<Vector3Int>();

        foreach (var kvp in specialBlocks)
        {
            Vector3Int pos = kvp.Key;
            SpecialBlockType type = kvp.Value;

            if (IsInCompletedLine(pos))
            {
                Debug.Log($"Special block triggered at {pos}: {type}");
                TriggerSpecialBlock(pos, type);
                toRemove.Add(pos);
            }
        }

        foreach (var pos in toRemove)
        {
            specialBlockData.RemoveSpecialBlock(pos);
        }
    }

    private bool IsInCompletedLine(Vector3Int pos)
    {
        return IsLineFull(pos.y);
    }

    public void TriggerSpecialBlock(Vector3Int position, SpecialBlockType type)
    {
        ScoreManager.Instance.AddScore(5);
        switch (type)
        {
            case SpecialBlockType.Horizontal:
                ClearHorizontalLine(position.y);
                break;

            case SpecialBlockType.Vertical:
                ClearVerticalLine(position.x);
                break;

            case SpecialBlockType.Cross:
                ClearHorizontalLine(position.y);
                ClearVerticalLine(position.x);
                break;
            case SpecialBlockType.Multiplier:
                StartCoroutine(ScoreMultiplierRoutine());
                break;
        }
    }
    private bool multiplierActive = false;

    private IEnumerator ScoreMultiplierRoutine()
    {
        if (multiplierActive)
            yield break;

        multiplierActive = true;
        ScoreManager.Instance.SetMultiplier(2);

        if (multiplierText != null)
            multiplierText.text = "x2 SCORE!";

        yield return new WaitForSeconds(10);

        ScoreManager.Instance.SetMultiplier(1);

        if (multiplierText != null)
            multiplierText.text = "";

        multiplierActive = false;
    }


    private void ClearHorizontalLine(int row)
    {
        RectInt bounds = Bounds;
        Debug.Log($"Clearing horizontal line at row {row}");

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            tilemap.SetTile(position, null);
            specialBlockData.RemoveSpecialBlock(position);
        }

        // Применяем гравитацию 
        for (int y = row + 1; y < bounds.yMax; y++)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int from = new Vector3Int(col, y, 0);
                Vector3Int to = new Vector3Int(col, y - 1, 0);

                TileBase tile = tilemap.GetTile(from);
                tilemap.SetTile(to, tile);
                tilemap.SetTile(from, null);

                if (specialBlockData.IsSpecialBlock(from))
                {
                    SpecialBlockType type = specialBlockData.GetSpecialBlockType(from);
                    specialBlockData.RemoveSpecialBlock(from);
                    if (tile != null) 
                    {
                        specialBlockData.RegisterSpecialBlock(to, type);
                    }
                }
            }
        }
     
    }


    private void ClearVerticalLine(int col)
    {
        RectInt bounds = Bounds;
        Debug.Log($"Clearing vertical line at column {col}");

        for (int row = bounds.yMin; row < bounds.yMax; row++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            tilemap.SetTile(position, null);
            specialBlockData.RemoveSpecialBlock(position);
        }

        ApplyGravityToColumn(col);
    }

    private void ApplyGravityToColumn(int col)
    {
        RectInt bounds = Bounds;

        for (int row = bounds.yMin; row < bounds.yMax; row++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            if (tilemap.GetTile(position) == null)
            {
                for (int above = row + 1; above < bounds.yMax; above++)
                {
                    Vector3Int abovePos = new Vector3Int(col, above, 0);
                    TileBase tile = tilemap.GetTile(abovePos);

                    if (tile != null)
                    {
                        // Перемещаем блок вниз
                        tilemap.SetTile(position, tile);
                        tilemap.SetTile(abovePos, null);

                        // Переносим информацию о спецблоке
                        if (specialBlockData.IsSpecialBlock(abovePos))
                        {
                            SpecialBlockType type = specialBlockData.GetSpecialBlockType(abovePos);
                            specialBlockData.RemoveSpecialBlock(abovePos);
                            specialBlockData.RegisterSpecialBlock(position, type);
                        }
                        break;
                    }
                }
            }
        }
    }

    public bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            if (!tilemap.HasTile(position))
            {
                return false;
            }
        }

        return true;
    }

    public void LineClear(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            tilemap.SetTile(position, null);
            specialBlockData.RemoveSpecialBlock(position);
        }

        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                tilemap.SetTile(position, above);

                Vector3Int abovePos = new Vector3Int(col, row + 1, 0);
                if (specialBlockData.IsSpecialBlock(abovePos))
                {
                    SpecialBlockType type = specialBlockData.GetSpecialBlockType(abovePos);
                    specialBlockData.RemoveSpecialBlock(abovePos);
                    if (above != null)
                    {
                        specialBlockData.RegisterSpecialBlock(position, type);
                    }
                }
            }

            row++;
        }
    }
        public void LoadScene(string sceneName)

    {

        SceneManager.LoadScene(sceneName);

    }



    public TMP_Dropdown graphicsDropdown;

    public Slider  musicVol;

    public AudioMixer mainAudioMixer;



    public void ChangeGraphicsQuality()

    {

        QualitySettings.SetQualityLevel(graphicsDropdown.value);

    }



    public void ChangeMasterVolume()

    {

        mainAudioMixer.SetFloat("MusicVol", musicVol.value);

    }


    
}