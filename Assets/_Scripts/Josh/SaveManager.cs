using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveManager
{
    public static string directory = "SaveData";
    public static string fileName = "Settings.dat";

    public static void Save(Volume lo)
    {
        if (!DirectoryExists())
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/" + directory);
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(GetFullPath());
        bf.Serialize(file, lo);
        file.Close();
    }

    public static void InitialSave(Volume lo)
    {
        if (!SaveExists())
        {
            Save(lo);
        }
    }

    public static void SaveArray(Volume[] lo)
    {
        if (!DirectoryExists())
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/" + directory);
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(GetFullPath());
        bf.Serialize(file, lo);
        file.Close();

        Debug.Log("Save Successful");
    }

    public static Volume Load()
    {
        if (SaveExists())
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(GetFullPath(), FileMode.Open);
                Volume lo = (Volume)bf.Deserialize(file);
                file.Close();

                return lo;
            }
            catch (SerializationException)
            {
                Debug.Log("Failed to load file");
            }
        }

        return null;
    }

    public static Volume[] LoadArray()
    {
        if (SaveExists())
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(GetFullPath(), FileMode.Open);
                Volume[] lo = (Volume[])bf.Deserialize(file);
                file.Close();
                Debug.Log("Load Successful");

                return lo;

            }
            catch (SerializationException)
            {
                Debug.Log("Failed to load file");
            }
        }

        return null;
    }

    public static bool SaveExists()
    {
        return File.Exists(GetFullPath());
    }

    public static void DeleteSave()
    {
        File.Delete(GetFullPath());
    }

    private static bool DirectoryExists()
    {
        return Directory.Exists(Application.persistentDataPath + "/" + directory);
    }

    private static string GetFullPath()
    {
        return Application.persistentDataPath + "/" + directory + "/" + fileName;
    }

}
