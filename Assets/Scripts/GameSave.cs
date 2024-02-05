using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameSave : MonoBehaviour
{
    public float ProcentOffAsyncLoad { get; private set; }
    public Action OnLoad;
    public Action OnSave;

    [SerializeField] private List<MonoBehaviour> _savebles = new List<MonoBehaviour>();

    private static string _quickSaveDirectory => "quicksave";
    private static string _standartSaveDirectory => "standard";

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.F5))
        {
            Save(_quickSaveDirectory);
        }
        if (Input.GetKeyDown(KeyCode.F12))
        {
            Load(_quickSaveDirectory);
        }
    }

    public void QuickLoad()
    {
        Load(_quickSaveDirectory);
    }

    public void AsyncQuickLoad()
    {
        StartCoroutine(LoadAsync(_quickSaveDirectory));
    }

    public void UpdateQuickSaveFromStandart()
    {
        if (Directory.Exists("./Saves/" + _quickSaveDirectory) == true)
        {
            DeleteSave(_quickSaveDirectory);
        }
        CopySave(_standartSaveDirectory, _quickSaveDirectory);
    }

    public void Save(string saveName)
    {
        if (Directory.Exists(saveName) == false)
        {
            Directory.CreateDirectory("./Saves/" + saveName);
        }
        foreach (var sv in _savebles)
        {
            var str = (sv as ISaveble).Save();
            File.WriteAllText("./Saves/" + saveName + "/" + (sv as ISaveble).GetFileName() + ".txt", str);
        }
        OnSave?.Invoke();
    }

    public void Load(string saveName)
    {
        foreach (var sv in _savebles)
        {
            var fileName = "./Saves/" + saveName + "/" + (sv as ISaveble).GetFileName() + ".txt";
            if (File.Exists(fileName))
            {
                var str = File.ReadAllText(fileName);
                (sv as ISaveble).Load(str);
            }
            else
            {
                Debug.LogError("File not found error.");
            }
        }
        OnLoad?.Invoke();
    }

    public IEnumerator LoadAsync(string saveName)
    {
        yield return null;
        foreach (var sv in _savebles)
        {
            yield return null;
            ProcentOffAsyncLoad = (((float)_savebles.IndexOf(sv)) / ((float)_savebles.Count));
            var fileName = "./Saves/" + saveName + "/" + (sv as ISaveble).GetFileName() + ".txt";
            if (File.Exists(fileName))
            {
                var str = File.ReadAllText(fileName);
                (sv as ISaveble).Load(str);
            }

        }
        OnLoad?.Invoke();
    }

    public static string GetSavePlayerCountryID(string saveName)
    {
        var fileName = "./Saves/" + saveName + "/player.txt";
        if (File.Exists(fileName))
        {
            var str = File.ReadAllText(fileName);
            return JsonUtility.FromJson<Player.PlayerSerialize>(str).CountryID;
        }
        else
        {
            throw new FileNotFoundException();
        }
    }

    public static void SetSavePlayerCountryIDInQuickSave(string countryId)
    {
        var fileName = "./Saves/" + _quickSaveDirectory + "/player.txt";
        if (File.Exists(fileName))
        {
            File.WriteAllText(fileName, JsonUtility.ToJson(new Player.PlayerSerialize(countryId)));
        }
        else
        {
            throw new FileNotFoundException();
        }
    }

    public static List<string> GetSaves()
    {
        var directories = Directory.GetDirectories("./Saves");
        var result = new List<string>();
        for (int i = 0; i < directories.Length; i++)
        {
            result.Add(Path.GetFileName(directories[i]));
        }
        return result;
    }

    public static List<SaveSlotData> GetSavesData()
    {
        var directories = Directory.GetDirectories("./Saves");
        var result = new List<SaveSlotData>();
        for (int i = 0; i < directories.Length; i++)
        {
            var saveName = Path.GetFileName(directories[i]);
            result.Add(new SaveSlotData(saveName, GetSavePlayerCountryID(saveName)));
        }
        return result;
    }

    public class SaveSlotData
    {
        public string SaveName { get; }
        public string CountryID { get; }

        public SaveSlotData(string saveName, string countryID)
        {
            SaveName = saveName;
            CountryID = countryID;
        }
    }

    public void DeleteSave(string saveName)
    {
        if (Directory.Exists("./Saves/" + saveName) == false)
        {
            Debug.LogError($"File {saveName} not found error.");
            return;
        }
        Directory.Delete("./Saves/" + saveName, true);
    }

    public void CopySave(string saveName, string distanationName)
    {
        if(saveName == distanationName)
        {
            throw new ArgumentException();
        }
        CopyDirectory("./Saves/" + saveName, "./Saves/" + distanationName, true);
    }

    private void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
    {
        var dir = new DirectoryInfo(sourceDir);
        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");
        }

        DirectoryInfo[] dirs = dir.GetDirectories();
        Directory.CreateDirectory(destinationDir);

        foreach (FileInfo file in dir.GetFiles())
        {
            string targetFilePath = Path.Combine(destinationDir, file.Name);
            file.CopyTo(targetFilePath);
        }
        if (recursive)
        {
            foreach (DirectoryInfo subDir in dirs)
            {
                string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                CopyDirectory(subDir.FullName, newDestinationDir, true);
            }
        }
    }
}

public interface ISaveble
{
    string GetFileName();
    string Save();
    void Load(string data);
    Type GetSaveType();
}

public abstract class SerializeForSave
{
    public abstract string SaveToJson();
    public abstract void Load(object objTarget);
}