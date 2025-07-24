using UnityEngine;
using System.IO;
<<<<<<< Updated upstream
using System.Security.Cryptography;
using System.Text;
using System;

public static class SaveSystem
{
    
    // Encryption key and IV 
    private static readonly byte[] Key = new byte[16] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10 };
    private static readonly byte[] IV = new byte[16] { 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F, 0x20 };

    private static string Encrypt(string json)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Key;
            aes.IV = IV;
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
            byte[] encryptedBytes = encryptor.TransformFinalBlock(jsonBytes, 0, jsonBytes.Length);
            return Convert.ToBase64String(encryptedBytes); // Uses System.Convert
        }
    }

    private static string Decrypt(string encryptedJson)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Key;
            aes.IV = IV;
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            byte[] encryptedBytes = Convert.FromBase64String(encryptedJson); // Uses System.Convert
            byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }

=======

public static class SaveSystem
{
>>>>>>> Stashed changes
    public static void SaveProfile(PlayerProfile profile)
    {
        string path = Application.persistentDataPath + $"/{profile.profileName}_profile.json";
        string json = JsonUtility.ToJson(profile);
<<<<<<< Updated upstream
        string encryptedJson = Encrypt(json);
=======
>>>>>>> Stashed changes
        try
        {
            // If the file exists, ensure it's not read-only before writing
            if (File.Exists(path))
            {
                File.SetAttributes(path, File.GetAttributes(path) & ~FileAttributes.ReadOnly);
            }

<<<<<<< Updated upstream
            // Save the encrypted profile
            File.WriteAllText(path, encryptedJson);
=======
            // Save the profile
            File.WriteAllText(path, json);
>>>>>>> Stashed changes

            // Set the file as read-only after saving
            File.SetAttributes(path, File.GetAttributes(path) | FileAttributes.ReadOnly);

<<<<<<< Updated upstream
            Debug.Log($"[SaveSystem] Saved encrypted profile: {profile.profileName} at {path} with gold: {profile.gold}, set as read-only");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveSystem] Failed to save encrypted profile: {profile.profileName} at {path}. Error: {e.Message}");
=======
            Debug.Log($"[SaveSystem] Saved profile: {profile.profileName} at {path} with gold: {profile.gold}, set as read-only");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveSystem] Failed to save profile: {profile.profileName} at {path}. Error: {e.Message}");
>>>>>>> Stashed changes
        }
    }

    public static PlayerProfile LoadProfile(string profileName)
    {
        string path = Application.persistentDataPath + $"/{profileName}_profile.json";
<<<<<<< Updated upstream
        Debug.Log($"[SaveSystem] Attempting to load encrypted profile: {profileName} from {path}");
=======
        Debug.Log($"[SaveSystem] Attempting to load profile: {profileName} from {path}");
>>>>>>> Stashed changes
        if (File.Exists(path))
        {
            try
            {
<<<<<<< Updated upstream
                string encryptedJson = File.ReadAllText(path);
                string json = Decrypt(encryptedJson); // Decrypt first to get valid JSON
=======
                string json = File.ReadAllText(path);
>>>>>>> Stashed changes
                PlayerProfile profile = JsonUtility.FromJson<PlayerProfile>(json);
                if (profile != null)
                {
                    profile.InitializeLevelStars();
                    profile.InitializeMissionProgress();
<<<<<<< Updated upstream
                    Debug.Log($"[SaveSystem] Successfully loaded encrypted profile: {profileName} with gold: {profile.gold}");
                    return profile;
                }
                Debug.LogError($"[SaveSystem] Failed to deserialize encrypted profile: {profileName}");
=======
                    Debug.Log($"[SaveSystem] Successfully loaded profile: {profileName} with gold: {profile.gold}");
                    return profile;
                }
                Debug.LogError($"[SaveSystem] Failed to deserialize profile: {profileName}");
>>>>>>> Stashed changes
                return null;
            }
            catch (System.Exception e)
            {
<<<<<<< Updated upstream
                Debug.LogError($"[SaveSystem] Failed to load encrypted profile: {profileName} at {path}. Error: {e.Message}");
                return null;
            }
        }
        Debug.LogError($"[SaveSystem] Encrypted profile not found: {profileName} at {path}");
=======
                Debug.LogError($"[SaveSystem] Failed to load profile: {profileName} at {path}. Error: {e.Message}");
                return null;
            }
        }
        Debug.LogError($"[SaveSystem] Profile not found: {profileName} at {path}");
>>>>>>> Stashed changes
        return null;
    }

    public static bool ProfileExists(string profileName)
    {
        string path = Application.persistentDataPath + $"/{profileName}_profile.json";
        bool exists = File.Exists(path);
        Debug.Log($"[SaveSystem] Checking if profile exists: {profileName} at {path} - Exists: {exists}");
        return exists;
    }

    public static void DeleteProfile(string profileName)
    {
        string path = Application.persistentDataPath + $"/{profileName}_profile.json";
        Debug.Log($"[SaveSystem] Attempting to delete profile: {profileName} at {path}");
        if (File.Exists(path))
        {
            try
            {
                // Remove read-only attribute before deletion
                File.SetAttributes(path, File.GetAttributes(path) & ~FileAttributes.ReadOnly);
                File.Delete(path);
                Debug.Log($"[SaveSystem] Successfully deleted profile: {profileName} at {path}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SaveSystem] Failed to delete profile: {profileName} at {path}. Error: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning($"[SaveSystem] Profile not found for deletion: {profileName} at {path}");
        }
    }
}