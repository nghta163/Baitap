using UnityEngine;
using UnityEngine.EventSystems;

namespace BlockBlast
{
    public class SliderReleaseSound : MonoBehaviour, IPointerUpHandler
    {
        public string SoundName = "sfx_place";

        public void OnPointerUp(PointerEventData e)
        {
            AudioManager.Play(SoundName);
        }
    }
}
