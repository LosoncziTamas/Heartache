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
        
        private void Start()
        {
            GenerateRooms();
            var roomCount = Rooms.Count;
            for (var i = 0; i < roomCount / 2; i++)
            {
                Rooms.GetRandomElement().SpawnEnemy();
            }
        }

        private void GenerateRooms()
        {
            var roomsToProcess = new List<Room> { _entryRoom };
            while (roomsToProcess.Count > 0)
            {
                var room = roomsToProcess[0];
                roomsToProcess.RemoveAt(0);
                var newRooms = GenerateNeighbourRooms(room);
                roomsToProcess.AddRange(newRooms);
                Rooms.AddRange(newRooms);
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
                            otherSpawner.Processed = true;
                        }
                    }
                }

                spawner.Processed = true;
                
                if (skip)
                {
                    continue;
                }

                var roomPrefab = _roomPrefabs.GetRandomRoomWithOpening(spawner.Opening);
                var instance = Instantiate(roomPrefab, spawner.transform.position, Quaternion.identity, transform);
                instance.parentSpawner = spawner;
                instance.name = $"{_counter++} {instance.name}"; 
                newRooms.Add(instance);
            }

            return newRooms;
        }
    }
}
