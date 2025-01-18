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

    // player stuff
    public int playerHealth;
    public float playerPotion;
    public Vector2 playerPosition;
    public string lastScene;

    // enemy stuff


    public void Initialize()
    {
        if(!File.Exists(Application.persistentDataPath + "/save.point.data"))
        {
            BinaryWriter writer = new BinaryWriter(File.Create(Application.persistentDataPath + "/save.point.data"));
        }
        if (!File.Exists(Application.persistentDataPath + "/save.player.data"))
        {
            BinaryWriter writer = new BinaryWriter(File.Create(Application.persistentDataPath + "/save.player.data"));
        }
        if (sceneNames == null)
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

    public void LoadSave()
    {
        if(File.Exists(Application.persistentDataPath + "/save.point.data"))
        {
            using(BinaryReader reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + "/save.point.data")))
            {
                saveSceneName = reader.ReadString();
                savePos.x = reader.ReadSingle();
                savePos.y = reader.ReadSingle();
            }
        }
    }

    public void SavePlayerData()
    {
        using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.player.data")))
        {
            playerHealth = PlayerController.Instance.Health;
            writer.Write(playerHealth);
            playerPotion = PlayerController.Instance.Energy;
            writer.Write(playerPotion);

            playerPosition = PlayerController.Instance.transform.position;
            writer.Write(playerPosition.x);
            writer.Write(playerPosition.y);

            lastScene = SceneManager.GetActiveScene().name;
            writer.Write(lastScene);
        }
    }

    public void LoadPlayerData()
    {
        if (File.Exists(Application.persistentDataPath + "/save.player.data"))
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + "/save.player.data")))
            {
                playerHealth = reader.ReadInt32();
                playerPotion = reader.ReadSingle();
                playerPosition.x = reader.ReadSingle();
                playerPosition.y = reader.ReadSingle();
                lastScene = reader.ReadString();

                SceneManager.LoadScene(lastScene);
                PlayerController.Instance.transform.position = playerPosition;
                PlayerController.Instance.Energy = playerPotion;
                PlayerController.Instance.Health = playerHealth;
            }
        }
        else
        {
            Debug.Log("File doenst exist");
            PlayerController.Instance.Health = PlayerController.Instance.maxHealth;
            PlayerController.Instance.Energy = 0.5f;
        }
    }
}
