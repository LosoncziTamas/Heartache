using System.Collections.Generic;
using UnityEngine;

namespace Code
{
    public class LevelGenerator : MonoBehaviour
    {
        private static readonly ContactFilter2D NoFilter = new ContactFilter2D().NoFilter();
        
        [SerializeField] private RoomPrefabs _roomPrefabs;
        [SerializeField] private Room _entryRoom;

        private readonly Collider2D[] _contacts = new Collider2D[8];

        private int _counter;

        private void Start()
        {
            GenerateRooms();
        }
        
        private void GenerateRooms()
        {
            var roomsToProcess = new List<Room> { _entryRoom };
            while (roomsToProcess.Count > 0 && _counter < 100)
            {
                _counter++;
                var room = roomsToProcess[0];
                roomsToProcess.RemoveAt(0);
                var newRooms = GenerateNeighbourRooms(room);
                roomsToProcess.AddRange(newRooms);
            }
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
                        break;
                    }
                    
                    if (contact.gameObject.CompareTag(Tags.Spawner))
                    {
                        var otherSpawner = contact.gameObject.GetComponent<RoomSpawner>();
                        if (!otherSpawner.Spawned)
                        {
                            Debug.Log("merge");
                            // TODO: merge spawners
                        }
                    }
                }

                spawner.Spawned = true;
                
                if (skip)
                {
                    continue;
                }

                var roomPrefab = _roomPrefabs.GetRandomRoomWithOpening(spawner.Opening);
                var instance = Instantiate(roomPrefab, spawner.transform.position, Quaternion.identity);
                newRooms.Add(instance);
            }

            return newRooms;
        }
    }
}
