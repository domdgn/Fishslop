using UnityEngine;

namespace FishSlop
{
    public class Billboard : MonoBehaviour
    {
        private Camera cam;

        private void LateUpdate()
        {
            if (cam == null)
            {
                cam = Camera.main;
                if (cam == null) return;
            }

            transform.rotation = cam.transform.rotation;
        }
    }
}