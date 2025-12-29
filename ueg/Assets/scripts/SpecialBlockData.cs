using UnityEngine;
using System.Collections.Generic;

public class SpecialBlockData : MonoBehaviour
{
    private Dictionary<Vector3Int, SpecialBlockType> specialBlocks = new Dictionary<Vector3Int, SpecialBlockType>();

    public void RegisterSpecialBlock(Vector3Int position, SpecialBlockType type)
    {
        if (!specialBlocks.ContainsKey(position))
        {
            specialBlocks[position] = type;
            Debug.Log($"Special block registered at {position}: {type}");
        }
    }

    public void RemoveSpecialBlock(Vector3Int position)
    {
        if (specialBlocks.ContainsKey(position))
        {
            specialBlocks.Remove(position);
        }
    }

    public bool IsSpecialBlock(Vector3Int position)
    {
        return specialBlocks.ContainsKey(position);
    }

    public SpecialBlockType GetSpecialBlockType(Vector3Int position)
    {
        if (specialBlocks.ContainsKey(position))
        {
            return specialBlocks[position];
        }
        return SpecialBlockType.None;
    }

    public void Clear()
    {
        specialBlocks.Clear();
    }

    public Dictionary<Vector3Int, SpecialBlockType> GetAllSpecialBlocks()
    {
        return new Dictionary<Vector3Int, SpecialBlockType>(specialBlocks);
    }
}