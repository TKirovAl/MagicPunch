using System.Collections.Generic;
using UnityEngine;

public class MagicManager : MonoBehaviour
{
    [System.Serializable]
    public class MagicTypeData
    {
        public string id;
        public MagicType magicType;
    }
    
    [SerializeField] private List<MagicTypeData> magicTypes = new List<MagicTypeData>();
    
    private Dictionary<string, MagicType> magicDictionary = new Dictionary<string, MagicType>();
    
    private void Awake()
    {
        InitializeMagicDictionary();
    }
    
    private void InitializeMagicDictionary()
    {
        foreach (var data in magicTypes)
        {
            if (!magicDictionary.ContainsKey(data.id))
            {
                magicDictionary.Add(data.id, data.magicType);
            }
        }
    }
    
    public MagicType GetMagicType(string id)
    {
        if (magicDictionary.TryGetValue(id, out MagicType magicType))
        {
            return magicType;
        }
        
        Debug.LogWarning($"Magic type with id {id} not found!");
        return null;
    }
    
    // Метод для создания типов магии через код
    public MagicType CreateMagicType(string name, Color color, float damage, float cost)
    {
        return new MagicType
        {
            magicName = name,
            magicColor = color,
            baseDamage = damage,
            manaCost = cost
        };
    }
}