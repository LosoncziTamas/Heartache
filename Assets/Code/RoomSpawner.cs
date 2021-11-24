using System;
using UnityEngine;

namespace Code
{
    public class RoomSpawner : MonoBehaviour
    {
        public Room.Opening Opening;
        
        public Collider2D Collider { get; private set; }
        
        public bool Spawned { get; set; }

        private void Awake()
        {
            Collider = GetComponent<Collider2D>();
        }
    }
}
