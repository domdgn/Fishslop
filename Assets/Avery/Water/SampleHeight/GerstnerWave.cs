using UnityEngine;

namespace FishSlop
{
    [System.Serializable]
    public struct GerstnerWave
    {
        public float steepness;
        public float wavelength;
        public float angle;
        public float phaseOffset;

        public Vector3 GetDisplacement(Vector3 worldPosition, float time)
        {
            float radians = angle * Mathf.Deg2Rad;

            float sinAngle = Mathf.Sin(radians);
            float cosAngle = Mathf.Cos(radians);

            float k = 2f * Mathf.PI / wavelength;

            float positionDotDirection =
                sinAngle * worldPosition.x +
                cosAngle * worldPosition.z;

            float frequency = Mathf.Sqrt(9.81f / k);

            float theta =
                positionDotDirection * k
                - time * frequency
                + phaseOffset;

            float sinTheta = Mathf.Sin(theta);
            float cosTheta = Mathf.Cos(theta);

            float x = steepness * cosAngle * sinTheta;
            float y = steepness * sinTheta;
            float z = steepness * sinAngle * sinTheta;

            return new Vector3(x, y, z);
        }
    }
}