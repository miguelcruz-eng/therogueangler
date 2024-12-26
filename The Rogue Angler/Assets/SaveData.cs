using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;


[System.Serializable]
public struct SaveData
{
    public static SaveData Instance;

    // map stuff
    public HashSet<string> sceneNames;

    // SaveData Stuf
    public string saveSceneName;
    public Vector2 savePos;

    public void Initialize()
    {
        if(!File.Exists(Application.persistentDataPath + "/save.point.data"))
        {
            BinaryWriter writer = new BinaryWriter(File.Create(Application.persistentDataPath + "/save.point.data"));
        }
        if(sceneNames == null)
        {
            sceneNames = new HashSet<string>();
        }
    }

    public void SaveLocation()
    {
        using(BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.point.data")))
        {
            writer.Write(saveSceneName);
            writer.Write(savePos.x);
            writer.Write(savePos.y);
        }
    }
}
