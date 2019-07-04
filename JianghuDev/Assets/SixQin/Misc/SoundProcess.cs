using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine.SceneManagement;

public class SoundProcess : MonoBehaviour
{
    /// <summary>
    /// 背景音乐音量大小
    /// </summary>
    public static float m_fDefaultMusicVolume = 1.0f;
    /// <summary>
    /// 音效音量大小
    /// </summary>
    public static float m_fDefaultSoundVolume = 0.8f;
    /// <summary>
    /// 是否震动
    /// </summary>
    public static bool m_IsShake = true;
    /// <summary>
    /// 是否开启方言
    /// </summary>
    public static bool m_IsDialect = false;


    public static float m_fMusicVolume = m_fDefaultMusicVolume;
    public static float m_fSoundVolume = m_fDefaultSoundVolume;
    //---------------------------------------------------------------------------
    private static int m_iAudioPlayCount = 20;
    private static Dictionary<string, AudioClip> m_hsSoundObjDB = new Dictionary<string, AudioClip>();
    private static List<AudioSource> m_AllAudios = new List<AudioSource>();
    private static List<AudioSource> m_aAudioPlayList = new List<AudioSource>();
    private static List<AudioSource> m_aMusicPlayList = new List<AudioSource>();
    private static List<AudioClip> m_SoundNameQueue = new List<AudioClip>();
    private static AudioSource m_SoundListAudio = null;
    private static string m_szNowMusicName = "";
    private static string m_szSetMusicName = "";
    private static int m_iNowMusicIndex = 0;
    private static float m_iNowMusicVolume = 0;
    private static float m_fGradualChange = 0;
    public static string m_sMusicPath = "Music/";
    public static string m_sSoundPath = "Sound/";
    //
    public static float SoundVolume { get { return m_fSoundVolume; } }
    public static float MusicVolume { get { return m_fMusicVolume; } }

    public static bool ShakeModel { get { return m_IsShake; } }

    public static bool DialectModel { get { return m_IsDialect; } }
    /// <summary>
    /// 初始化音效管理器
    /// </summary>
    public static void Create()
    {
        GameObject go = new GameObject();
        go.AddComponent<SoundProcess>();
        go.AddComponent<AudioListener>();
        go.name = "AudioManager";
        LoadConfig();
        GameObject.DontDestroyOnLoad(go);
    }


    public static void LoadConfig()
    {
        m_fMusicVolume = PlayerPrefs.GetFloat("MusicVolume", m_fDefaultMusicVolume);
        m_fSoundVolume = PlayerPrefs.GetFloat("SoundVolume", m_fDefaultSoundVolume);
        m_IsShake = PlayerPrefs.GetInt("ShakeModel", m_IsShake ? 0 : 1) == 0 ? true : false;         //0开   1关
        m_IsDialect = PlayerPrefs.GetInt("DialectModel", m_IsDialect ? 0 : 1) == 0 ? true : false;

    }


    public static void SaveConfig()
    {
        PlayerPrefs.SetFloat("MusicVolume", m_fMusicVolume);
        PlayerPrefs.SetFloat("SoundVolume", m_fSoundVolume);
    }


    public static void StopAllAudios()
    {
        foreach (AudioSource audio in m_AllAudios)
            audio.Stop();
    }

    public static void PlaySound(string szSoundName, float rate = 1.0f)
    {
        if (szSoundName == "")
            return;
        SQDebug.Log(szSoundName);

        AudioClip ClipData = GetSoundClip(m_sSoundPath, szSoundName);

        if (ClipData == null)
            return;

        for (int i = 0; i < m_aAudioPlayList.Count; i++)
        {
            AudioSource AudioObj = m_aAudioPlayList[i];
            if (AudioObj.enabled == false)
            {
                AudioObj.enabled = true;
                AudioObj.clip = ClipData;
                AudioObj.volume = m_fSoundVolume * rate;
                AudioObj.Play();
                return;
            }
        }
    }
    //若是要播放連續多個聲音組合(用逗號分隔),則將其餘音效名稱放到queue.
    public static void PlaySoundList(string szSoundNameList)
    {
        if (szSoundNameList == "")
            return;

        AudioClip clipData = null;

        if (szSoundNameList.IndexOf(",") >= 0)
        {
            m_SoundNameQueue.Clear();
            string[] nameList = szSoundNameList.Split(","[0]);

            for (int i = 0; i < nameList.Length; i++)
            {
                clipData = GetSoundClip(m_sSoundPath, nameList[i]);
                if (clipData != null)
                    m_SoundNameQueue.Add(clipData);
            }
        }
        if (m_SoundNameQueue.Count > 0)
        {
            clipData = m_SoundNameQueue[0];
            m_SoundNameQueue.RemoveAt(0);
            m_SoundListAudio.clip = clipData;
            m_SoundListAudio.volume = m_fSoundVolume;
            m_SoundListAudio.Play();
        }
    }

