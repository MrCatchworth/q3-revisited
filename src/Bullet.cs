using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;

namespace JohnnySpaceGame {
    /*
    an object existing in encounters shot from a Weapon
    it travels forward and dies when it reaches its max range (age >= lifetime)
    or when it hits a collideable object (shooting ship exempt)
    */
    public class Bullet : EncounterObject {
        private readonly Sprite BulletSprite;
        
        public readonly BulletProps Props;
        
        public readonly Ship Shooter;
        
        private float Age;
        
        public Bullet(Vector2f _Pos, float _Ang, BulletProps _Props, Ship _Shooter) : base(_Pos, _Ang) {
            Props = _Props;
            Shooter = _Shooter;

            BulletSprite = new Sprite(Props.Tex);
            Vector2u texSize = Props.Tex.Size;
            BulletSprite.Origin = new Vector2f((float)texSize.X/2f, (float)texSize.Y/2f);
        }
        
        public override void Update(float dt) {
            Age += dt;
            if (Age >= Props.Lifetime) {
                Destroy();
                return;
            }
            
            Vector2f newPos = Pos;
            newPos.X += (float)Math.Cos(Ang) * Props.Speed * dt;
            newPos.Y += (float)Math.Sin(Ang) * Props.Speed * dt;
            Pos = newPos;
            
            foreach (EncounterObject so in Location) {
                if (so != Shooter && so.Alive && so.CollidesPoint(Pos)) {
                    so.TakeDamage(Props.Damage);
                    Destroy();
                    return;
                }
            }
        }
        
        public override void Draw(RenderTarget rt) {
            BulletSprite.Position = Pos;
            BulletSprite.Rotation = (Ang/Util.fPi)*180f;
            rt.Draw(BulletSprite);
        }
        
        public override bool CollidesPoint(Vector2f point) {
            return false;
        }
    }
}