using SFML.Window;

namespace JohnnySpaceGame {
    public struct EngineFlareData {
        public readonly Vector2f Pos;
        public readonly float Radius;
        
        public EngineFlareData(Vector2f _Pos, float _Size) {
            Pos = _Pos;
            Radius = _Size;
        }
    }
}