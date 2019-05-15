using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using System;

public class AudioObject : MonoBehaviour
{
    public AudioSource audioSource;
    public bool isPlaying;
    public Subject<float> AudioObservable;
    public int ClipIndex;
    public bool isLoop;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(this.audioSource.clip != null 
            && !this.audioSource.loop 
            && this.isPlaying)
        {
            if(!this.audioSource.isPlaying)
            {
                //clip is not playing anymore...
                //observable complete
                Debug.Log(this.gameObject.name + " is complete");
                this.isPlaying = false;
                AudioObservable.OnCompleted();
            }
            else 
            {
                AudioObservable.OnNext(audioSource.time);
            }
        }
    }

    public void Load(SoundQueue soundClip)
    {
        var clip = Resources.Load<AudioClip>("Audio/" + soundClip.FileLoc);
        this.audioSource.clip = clip;
        this.audioSource.loop = soundClip.IsRepeat;
        if(soundClip.Volume.HasValue) 
        {
            this.audioSource.volume = soundClip.Volume.Value;
        }
        this.AudioObservable = new Subject<float>();
        this.AudioObservable.OnNext(this.audioSource.clip.length);

        if(soundClip.IsRepeat)
        { 
            this.AudioObservable.OnCompleted();
        }

        this.Play();
    }

    public void Play()
    {
        this.isPlaying = true;
        this.audioSource.Play();
        this.audioSource.enabled = true;
        //return AudioObservable.AsObservable();
    }

    public void Pause()
    {
        this.isPlaying = false;
        this.audioSource.Pause();
    }

    public void Stop()
    {
        this.isPlaying = false;
        this.audioSource.Stop();
    }
}
