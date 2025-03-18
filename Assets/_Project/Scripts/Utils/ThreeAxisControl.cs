using System;
using UnityEngine;

namespace AS.Utils
{
    [Serializable]
    public class ThreeAxisControl
    {
        private Vector3 _data;

        public ThreeAxisControl(Vector3 yawPitchRoll)
        {
            _data = yawPitchRoll;
        }

        public float Pitch
        {
            get => _data.y;
            set => _data.y = value;
        }

        public float Yaw
        {
            get => _data.x;
            set => _data.x = value;
        }

        public float Roll
        {
            get => _data.z;
            set => _data.z = value;
        }

        public static implicit operator ThreeAxisControl(Vector3 vec)
        {
            return new ThreeAxisControl(vec);
        }

        public static implicit operator Vector3(ThreeAxisControl yawPitchRoll)
        {
            return yawPitchRoll._data;
        }
    }
}