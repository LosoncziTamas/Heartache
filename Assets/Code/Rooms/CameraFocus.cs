using System.Collections;
using UnityEngine;

namespace Code.Rooms
{
    [RequireComponent(typeof(Room))]
    public class CameraFocus : MonoBehaviour
    {
        private static readonly Color LitColor = new Color(1, 245.0f / 255.0f, 218.0f / 255.0f);
        private const float LightAnimDuration = 0.6f;

        private static CameraFocus _focusedRoom;
        public static CameraFocus FocusedRoom
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
            }
        }

        private Room _room;

        private void Awake()
        {
            _room = GetComponent<Room>();
        }

        public bool CameraIsMoving { get; private set; }
        
        private IEnumerator AnimateLightOn()
        {
            var accumulated = 0f;
            var t = 0f;
            while (accumulated < LightAnimDuration)
            {
                foreach (var light2D in _room.Lights)
                {
                    light2D.color = Color.Lerp(Color.black, LitColor, t);
                }
                accumulated += Time.deltaTime;
                t = (accumulated / LightAnimDuration);
                yield return null;
            }
        }
        
        private IEnumerator AnimateLightOff()
        {
            var accumulated = 0f;
            var t = 0f;
            while (accumulated < LightAnimDuration)
            {
                foreach (var light2D in _room.Lights)
                {
                    light2D.color = Color.Lerp(light2D.color, Color.black, t);
                }
                accumulated += Time.deltaTime;
                t = (accumulated / LightAnimDuration);
                yield return null;
            }
            TurnLights(false);
        }
        
        private void TurnLights(bool on)
        {
            foreach (var light2D in _room.Lights)
            {
                if (light2D)
                {
                    light2D.enabled = on;
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

        private void OnTriggerStay2D(Collider2D other)
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

        private void RevokeFocus()
        {
            CameraIsMoving = false;
        }
    }
}