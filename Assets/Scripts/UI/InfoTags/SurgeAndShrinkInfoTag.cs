using TMPro;
using UnityEngine;

namespace ECellDive.UI
{
    public class SurgeAndShrinkInfoTag : MonoBehaviour
    {
        public Animation anim;
        public TextMeshProUGUI refInfoTagText;

        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        public void SurgeAndShrink(string _text)
        {
            if (anim.isPlaying)
            {
                anim.Stop();
            }
            gameObject.SetActive(true);
            refInfoTagText.text = _text;
            anim.Play("SurgeAndShrink");
        }
    }
}

