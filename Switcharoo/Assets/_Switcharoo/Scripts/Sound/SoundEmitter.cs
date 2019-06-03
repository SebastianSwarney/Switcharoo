using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEmitter : MonoBehaviour
{
    public List<SoundList> m_sounds;

    Dictionary<string, string> m_allSounds;

    private void Awake()
    {
        m_allSounds = new Dictionary<string, string>();
        foreach(SoundList currentSound in m_sounds)
        {
            m_allSounds.Add(currentSound.m_soundType, currentSound.m_soundPath);
        }
    }
    public void PlaySound(string p_soundType)
    {
        if (m_allSounds.ContainsKey(p_soundType))
        {
            string path = m_allSounds[p_soundType];
            Debug.Log("Play Sound: " + path);
        }
        else
        {
            Debug.Log("Error! " + gameObject.name + " does not contain sound type: " + p_soundType);
        }
    }
}

[System.Serializable]
public struct SoundList
{
    public string m_soundType;
    [FMODUnity.EventRef]
    public string m_soundPath;
}
