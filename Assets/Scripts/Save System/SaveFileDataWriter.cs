using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
public class SaveFileDataWriter
{
    public string saveDataDirectoryPath = "";
    public string saveFileName = "";

    public bool CheckToSeeIfFileExists()
    {
        if (File.Exists(Path.Combine(saveDataDirectoryPath, saveFileName)))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void DeleteSaveFile()
    {
        File.Delete(Path.Combine(saveDataDirectoryPath, saveFileName));
    }
    
    public void CreateNewCharacterSaveFile(CharacterSaveData characterData)
    {
        string savePath = Path.Combine(saveDataDirectoryPath, saveFileName);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(savePath));
            Debug.Log("Created directory: " + Path.GetDirectoryName(savePath));

            string dataToStore = JsonUtility.ToJson(characterData, true);

            using (FileStream stream = new FileStream(savePath, FileMode.Create))
            {
                using (StreamWriter fileWriter = new StreamWriter(stream))
                {
                    fileWriter.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error saving file: " + savePath + "\n" + e.Message);
        }
    }

    public CharacterSaveData LoadSaveFile()
    {
        CharacterSaveData characterData = null;

        string loadPath = Path.Combine(saveDataDirectoryPath, saveFileName);

        if (File.Exists(loadPath))
        {
            try
            {
                string dataToLoad = "";

                using (FileStream stream = new FileStream(loadPath, FileMode.Open))
                {
                    using (StreamReader fileReader = new StreamReader(stream))
                    {
                        dataToLoad = fileReader.ReadToEnd();
                    }
                }

                characterData = JsonUtility.FromJson<CharacterSaveData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error loading file: " + loadPath + "\n" + e.Message);
            }
        }
        return characterData;
    }
}
