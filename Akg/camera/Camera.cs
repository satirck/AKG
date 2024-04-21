using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace _3dObjViewr.camera
{
    public class Camera
    {
        public Vector3 target { get; set; }
        public Vector3 position { get; set; }

        public readonly Vector3 up = Vector3.UnitY;

        public float zNear { get; set; }
        public float zFar { get; set; }
        public float angle { get; set; }

        public int width { get; set; }
        public int height { get; set; }

        public Camera(Vector3 trg, Vector3 pos, float zNear, float zFar, float angle, int width, int height)
        {
            this.target = target;
            this.position = pos;
            this.zNear = zNear;
            this.zFar = zFar;
            this.angle = angle;
            this.width = width;
            this.height = height;
        }

        public Matrix4x4[] GetMatrix4X4s()
        {
            //up
            var viewDirection = Vector3.Normalize(position - target);
            var upDirection = Vector3.UnitY;
            var rightDirection = Vector3.Normalize(Vector3.Cross(upDirection, viewDirection));
            var perpendicular = Vector3.Cross(viewDirection, rightDirection);

            float aspectRatio = (float)width / height;

            var viewMatrix = Matrix4x4.CreateLookAt(position, target, perpendicular);
            var projectionMatrix =
                Matrix4x4.CreatePerspectiveFieldOfView(angle, aspectRatio, zNear, zFar);

            //0 is viewMatrix and 1 is projection matrix
            return [viewMatrix, projectionMatrix];
        }

    }
}