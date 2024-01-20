using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveGameManager : MonoBehaviour
{
    public static SaveData data;

    private void Awake()
    {
        SaveLoad.OnLoadGame += LoadData;
    }


    public void DeleteData()
    {
        SaveLoad.DeleteSaveData();
    }
    
    public static void SaveData()
    {
        var saveData = data;

        SaveLoad.Save(saveData);
    }
    private static void LoadData(SaveData _data)
    {
        data = _data;
    }

    public static void TryLoadData()
    {
        SaveLoad.Load();
    }
}
