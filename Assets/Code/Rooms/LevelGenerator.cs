using System.Collections.Generic;
using System.Linq;
using Code.Gui;
using Code.Hero;
using UnityEngine;

namespace Code.Rooms
{
    public class LevelGenerator : MonoBehaviour
    {
        [SerializeField] private int _maxRoom;
        
        private static readonly ContactFilter2D NoFilter = new ContactFilter2D().NoFilter();
        
        [SerializeField] private RoomPrefabs _roomPrefabs;
        [SerializeField] private Room _entryRoom;

        private readonly Collider2D[] _contacts = new Collider2D[8];
        public List<Room> Rooms { get; } = new List<Room>();
        
        private Room _roomWithExit;

        private int _counter;
        
        private void Start()
        {
            GenerateLevel();
        }
        
        public void GenerateLevel()
        {
            PrepareForNextLevel();
            GenerateRooms();
            var roomCount = Rooms.Count;
            var enemyCount = Mathf.RoundToInt(GlobalProperties.Instance.EnemyToRoomRatio * roomCount);
            for (var i = 0; i < enemyCount; i++)
            {
                var randomRoom = Rooms.GetRandomElement();
                while (randomRoom.HasEnemy)
                {
                    randomRoom = Rooms.GetRandomElement();
                }
                randomRoom.SpawnEnemy();
            }

            for (var i = 0; i < GlobalProperties.Instance.KeyCount; i++)
            {
                var randomRoom = Rooms.GetRandomElement();
                while (randomRoom.HasKey)
                {
                    randomRoom = Rooms.GetRandomElement();
                }
                randomRoom.SpawnKey();
            }
            
            Countdown.Instance.StartCountDown(Rooms.Count * 10f);
            KeyCompass.Instance.Setup();
        }

        private void PrepareForNextLevel()
        {
            if (_roomWithExit != null)
            {
                _roomWithExit.RemoveExit();
                _entryRoom = _roomWithExit;
                _roomWithExit = null;
            }

            foreach (var room in Rooms)
            {
                if (room != _entryRoom)
                {
                    Destroy(room.gameObject);
                }
            }

            Key.CollectedKeyCount = 0;
            Rooms.Clear();
        }
        
        private void GenerateRooms()
        {
            var roomsToProcess = new List<Room> { _entryRoom };
            while (roomsToProcess.Count > 0 && _maxRoom > Rooms.Count)
            {
                var room = roomsToProcess[0];
                roomsToProcess.RemoveAt(0);
                var newRooms = GenerateNeighbourRooms(room);
                roomsToProcess.AddRange(newRooms);
                Rooms.AddRange(newRooms);
            }

            _roomWithExit = Rooms.Last();
            _roomWithExit.SpawnExit();

            foreach (var room in Rooms)
            {
                room.CloseNonTraversableOpenings();
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
                    }

                    if (contact.gameObject.CompareTag(Tags.Spawner))
                    {
                        var otherSpawner = contact.gameObject.GetComponent<RoomSpawner>();
                        otherSpawner.Processed = true;
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
