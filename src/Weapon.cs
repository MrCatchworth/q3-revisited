using SFML.Graphics;
using System;

namespace JohnnySpaceGame {
    /*
    a weapon of a certain design (class) fitted on a certain slot, with a certain cooldown between shots
    the slot tells it whether or not it's shooting
    */
    public class Weapon {
        public readonly WeaponSlot SlotFitted;
        public readonly WeaponClass MyClass;
        
        private float Cooldown;
        
        public Weapon(WeaponSlot _SlotFitted, WeaponClass _class) {
            SlotFitted = _SlotFitted;
            MyClass = _class;
            
            Cooldown = 0f;
        }
        
        public void Update(float dt, bool triggerPulled) {
            if (Cooldown > 0) {
                Cooldown -= dt;
            } else if (triggerPulled) {
                Cooldown = MyClass.ShotInterval;
                new Bullet(SlotFitted.GetShootPos(), SlotFitted.GetShootAngle(), MyClass.ShotProps, SlotFitted.Owner).Location = SlotFitted.Owner.Location;
                //new Bullet(SlotFitted.Owner.LocalToEncounter(SlotFitted.PosOnShip), SlotFitted.Owner.Ang, MyClass.ShotProps, SlotFitted.Owner).Location = SlotFitted.Owner.Location;
            }
        }
    }
}