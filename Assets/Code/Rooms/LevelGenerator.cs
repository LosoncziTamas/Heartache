using System;
using System.Collections.Generic;
using System.Linq;
using Code.Common;
using Code.Gui;
using Code.Hero;
using Code.Objects;
using UnityEngine;
using Compass = Code.Hero.Compass;

namespace Code.Rooms
{
    public class LevelGenerator : MonoBehaviour
    {
        public List<Room> Rooms { get; } = new List<Room>();
        
        [SerializeField] private RoomPrefabs[] _roomPrefabs;
        [SerializeField] private Room _entryRoomPrefab;
        [SerializeField] private BoolReference _playerEscaped;

        private readonly Collider2D[] _contacts = new Collider2D[8];
        
        private RoomPrefabs _currentLevelRoomPrefabs;
        private int _currentRoomPrefabIdx = 0;
        private Room _entryRoom;
        private Room _roomWithExit;

        private void Awake()
        {
            _playerEscaped.Variable.Value = false;
            _currentLevelRoomPrefabs = _roomPrefabs[_currentRoomPrefabIdx];
            _currentLevelRoomPrefabs.Init();
        }

        private void Start()
        {
            GenerateLevel(calledFromStart: true);
        }

        public void RestartGame(Transform hero)
        {
            _playerEscaped.Variable.Value = false;
            _currentRoomPrefabIdx = 0;
            _currentLevelRoomPrefabs = _roomPrefabs[_currentRoomPrefabIdx];
            _currentLevelRoomPrefabs.Init();
            
            foreach (var room in Rooms)
            {
                var roomGo = room.gameObject;
                roomGo.SetActive(false);
                Destroy(roomGo);
            }

            _entryRoom = Instantiate(_entryRoomPrefab, Vector3.zero, Quaternion.identity, transform);
            hero.position = Vector3.zero;
            Chest.CollectedChestCount = 0;
            Rooms.Clear();
            Rooms.Add(_entryRoom);
            
            GenerateRooms();
            SetupRoomObjects();
            
            Countdown.Instance.StartCountDown(Rooms.Count * 10f);
            Compass.Instance.Setup();
        }
        
        private void SetupRoomObjects()
        {
            var roomCount = Rooms.Count;
            var enemyCount = Mathf.RoundToInt(GlobalProperties.Instance.EnemyToRoomRatio * roomCount);
            for (var i = 0; i < enemyCount; i++)
            {
                var randomRoom = Rooms.GetRandomElement();
                while (randomRoom == _entryRoom)
                {
                    randomRoom = Rooms.GetRandomElement();
                }
                randomRoom.SpawnEnemy();
            }
            
            var trapCount = Mathf.RoundToInt(GlobalProperties.Instance.TrapToRoomRatio * roomCount);
            for (var i = 0; i < trapCount; i++)
            {
                var randomRoom = Rooms.GetRandomElement();
                while (randomRoom == _entryRoom)
                {
                    randomRoom = Rooms.GetRandomElement();
                }
                randomRoom.SpawnTrap();
            }       
            
            var randomTilePerRoomCount = GlobalProperties.Instance.RandomTilePerRoomCount;
            foreach (var room in Rooms)
            {
                room.SpawnTorches();
                for (var i = 0; i < randomTilePerRoomCount; i++)
                {
                    room.SpawnRandomTile();
                }
            }
            
            for (var i = 0; i < GlobalProperties.Instance.KeyCountPerLevel; i++)
            {
                var randomRoom = Rooms.GetRandomElement();
                while (randomRoom.HasChest || randomRoom == _entryRoom)
                {
                    randomRoom = Rooms.GetRandomElement();
                }
                randomRoom.SpawnChest();
            }
        }
        
        public void GenerateLevel(bool calledFromStart = false)
        {
            PrepareForNextLevel(incrementProgress: !calledFromStart);
            GenerateRooms();
            SetupRoomObjects();

            if (!calledFromStart)
            {
                Countdown.Instance.StartCountDown(Rooms.Count * 10f);
            }
            Compass.Instance.Setup();
        }

        private void PrepareForNextLevel(bool incrementProgress)
        {
            if (incrementProgress)
            {
                _currentLevelRoomPrefabs.CompletionCount++;
            }
            
            if (_currentLevelRoomPrefabs.CompletionCount >= _currentLevelRoomPrefabs.MinLevelToComplete)
            {
                var newIdx = Math.Min(_roomPrefabs.Length - 1, _currentRoomPrefabIdx + 1);
                _currentLevelRoomPrefabs = _roomPrefabs[newIdx];
                if (newIdx > _currentRoomPrefabIdx)
                {
                    _currentLevelRoomPrefabs.Init();
                }
                _currentRoomPrefabIdx = Math.Min(_roomPrefabs.Length - 1, _currentRoomPrefabIdx + 1);
            }

            if (_roomWithExit != null)
            {
                _roomWithExit.RemoveExit();
                _entryRoom = Instantiate(_entryRoomPrefab, _roomWithExit.transform.position, Quaternion.identity, transform);
                _roomWithExit = null;
            }
            else
            {
                _entryRoom = Instantiate(_entryRoomPrefab, Vector3.zero, Quaternion.identity, transform);
            }
            
            foreach (var room in Rooms)
            {
                var roomGo = room.gameObject;
                roomGo.SetActive(false);
                Destroy(roomGo);
            }
            
            Chest.CollectedChestCount = 0;
            Rooms.Clear();
            Rooms.Add(_entryRoom);
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

            _roomWithExit = Rooms.Last();
            var finalExit = GlobalProperties.Instance.LevelsToCompleteCount == _currentRoomPrefabIdx + 1;
            _roomWithExit.SpawnExit(finalExit);
            
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
                var overlapCount = spawner.Collider.OverlapCollider(PhysicsUtils.NoFilter, _contacts);
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

                var roomPrefab = _currentLevelRoomPrefabs.GetRandomRoomWithOpening(spawner.Opening);
                var instance = Instantiate(roomPrefab, spawner.transform.position, Quaternion.identity, transform);
                instance.parentSpawner = spawner;
                instance.name = $"{newRooms.Count + Rooms.Count} {instance.name}"; 
                newRooms.Add(instance);
            }

            return newRooms;
        }
    }
}
