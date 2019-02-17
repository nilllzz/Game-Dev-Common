using System;
using Microsoft.Xna.Framework;

namespace GameDevCommon.Rendering
{
    public abstract class PerspectiveCamera : ICamera
    {
        private float _fov = 90f;
        protected float NearPlane { get; set; } = 0.01f;
        protected float FarPlane { get; set; } = 1000f;

        public event Action FOVChanged;

        public Matrix View { get; set; }
        public Matrix Projection { get; set; }
        public Vector3 Position { get; set; }
        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public float Roll { get; set; }
        public float FOV
        {
            get { return _fov; }
            set
            {
                _fov = value;
                FOVChanged?.Invoke();
            }
        }
        public float AspectRatio { get; protected set; }

        public PerspectiveCamera()
        {
            AspectRatio = GameInstanceProvider.Instance.GraphicsDevice.Viewport.AspectRatio;
        }

        protected virtual void CreateView()
        {
            var up = Vector3.Up;
            var forward = Vector3.Forward;

            // yaw:
            {
                forward.Normalize();
                forward = Vector3.Transform(forward, Matrix.CreateFromAxisAngle(up, Yaw));
            }

            // pitch:
            {
                forward.Normalize();
                var left = Vector3.Cross(up, forward);
                left.Normalize();

                forward = Vector3.Transform(forward, Matrix.CreateFromAxisAngle(left, -Pitch));
                up = Vector3.Transform(up, Matrix.CreateFromAxisAngle(left, -Pitch));
            }

            // roll:
            {
                up.Normalize();
                var left = Vector3.Cross(up, forward);
                left.Normalize();
                up = Vector3.Transform(up, Matrix.CreateFromAxisAngle(forward, Roll));
            }

            View = Matrix.CreateLookAt(Position, forward + Position, up);
        }

        protected virtual void CreateProjection()
        {
            Projection =
                Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(_fov), AspectRatio, NearPlane, FarPlane);
        }

        public Vector3 GetDirection()
        {
            var dir = Vector3.Forward;
            var rot = Matrix.CreateFromYawPitchRoll(Yaw, Pitch, Roll);
            return Vector3.Transform(dir, rot);
        }

        public abstract void Update();
    }
}
