using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code
{
    public class Room : MonoBehaviour
    {
        [Flags, Serializable]
        public enum Opening
        {
            None = 0,
            Left = 1 << 1,
            Top = 1 << 2,
            Right = 1 << 3,
            Bottom = 1 << 4
        }

        public Opening opening;

        private List<RoomSpawner> _roomSpawners;
        
        

    }
}