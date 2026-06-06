using UnityEngine;
using UnityEngine.UIElements;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("Audio Clip")]
    public AudioClip bakcground;
    public AudioClip death;
    public AudioClip shoot;
    public AudioClip run;
    public AudioClip takeDamage;
    public AudioClip hitGround;
    public AudioClip enemyTakeDamage;
    public AudioClip enemyDie;
    public AudioClip enemyShoot;

    private void Start()
    {
        musicSource.clip = bakcground;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}
