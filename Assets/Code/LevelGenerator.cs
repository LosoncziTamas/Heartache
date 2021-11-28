using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code
{
    public class LevelGenerator : MonoBehaviour
    {
        private static readonly ContactFilter2D NoFilter = new ContactFilter2D().NoFilter();
        
        [SerializeField] private RoomPrefabs _roomPrefabs;
        [SerializeField] private Room _entryRoom;

        private readonly Collider2D[] _contacts = new Collider2D[8];
        public List<Room> Rooms { get; } = new List<Room>();

        private int _counter;
        private Color _color;
        
        private void Start()
        {
            GenerateRooms();
        }

        private void GenerateRooms()
        {
            _color = Color.white;
            Rooms.Add(_entryRoom);
            _entryRoom.EntryRoom = true;
            var roomsToProcess = new List<Room> { _entryRoom };
            while (roomsToProcess.Count > 0)
            {
                var room = roomsToProcess[0];
                roomsToProcess.RemoveAt(0);
                var newRooms = GenerateNeighbourRooms(room);
                roomsToProcess.AddRange(newRooms);
                Rooms.AddRange(newRooms);
                _color = Color.HSVToRGB(Random.value, Random.value, Random.value);
            }

            Rooms.Last().ExitRoom = true;
        }

        private List<Room> GenerateNeighbourRooms(Room startRoom)
        {
            var newRooms = new List<Room>();
            
            foreach (var spawner in startRoom.RoomSpawners)
            {
                var overlapCount = spawner.Collider.OverlapCollider(NoFilter, _contacts);
                var skip = false;
                for (var contactIndex = 0; contactIndex < overlapCount; ++contactIndex)
                {
                    var contact = _contacts[contactIndex];
                    if (contact.gameObject.CompareTag(Tags.Room))
                    {
                        skip = true;
                    }
                    
                    if (contact.gameObject.CompareTag(Tags.Spawner))
                    {
                        var otherSpawner = contact.gameObject.GetComponent<RoomSpawner>();
                        if (!otherSpawner.Processed)
                        {
                            Debug.Log("set " + otherSpawner.transform.parent.name + " spawner " + otherSpawner.transform.parent.parent.name + " processed");
                            otherSpawner.Processed = true;
                        }
                    }
                }

                Debug.Log(startRoom.name + " spawner " + spawner.transform.parent.name + " processed");
                spawner.Processed = true;
                
                if (skip)
                {
                    // Close open wall
                    continue;
                }

                var roomPrefab = _roomPrefabs.GetRandomRoomWithOpening(spawner.Opening);
                var instance = Instantiate(roomPrefab, spawner.transform.position, Quaternion.identity, transform);
                instance.parentSpawner = spawner;
                /*var renderers = instance.GetComponentsInChildren<SpriteRenderer>();
                foreach (var spriteRenderer in renderers)
                {
                    spriteRenderer.color = _color;
                }*/
                instance.name = $"{_counter++} {instance.name}"; 
                newRooms.Add(instance);
            }

            return newRooms;
        }
    }
}
