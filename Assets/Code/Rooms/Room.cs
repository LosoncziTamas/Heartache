using System;
using System.Collections;
using System.Collections.Generic;
using Code.Enemy;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using Random = UnityEngine.Random;

namespace Code.Rooms
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
        public bool HasKey { get; private set; }
        public List<RoomSpawner> RoomSpawners { get; } = new List<RoomSpawner>();
        
        private Exit _exit;
        private Trap _trapPrefab;
        private RandomObject _randomObjectPrefab;
        private GameObject _torchPrefab;
        private List<Trap> _traps;
        private List<Vector3> _occupiedTilePositions = new List<Vector3>();
        private CheckOpeningTraversable[] _openingTraversables;
        
        private void Awake()
        {
            RoomSpawners.AddRange(GetComponentsInChildren<RoomSpawner>());
            _openingTraversables = GetComponentsInChildren<CheckOpeningTraversable>();
        }
        
        public void SpawnEnemy()
        {
            var enemy = Resources.Load<EnemyController>("Enemy");
            var enemyInstance = Instantiate(enemy, transform);
            PlaceObjectAtRandomPosition(enemyInstance.transform);
        }

        public void SpawnKey()
        {
            var key = Resources.Load<Key>("Key");
            var keyInstance = Instantiate(key, transform);
            HasKey = true;
            PlaceObjectAtRandomPosition(keyInstance.transform);
        }

        public void SpawnRandomTile()
        {
            if (_randomObjectPrefab == null)
            {
                _randomObjectPrefab = Resources.Load<RandomObject>("Random Object");
            }
            var instance = Instantiate(_randomObjectPrefab, transform);
            PlaceObjectAtRandomPosition(instance.transform, 5, -3);
        }
        
        public void SpawnTrap()
        {
            if (_trapPrefab == null)
            {
                _trapPrefab = Resources.Load<Trap>("Trap");
            }
            var trap = Instantiate(_trapPrefab, transform);
            PlaceObjectAtRandomPosition(trap.transform);
        }

        public List<Light2D> Lights { get; private set; } = new List<Light2D>();

        public void SpawnTorches()
        {
            if (_torchPrefab == null)
            {
                _torchPrefab = Resources.Load<GameObject>("Torch");
            }

            var upperLeftPos = new Vector3(-3.9f, 4.25f, 0f);
            var upperLeftTorch = Instantiate(_torchPrefab, transform);
            upperLeftTorch.transform.localPosition = upperLeftPos;
            
            var upperRightPos = new Vector3(3.9f, 4.25f, 0f);
            var upperRightTorch = Instantiate(_torchPrefab, transform);
            upperRightTorch.transform.localPosition = upperRightPos;
            
            var lowerRightPos = new Vector3(3.9f, -3.85f, 0f);
            var lowerRightTorch = Instantiate(_torchPrefab, transform);
            lowerRightTorch.transform.localPosition = lowerRightPos;

            var lowerLeftPos = new Vector3(-3.9f, -3.85f, 0f);
            var lowerLeftTorch = Instantiate(_torchPrefab, transform);
            lowerLeftTorch.transform.localPosition = lowerLeftPos;

            Lights = new List<Light2D>
            {
                upperLeftTorch.GetComponentInChildren<Light2D>(),
                upperRightTorch.GetComponentInChildren<Light2D>(),
                lowerRightTorch.GetComponentInChildren<Light2D>(),
                lowerLeftTorch.GetComponentInChildren<Light2D>(),
            };

            TurnLights(on: false);
        }
        
        private void TurnLights(bool on)
        {
            foreach (var light2D in Lights)
            {
                if (light2D)
                {
                    light2D.enabled = on;
                }
            }
        }

        private void PlaceObjectAtRandomPosition(Transform objectTransform, int maxTilePos = 4, int minTilePos =- 2)
        {
            var randomPos = GetRandomTilePosition(minTilePos, maxTilePos);
            while (_occupiedTilePositions.Contains(randomPos))
            {
                randomPos = GetRandomTilePosition(minTilePos, maxTilePos);
            }
            objectTransform.localPosition = randomPos;
            _occupiedTilePositions.Add(objectTransform.localPosition);
        }

        private static Vector3 GetRandomTilePosition(int min, int max)
        {
            var randomPosX = Random.Range(min, max);
            var randomPosY = Random.Range(min, max);
            return new Vector3(randomPosX, randomPosY, 0f);
        }
        
        public void SpawnExit(bool finalExit)
        {
            var exitPrefab = Resources.Load<Exit>("Exit");
            _exit = Instantiate(exitPrefab, transform);
            _exit.FinalExit = finalExit;
            PlaceObjectAtRandomPosition(_exit.transform);
        }

        public void RemoveExit()
        {
            if (_exit)
            {
                Destroy(_exit.gameObject);
                _exit = null;
            }
        }
        
        public void CloseNonTraversableOpenings()
        {
            foreach (var traversable in _openingTraversables)
            {
                if (!traversable.OpeningIsTraversable())
                {
                    traversable.CloseOpening();
                }
            }
        }
    }
}