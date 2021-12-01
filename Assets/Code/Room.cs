using System;
using System.Collections;
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
            
        }

        private void SpawnExit()
        {
            var exitPrefab = Resources.Load<Exit>("Exit");
            Instantiate(exitPrefab, transform);
        }
        
        public List<RoomSpawner> RoomSpawners { get; } = new List<RoomSpawner>();

        private void Awake()
        {
            RoomSpawners.AddRange(GetComponentsInChildren<RoomSpawner>());
        }
        
        
        // TODO: extract to separate component
        private static Room FocusedRoom { get; set; }
        private bool _cameraIsMoving;

        private IEnumerator MoveCamera(Camera cameraToMove, Vector3 target, float duration)
        {
            var accumulated = 0f;
            var startPos = cameraToMove.transform.position;
            _cameraIsMoving = true;
            while (accumulated < duration && _cameraIsMoving)
            {
                var t = Mathf.Clamp01(accumulated / duration);
                cameraToMove.transform.position = Vector3.Lerp(startPos, target, t);
                accumulated += Time.deltaTime;
                yield return null;
            }
            _cameraIsMoving = false;
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
                        StartCoroutine(MoveCamera(cameraToMove, target, 1.0f));
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
                            StartCoroutine(MoveCamera(cameraToMove, target, 1.0f));
                        }
                    }
                }
            }
        }

        private void RevokeFocus()
        {
            _cameraIsMoving = false;
        }
    }
}