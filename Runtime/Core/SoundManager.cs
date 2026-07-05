using System.Collections.Generic;
using UnityEngine;

namespace RedMinS
{
    public class SoundManager : SingletonMonobehaviour<SoundManager>
    {
        [Header("Audio Sources")]
        [SerializeField] AudioSource bgmAudio;
        [SerializeField] AudioSource[] soundAudios;
        [SerializeField] AudioSource alertAudio;

        public float volumeOfSound { private set; get; }
        AudioClip _curBgm = null;

        Dictionary<string, AudioClip> _clipRegistry;


        protected override void OnSingletonAwake()
        {
            base.OnSingletonAwake();

            _clipRegistry = new Dictionary<string, AudioClip>();
            ChangeSoundVolume(PlayerPrefs.GetFloat(ConfigString.F_SoundVolume, 1f));
        }

        public void RegisterClip(string label, AudioClip clip)
        {
            _clipRegistry[label] = clip;
        }

        public void ChangeSoundVolume(float volume)
        {
            volumeOfSound = volume;
            PlayerPrefs.SetFloat(ConfigString.F_SoundVolume, volumeOfSound);

            if (soundAudios != null)
            {
                for (int i = 0; i < soundAudios.Length; ++i)
                {
                    if (soundAudios[i] != null) soundAudios[i].volume = volumeOfSound;
                }
            }
            if (alertAudio != null) alertAudio.volume = volumeOfSound;
        }

        public void PlayEffectSound(string label)
        {
            if (!_clipRegistry.TryGetValue(label, out var clip))
            {
                Debug.LogWarning($"[SoundManager] Sound not found: {label}");
                return;
            }

            PlayEffectSound(clip);
        }

        public void PlayEffectSound(AudioClip clip)
        {
            if (soundAudios == null || soundAudios.Length == 0)
            {
                Debug.LogWarning("[SoundManager] soundAudios not assigned");
                return;
            }

            AudioSource audio = GetEmptySoundAudio();
            audio.clip = clip;
            audio.Play();
        }

        AudioSource GetEmptySoundAudio()
        {
            int largestIndex = 0;
            float largestProgress = 0;
            for (int i = 0; i < soundAudios.Length; i++)
            {
                if (!soundAudios[i].isPlaying)
                {
                    return soundAudios[i];
                }

                float progress = soundAudios[i].time / soundAudios[i].clip.length;
                if (progress > largestProgress)
                {
                    largestIndex = i;
                    largestProgress = progress;
                }
            }
            return soundAudios[largestIndex];
        }
    }
}
