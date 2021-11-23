using System;
using UnityEngine;

namespace Code
{
    public class RoomSpawner : MonoBehaviour
    {
        public Room.Opening Opening { get; set; }
        
        public Collider2D Collider { get; private set; }

        private void Awake()
        {
            Collider = GetComponent<Collider2D>();
        }
    }
}
