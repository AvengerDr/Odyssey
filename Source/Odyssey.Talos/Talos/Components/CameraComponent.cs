using System.ComponentModel;
using Odyssey.Animations;
using Odyssey.Engine;
using Odyssey.Graphics;
using SharpDX;
using System;
using SharpYaml;
using SharpYaml.Serialization;

namespace Odyssey.Talos.Components
{
    public enum CameraType
    {
        Perspective,
        Orthographic,
        Stereo
    }

    public class CameraComponent : Component, ICamera
    {
        public int Index { get; set; }
        public float NearClip { get; set; }
        public float FarClip { get; set; }
        public float FieldOfView { get; set; }
        public CameraType Type { get; set; }

        public Vector3 Up { get; set; }

        public Matrix View { get; internal set; }
        public Matrix Projection { get; internal set; }
        public ViewportF Viewport { get; set; }

        public Vector3 Direction
        {
            get { return new Vector3(-View.M13, -View.M23, -View.M33); }
        }

        public Vector3 AxisX
        {
            get { return new Vector3(View.M11, View.M21, View.M31); }
        }

        public Vector3 AxisY
        {
            get { return new Vector3(View.M12, View.M22, View.M32); }
        }

        public Vector3 AxisZ
        {
            get { return new Vector3(View.M13, View.M23, View.M33); }
        } 

        public CameraComponent() : base(ComponentTypeManager.GetType<CameraComponent>())
        {
            Index = 0;
            NearClip = 1.0f;
            FarClip = 1000.0f;
            FieldOfView = (float) (Math.PI/4);
            Up = Vector3.UnitY;
            Type = CameraType.Perspective;
        }


    }
}