    public static void StopSoundList()
    {
        m_SoundListAudio.Stop();
        m_SoundNameQueue.Clear();
    }
    //判断声音队列是否播放完毕.
    public static bool IsSoundListPlaying()
    {
        if (m_SoundNameQueue.Count > 0)
            return true;
        else if (m_SoundListAudio.isPlaying)
            return true;
        return false;
    }

    public static void StopSound(string szSoundName)
    {
        if (szSoundName == "")
            return;

        AudioClip ClipData = GetSoundClip(m_sSoundPath, szSoundName);

        if (ClipData == null)
            return;

        for (int i = 0; i < m_aAudioPlayList.Count; i++)
        {
            AudioSource AudioObj = m_aAudioPlayList[i];
            if (AudioObj.enabled == true && AudioObj.clip == ClipData && AudioObj.isPlaying == true)
            {
                AudioObj.Stop();
                AudioObj.enabled = false;
                AudioObj.clip = null;
                return;
            }
        }
    }

    public static void PlayMusic(string szMusicName)
    {
        if (szMusicName == "")
            return;

        if (m_szNowMusicName == szMusicName)
            return;
        if (m_szNowMusicName == "") {
            m_szNowMusicName = szMusicName;
        }
        m_szNowMusicName = szMusicName;
        m_szSetMusicName = szMusicName;
        m_fGradualChange = 1.0f;
        AudioSource AudioObj = m_aMusicPlayList[m_iNowMusicIndex];

        m_iNowMusicVolume = AudioObj.volume;

        if (m_iNowMusicIndex < m_aMusicPlayList.Count)
        {
            AudioObj = m_aMusicPlayList[m_iNowMusicIndex];
            AudioObj.clip = GetSoundClip(m_sMusicPath, m_szSetMusicName);
            AudioObj.Play();
            AudioObj.volume = 0.5f;
        }
        //
        SoundProcess.SetMusicVolume(m_fMusicVolume);
    }

    public static void ChangeMusicVolume()
    {
        AudioSource AudioObj = m_aMusicPlayList[m_iNowMusicIndex];
        AudioObj.volume = m_fMusicVolume;
    }

    /// <summary>
    /// 设置音效音量
    /// </summary>
    /// <param name="vol"></param>
    public static void SetEffectVolume(float vol)
    {
        m_fSoundVolume = vol;
        PlayerPrefs.SetFloat("SoundVolume", m_fSoundVolume);
    }

    /// <summary>
    /// 设置背景音乐音量大小
    /// </summary>
    /// <param name="vol"></param>
    public static void SetMusicVolume(float vol)
    {
        m_fMusicVolume = vol;
        AudioSource AudioObj = m_aMusicPlayList[m_iNowMusicIndex];
        AudioObj.volume = m_fMusicVolume;
        //
        PlayerPrefs.SetFloat("MusicVolume", m_fMusicVolume);
    }

    /// <summary>
    /// 设置是否震动
    /// </summary>
    /// <param name="isShake"></param>
    public static void SetShakeModel(bool isShake)
    {
        m_IsShake = isShake;
        int shakeValue = m_IsShake == true ? 0 : 1;
        PlayerPrefs.SetInt("ShakeModel", shakeValue);
    }
    /// <summary>
    /// 设置是否方言
    /// </summary>
    /// <param name="isShake"></param>
    public static void SetDialectModel(bool isDialect)
    {
        m_IsDialect = isDialect;
        int dialectValue = m_IsDialect == true ? 0 : 1;
        PlayerPrefs.SetInt("DialectModel", dialectValue);
    }
    public static void StopAllSound()
    {
        for (int i = 0; i < m_aAudioPlayList.Count; i++)
        {
            AudioSource AudioObj = m_aAudioPlayList[i];
            AudioObj.Stop();
            AudioObj.enabled = false;
            AudioObj.clip = null;
        }
    }

    /// <summary>
    /// 使用震动
    /// </summary>
    public static void PlayShake()
    {
#if UNITY_ANDROID
        Handheld.Vibrate();
#endif
    }

