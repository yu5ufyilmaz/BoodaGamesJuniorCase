using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData
{
    public SerializableDictionary<string, ChestSaveData> chestDictionary;

    public SaveData()
    {
        chestDictionary = new SerializableDictionary<string, ChestSaveData>();
        
    }
}
