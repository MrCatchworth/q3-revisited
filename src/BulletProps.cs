using SFML.Graphics;

namespace JohnnySpaceGame {
    public struct BulletProps {
        public int Damage;
        public float Speed;
        public float Lifetime;
        public Texture Tex;
        public BulletProps(int _Damage, float _Speed, float _Lifetime, Texture _Tex) {
            Damage = _Damage;
            Speed = _Speed;
            Lifetime = _Lifetime;
            Tex = _Tex;
        }
    }
}