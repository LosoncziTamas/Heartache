using UnityEngine;

namespace Code
{
    public class RoomSpawner : MonoBehaviour
    {
        public Room.Opening Opening;
        
        public Collider2D Collider { get; private set; }
        
        public bool Processed { get; set; }

        private void Awake()
        {
            Collider = GetComponent<Collider2D>();
        }

        public void CloseOpening()
        {
            Debug.Log("Close opening for " + transform.parent.parent.name);
        }
    }
}
