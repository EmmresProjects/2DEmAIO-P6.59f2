using UnityEngine;

   public class AudioManager : MonoBehaviour
   {
       public static AudioManager instance;
       public AudioSource shootSound;
       public AudioSource explosionSound;

       void Awake()
       {
           if (instance == null) instance = this;
           else Destroy(gameObject);
       }

       public void PlayShootSound()
       {
           shootSound.Play();
       }

       public void PlayExplosionSound()
       {
           explosionSound.Play();
       }
   }