    void Awake()
    {
        int OpenBGM = PlayerPrefs.GetInt("OpenBGM", 1);
        if (OpenBGM == 0)
        {
            m_fMusicVolume = 0;
        }
        else
        {
            m_fMusicVolume = m_fDefaultMusicVolume;
        }

        int OpenSoundEffect = PlayerPrefs.GetInt("OpenSoundEffect", 1);
        if (OpenSoundEffect == 0)
        {
            m_fSoundVolume = 0;
        }
        else
        {
            m_fSoundVolume = m_fDefaultSoundVolume;
        }

        m_AllAudios.Clear();
        m_hsSoundObjDB.Clear();
        m_aAudioPlayList.Clear();
        m_aMusicPlayList.Clear();
        m_SoundNameQueue.Clear();
        m_SoundListAudio = null;

        int i = 0;
        for (i = 0; i < m_iAudioPlayCount; i++)
        {
            AudioSource AudioObj = gameObject.AddComponent<AudioSource>();
            AudioObj.loop = false;
            AudioObj.enabled = false;
            AudioObj.spatialBlend = 0;//2d音效.
            m_aAudioPlayList.Add(AudioObj);
            m_AllAudios.Add(AudioObj);
        }

        for (i = 0; i < 2; i++)
        {
            AudioSource MusicObj = gameObject.AddComponent<AudioSource>();
            MusicObj.loop = true;
            MusicObj.spatialBlend = 0;//2d音效.
            m_aMusicPlayList.Add(MusicObj);
            m_AllAudios.Add(MusicObj);
        }
        m_SoundListAudio = gameObject.AddComponent<AudioSource>();
        m_SoundListAudio.loop = false;
        m_SoundListAudio.spatialBlend = 0;//2d音效.
        m_AllAudios.Add(m_SoundListAudio);
    }

    void Update()
    {
        AudioListUpdate();
        AudioUpdate();
       // MusicUpdate();
    }

    public static AudioClip GetSoundClip(string type, string szSoundName)
    {
        AudioClip clipData = null;
        if (!m_hsSoundObjDB.ContainsKey(szSoundName))
        {
            SQDebug.Log("播放音效路径为:" + type + szSoundName);
            clipData = Resources.Load<AudioClip>(type + szSoundName);

            if (clipData != null)
                m_hsSoundObjDB.Add(szSoundName, clipData);
            else
                return clipData;
        }
        else
        {
            clipData = m_hsSoundObjDB[szSoundName];
        }
        return clipData;
    }

    void MusicUpdate()
    {
        if (m_szSetMusicName != m_szNowMusicName)
        {
            int iSID = 0;
            if (m_iNowMusicIndex == 0)
                iSID = 1;

            AudioSource lastAudioObj = m_aMusicPlayList[m_iNowMusicIndex];
            AudioSource curAudioObj = m_aMusicPlayList[iSID];

            m_fGradualChange -= (Time.deltaTime * 1.2f);

            if (m_fGradualChange > 0.0f)
            {
                curAudioObj.volume = m_fMusicVolume * (1.0f - m_fGradualChange);
                lastAudioObj.volume = m_iNowMusicVolume * m_fGradualChange;
            }
            else
            {
                if (curAudioObj.volume != m_fMusicVolume)
                    curAudioObj.volume = m_fMusicVolume;

                if (lastAudioObj.volume != 0)
                    lastAudioObj.volume = 0.0f;

                lastAudioObj.Stop();
                lastAudioObj.clip = null;
                m_iNowMusicIndex = iSID;
                m_szNowMusicName = m_szSetMusicName;
            }
        }
    }

    void AudioUpdate()
    {
        for (int i = 0; i < m_aAudioPlayList.Count; i++)
        {
            AudioSource AudioObj = m_aAudioPlayList[i];
            if (AudioObj.enabled == true && AudioObj.isPlaying == false)
            {
                AudioObj.enabled = false;
                AudioObj.clip = null;
            }
        }
    }

    void AudioListUpdate()
    {
        if (m_SoundListAudio.isPlaying)
            return;

        if (m_SoundNameQueue.Count > 0)
        {
            AudioClip ClipData = m_SoundNameQueue[0];
            m_SoundNameQueue.RemoveAt(0);
            m_SoundListAudio.clip = ClipData;
            m_SoundListAudio.volume = m_fSoundVolume;
            m_SoundListAudio.Play();
        }
    }



}
