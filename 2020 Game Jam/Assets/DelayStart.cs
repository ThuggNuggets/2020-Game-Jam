using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class DelayStart : MonoBehaviour
{
    public AudioSource shout;
    public AudioSource music;
    public float delayTime;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(playSound());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator playSound()
    {
        yield return new WaitForSeconds(delayTime);
        CameraShaker.Instance.ShakeOnce(7f, 6f, 0.5f, 0.5f);
        shout.Play();
        music.Play();
    }
}
