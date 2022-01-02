using System;
using System.Collections.Generic;
using Code.Common;
using UnityEngine;
using static Code.Rooms.Room.Opening;

namespace Code.Rooms
{
    [CreateAssetMenu(fileName = "Room Prefabs", menuName = "Scriptable Objects/Room Prefabs", order = 1)]
    public class RoomPrefabs : ScriptableObject
    {
        [SerializeField] private List<Room> _rooms;
        [SerializeField] private int _minLevelToComplete;
        public int MinLevelToComplete => _minLevelToComplete;
        public int CompletionCount { get; set; }

        private List<Room> _perfabsWithLeftOpening = new List<Room>();
        private List<Room> _perfabsWithRightOpening = new List<Room>();
        private List<Room> _perfabsWithBottomOpening = new List<Room>();
        private List<Room> _perfabsWithTopOpening = new List<Room>();

        public void Init()
        {
            CompletionCount = 0;
            _perfabsWithLeftOpening.Clear();
            _perfabsWithTopOpening.Clear();
            _perfabsWithRightOpening.Clear();
            _perfabsWithBottomOpening.Clear();
            foreach (var room in _rooms)
            {
                if (room.opening.HasFlag(Left))
                {
                    _perfabsWithLeftOpening.Add(room);
                }
                if (room.opening.HasFlag(Top))
                {
                    _perfabsWithTopOpening.Add(room);
                }
                if (room.opening.HasFlag(Right))
                {
                    _perfabsWithRightOpening.Add(room);
                }
                if (room.opening.HasFlag(Bottom))
                {
                    _perfabsWithBottomOpening.Add(room);
                }
            }
        }

        public Room GetRandomRoomWithOpening(Room.Opening opening)
        {
            if (opening.HasFlag(Left))
            {
                return _perfabsWithLeftOpening.GetRandomElement();
            }
            if (opening.HasFlag(Top))
            {
                return _perfabsWithTopOpening.GetRandomElement();
            }
            if (opening.HasFlag(Right))
            {
                return _perfabsWithRightOpening.GetRandomElement();
            }
            if (opening.HasFlag(Bottom))
            {
                return _perfabsWithBottomOpening.GetRandomElement();
            }

            throw new InvalidOperationException();
        }
    }
}