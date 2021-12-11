using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Rooms
{
    public class Room : MonoBehaviour
    {
        public static Room FocusedRoom { get; private set; }
        
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
        public bool CameraIsMoving { get; private set; }
        public bool HasKey { get; private set; }
        public bool HasEnemy { get; private set; }
        public bool HasExit => _exit != null;
        public List<RoomSpawner> RoomSpawners { get; } = new List<RoomSpawner>();
        
        private Exit _exit;

        private CheckOpeningTraversable[] _openingTraversables;
        
        private void Awake()
        {
            RoomSpawners.AddRange(GetComponentsInChildren<RoomSpawner>());
            _openingTraversables = GetComponentsInChildren<CheckOpeningTraversable>();
        }
        
        public void SpawnEnemy()
        {
            var enemy = Resources.Load("Enemy");
            Instantiate(enemy, transform);
            HasEnemy = true;
        }

        public void SpawnKey()
        {
            var key = Resources.Load("Key");
            Instantiate(key, transform);
            HasKey = true;
        }
        
        public void SpawnExit()
        {
            var exitPrefab = Resources.Load<Exit>("Exit");
            _exit = Instantiate(exitPrefab, transform);
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
                if (FocusedRoom != this)
                {
                    if (FocusedRoom != null)
                    {
                        FocusedRoom.RevokeFocus();
                    }
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
                if (FocusedRoom != this)
                {
                    var heroPos = other.transform.position;
                    var currentRoomDiff = Vector3.Distance(FocusedRoom.transform.position, heroPos);
                    var myRoomDiff = Vector3.Distance(transform.position, heroPos);
                    if (myRoomDiff < currentRoomDiff)
                    {
                        FocusedRoom.RevokeFocus();
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
        }

        private void RevokeFocus()
        {
            CameraIsMoving = false;
        }
    }
}