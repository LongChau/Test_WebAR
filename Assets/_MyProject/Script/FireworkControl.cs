using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WebAR.Effect
{
    public class FireworkControl : MonoBehaviour
    {
        public ParticleSystem _firework1;
        public ParticleSystem _firework2;
        public ParticleSystem _firework3;

        public float _firework2Time;
        public float _firework3Time;

        public TextMesh _txtInfo;

        public bool IsPlaying { get; private set; }

        // Start is called before the first frame update
        void Start()
        {

        }

        [ContextMenu("PlayFirework")]
        public void PlayFirework()
        {
            _firework1.Play();
            _firework2.Play();
            _firework3.Play();
            //StartCoroutine(IEPlayFirework(_firework2Time, _firework3Time));
        }

        public void StopFirework()
        {
            _firework1.Stop();
            _firework2.Stop();
            _firework3.Stop();
        }

        IEnumerator IEPlayFirework(float time2, float time3)
        {
            IsPlaying = true;
            //_firework1.gameObject.SetActive(true);
            _firework1.Play();
            yield return new WaitForSecondsRealtime(time2);
            //_firework2.gameObject.SetActive(true);
            _firework2.Play();
            yield return new WaitForSecondsRealtime(time3);
            //_firework3.gameObject.SetActive(true);
            _firework3.Play();
            yield return new WaitForSecondsRealtime(_firework3.main.duration);
            IsPlaying = false;
        }

        private void Update()
        {
            _txtInfo.text = $"Firework: {transform.position}";
        }
    }
}
