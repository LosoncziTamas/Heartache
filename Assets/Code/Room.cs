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

        public RoomSpawner parentSpawner;

        private bool _entryRoom;
        
        public bool EntryRoom
        {
            set
            {
                _entryRoom = value;
                if (value)
                {
                    SpawnPlayer();
                }
            }
            get => _entryRoom;
        }

        private bool _exitRoom;
        
        public bool ExitRoom
        {
            set
            {
                _exitRoom = value;
                if (value)
                {
                    SpawnExit();
                }
            }
        }

        private void SpawnPlayer()
        {
            var renderers = GetComponentsInChildren<SpriteRenderer>();
            foreach (var spriteRenderer in renderers)
            {
                spriteRenderer.color = Color.green;
            }
        }

        private void SpawnExit()
        {
            var renderers = GetComponentsInChildren<SpriteRenderer>();
            foreach (var spriteRenderer in renderers)
            {
                spriteRenderer.color = Color.red;
            }
        }
        
        public List<RoomSpawner> RoomSpawners { get; } = new List<RoomSpawner>();

        private void Awake()
        {
            RoomSpawners.AddRange(GetComponentsInChildren<RoomSpawner>());
        }
    }
}