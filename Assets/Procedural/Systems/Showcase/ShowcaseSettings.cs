using UnityEngine;

namespace Procedural.Showcase
{
    public class ShowcaseSettings : MonoBehaviour
    {
        private void Awake()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

}
