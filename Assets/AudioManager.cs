using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public List<AudioSource> musicSources; 
    public List<AudioSource> sfxSources;   

    [Header("Audio Clips")]
    public List<AudioClip> musicClips;    
    public List<AudioClip> sfxClips;    

    private int musicIndex = 0; // Lưu vị trí AudioSource tiếp theo cho Music
    private int sfxIndex = 0;   // Lưu vị trí AudioSource tiếp theo cho SFX

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PlayMusic("background");
    }

    public void PlayMusic(string clipName, bool loop = true)
    {
        AudioClip clip = musicClips.Find(c => c.name == clipName);
        if (clip == null)
        {
            Debug.LogWarning("Music clip không tồn tại: " + clipName);
            return;
        }

        AudioSource source = GetAvailableSource(musicSources, ref musicIndex);
        source.clip = clip;
        source.loop = loop;
        source.Play();
    }

    public void PlaySFX(string clipName)
    {
        AudioClip clip = sfxClips.Find(c => c.name == clipName);
        if (clip == null)
        {
            Debug.LogWarning("SFX clip không tồn tại: " + clipName);
            return;
        }

        AudioSource source = GetAvailableSource(sfxSources, ref sfxIndex);
        source.clip = clip;
        source.loop = false;
        source.Play();
    }

    /// Lấy AudioSource rảnh, nếu không có thì chọn cái tiếp theo trong danh sách
    private AudioSource GetAvailableSource(List<AudioSource> sources, ref int index)
    {
        // Tìm source rảnh
        foreach (var src in sources)
        {
            if (!src.isPlaying)
                return src;
        }

        // Nếu tất cả bận, dùng vòng tròn
        AudioSource next = sources[index];
        index = (index + 1) % sources.Count;
        return next;
    }

    public void StopBackgroundMusic()
    {
        foreach (var source in musicSources)
        {
            if (source.isPlaying)
                source.Stop();
        }
    }

    public void PauseAudio()
    {
        foreach (var source in musicSources)
        {
            if (source.isPlaying)
                source.Pause();
        }

        foreach (var s in sfxSources)
        {
            if (s.isPlaying)
                s.Pause();
        }
    }

    public void ResumeAudio()
    {
        foreach (var source in musicSources)
        {
            if (source != null && !source.isPlaying && source.time > 0f)
                source.UnPause();
        }

        foreach (var s in sfxSources)
        {
            if (s != null && !s.isPlaying && s.time > 0f)
                s.UnPause();
        }
    }

}
