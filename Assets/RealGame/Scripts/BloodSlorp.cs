using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;


public class BloodSlorp : MonoBehaviour
{
  public float radius = 5f;
  public float slorpDuration = 1f;
  public KeyCode activationKey = KeyCode.Space;

  private ParticleSystem bloodParticles;
  private BloodSplatterTilemap bloodTilemap;
  private Tilemap tilemap;
  private bool isSlorping = false;

  private void Start()
  {
    bloodParticles = GetComponent<ParticleSystem>();
    if (bloodParticles == null)
    {
      Debug.LogError("ParticleSystem component not found on BloodSlorp object!");
      return;
    }

    bloodTilemap = FindObjectOfType<BloodSplatterTilemap>();
    if (bloodTilemap == null)
    {
      Debug.LogError("BloodSplatterTilemap not found in the scene!");
      return;
    }

    tilemap = bloodTilemap.GetComponent<Tilemap>();
    if (tilemap == null)
    {
      Debug.LogError("Tilemap component not found on BloodSplatterTilemap object!");
      return;
    }
  }

  private void Update()
  {
    if (Input.GetKeyDown(activationKey) && !isSlorping)
    {
      SlorpBlood();
    }
  }
  private void SlorpBlood()
  {
    List<Vector3Int> bloodTiles = GetBloodTilesInRadius(transform.position, radius);
    isSlorping = true;

    foreach (Vector3Int tilePos in bloodTiles)
    {
      Color bloodColor = bloodTilemap.bloodColor;
      Vector3 worldPos = tilemap.CellToWorld(tilePos) + tilemap.cellSize / 2;

      // Spawn particles
      ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams
      {
        position = worldPos,
        //startColor = bloodColor,
        startSize = tilemap.cellSize.x * 2// Set particle size to match tilemap cell size, x2
      };
      bloodParticles.Emit(emitParams, 1);

      // Use Debug.DrawLine to visualize the tile position
      Debug.DrawLine(worldPos + new Vector3(-0.1f, 0.1f, 0), worldPos + new Vector3(0.1f, -0.1f, 0), Color.red, 1f);
      Debug.DrawLine(worldPos - Vector3.one * 0.1f, worldPos + Vector3.one * 0.1f, Color.red, 1f);

      // Remove blood from the tile
      tilemap.SetTile(tilePos, null);
    }
    isSlorping = false;
  }

  private List<Vector3Int> GetBloodTilesInRadius(Vector3 center, float radius)
  {
    List<Vector3Int> bloodTiles = new List<Vector3Int>();
    Vector3Int centerCell = tilemap.WorldToCell(center);
    int cellRadius = Mathf.CeilToInt(radius / tilemap.cellSize.x);

    for (int x = -cellRadius; x <= cellRadius; x++)
    {
      for (int y = -cellRadius; y <= cellRadius; y++)
      {
        Vector3Int cellPos = centerCell + new Vector3Int(x, y, 0);
        if (Vector3.Distance(tilemap.CellToWorld(cellPos), center) <= radius)
        {
          if (tilemap.HasTile(cellPos))
          {
            bloodTiles.Add(cellPos);
          }
        }
      }
    }

    return bloodTiles;
  }
}
