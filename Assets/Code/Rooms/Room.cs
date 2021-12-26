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
        
        private static readonly Color LitColor = new Color(1, 245.0f / 255.0f, 218.0f / 255.0f);

        private static Room _focusedRoom;
        public static Room FocusedRoom
        {
            get => _focusedRoom;
            private set
            {
                if (_focusedRoom)
                {
                    _focusedRoom.StopCoroutine(_focusedRoom.AnimateLightOn());
                    _focusedRoom.StopCoroutine(_focusedRoom.AnimateLightOff());
                    _focusedRoom.StartCoroutine(_focusedRoom.AnimateLightOff());

                }
                _focusedRoom = value;
                _focusedRoom.TurnLights(on: true);
                _focusedRoom.StartCoroutine(_focusedRoom.AnimateLightOn());
                Debug.Log("Focused room: " + _focusedRoom.gameObject.name);
            }
        }
        
        public Opening opening;
        public RoomSpawner parentSpawner;
        public bool CameraIsMoving { get; private set; }
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

        private List<Light2D> _lights = new List<Light2D>();

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

            _lights = new List<Light2D>
            {
                upperLeftTorch.GetComponentInChildren<Light2D>(),
                upperRightTorch.GetComponentInChildren<Light2D>(),
                lowerRightTorch.GetComponentInChildren<Light2D>(),
                lowerLeftTorch.GetComponentInChildren<Light2D>(),
            };

            TurnLights(on: false);
        }

        private IEnumerator AnimateLightOn()
        {
            var accumulated = 0f;
            var duration = 1.0f;
            var t = 0f;
            while (accumulated < duration)
            {
                foreach (var light2D in _lights)
                {
                    light2D.color = Color.Lerp(Color.black, LitColor, t);
                }
                accumulated += Time.deltaTime;
                t = (accumulated / duration);
                yield return null;
            }
        }
        
        private IEnumerator AnimateLightOff()
        {
            var accumulated = 0f;
            var duration = 1.0f;
            var t = 0f;
            while (accumulated < duration)
            {
                foreach (var light2D in _lights)
                {
                    light2D.color = Color.Lerp(light2D.color, Color.black, t);
                }
                accumulated += Time.deltaTime;
                t = (accumulated / duration);
                yield return null;
            }
            TurnLights(false);
        }

        private void TurnLights(bool on)
        {
            foreach (var light2D in _lights)
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

        private IEnumerator MoveCamera(Camera cameraToMove, Vector3 target, float duration)
        {
            var accumulated = 0f;
            var startPos = cameraToMove.transform.position;
            CameraIsMoving = true;
            while (accumulated < duration && CameraIsMoving)
            {
                var t = Mathf.Clamp01(accumulated / duration);
                cameraToMove.transform.position = Vector3.Lerp(startPos, target, GlobalProperties.Instance.CameraMovementCurve.Evaluate(t));
                accumulated += Time.deltaTime;
                yield return null;
            }
            CameraIsMoving = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag(Tags.Player))
            {
                DetermineFocusedRoom(other.transform.position);
            }
        }

        private void DetermineFocusedRoom(Vector3 heroPos)
        {
            if (FocusedRoom != this)
            {
                var currentRoomDiff = FocusedRoom != null ? Vector3.Distance(FocusedRoom.transform.position, heroPos) : float.MaxValue;
                var myRoomDiff = Vector3.Distance(transform.position, heroPos);
                if (myRoomDiff < currentRoomDiff)
                {
                    FocusedRoom?.RevokeFocus();
                    FocusedRoom = this;
                    var cameraToMove = Camera.main;
                    if (cameraToMove != null)
                    {
                        var pos = transform.position;
                        var target = new Vector3(pos.x, pos.y, cameraToMove.transform.position.z);
                        StartCoroutine(MoveCamera(cameraToMove, target, GlobalProperties.Instance.CameraMovementDuration));
                    }
                }
            }
        }
        
        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.CompareTag(Tags.Player))
            {
                DetermineFocusedRoom(other.transform.position);
            }
        }

        private void RevokeFocus()
        {
            Debug.Log("Revoke focus: " + gameObject.name);
            CameraIsMoving = false;
        }
    }
}