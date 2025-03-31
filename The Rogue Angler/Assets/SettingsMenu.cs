using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;

    public void SetVolume(float _volume)
    {
        audioMixer.SetFloat("Volume", _volume);    
    }

    public void ResetSavedData()
    {
        DeleteData("save.point.data");
        DeleteData("save.player.data");
    }

    private void DeleteData(string fileName)
    {
        using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.player.data")))
        {
            writer.Write(50);
            writer.Write(1f);

            writer.Write(-39.35f);
            writer.Write(0f);

            writer.Write("Fase1");
        }
        using(BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.point.data")))
        {
            writer.Write("Fase1");
            writer.Write(-39.35f);
            writer.Write(0f);
        }
    }

    public void SetQuality(int _qualityIndex)
    {
        QualitySettings.SetQualityLevel(_qualityIndex);
    }

    public void SetFullScreen(bool _isFullScreen)
    {
        Screen.fullScreen = _isFullScreen;
    }

    public void Quit()
    {
        Application.Quit();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
