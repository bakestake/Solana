using UnityEngine;

namespace Gamegaard.CursorSystem
{
    [System.Serializable]
    public struct CursorState<T>
    {
        [SerializeField] private string name;
        [SerializeField] private T[] frames;
        [SerializeField] private Vector2 hotspot;
        [SerializeField] private float fps;

        public CursorState(string name, T[] frames, Vector2 hotspot, float fps = 4)
        {
            this.name = name;
            this.frames = frames ?? new T[0];
            this.hotspot = hotspot;
            this.fps = fps;
        }

        public readonly bool IsAnimated => frames.Length > 0;
        public readonly string Name => name;
        public readonly T[] Frames => frames;
        public readonly Vector2 Hotspot => hotspot;
        public readonly int FrameCount => frames?.Length ?? 1;
        public readonly float AnimationSpeed => IsAnimated ? 1f / fps : 0f;

        public readonly T GetFrame(int index) => IsAnimated ? frames[index % FrameCount] : frames[0];
    }
}