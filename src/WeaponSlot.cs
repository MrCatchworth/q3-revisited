using SFML.Graphics;
using SFML.Window;
using System;

namespace JohnnySpaceGame {
    /*
    a slot on a ship which may or may not have a weapon fitted on it
    receives Update call from the owning ship and passes it down to the fitted weapon
    */
    public class WeaponSlot {
        protected Weapon FittedWeapon;
        
        //where the slot is on the ship, in local coordinates
        public readonly Vector2f PosOnShip;
        //the ship on which the slot exists
        public readonly Ship Owner;
        
        //weapons below this weight can't be fitted on this slot
        public readonly int WeaponMinWeight;
        //weapons above this weight can't be fitted on this slot
        public readonly int WeaponMaxWeight;
        
        public WeaponSlot(Ship _Owner, Template templ) {
            Owner = _Owner;
            PosOnShip = templ.PosOnShip;
            WeaponMinWeight = templ.WeaponMinWeight;
            WeaponMaxWeight = templ.WeaponMaxWeight;
            FittedWeapon = null;
        }
        
        public virtual void Update(float dt, bool triggerPulled) {
            if (FittedWeapon != null) {
                FittedWeapon.Update(dt, triggerPulled);
            }
        }
        
        /*
        (try to) fit a weapon onto the slot, throws an exception if it's incompatible
        */
        public Weapon FitWeapon(Weapon newWeapon) {
            if (newWeapon.MyClass.Weight < WeaponMinWeight || newWeapon.MyClass.Weight > WeaponMaxWeight) {
                throw new FittingException("Outsize Weapon");
            }
            Weapon oldWeapon = FittedWeapon;
            FittedWeapon = newWeapon;
            return oldWeapon;
        }
        
        /*
        return the encounter coordinates that the bullet would come from if the weapon fired right now
        */
        public virtual Vector2f GetShootPos() {
            return Owner.LocalToEncounter(PosOnShip);
        }
        
        /*
        return the angle a bullet would point at if the weapon fired right now
        not very interesting here, but it's used more in TurretWeaponSlot
        */
        public virtual float GetShootAngle() {
            return Owner.Ang;
        }
        
        /*
        contains the data required to initialize a weapon slot
        this struct is used on ship classes to specify what slots are on ships of that design
        */
        public struct Template {
            public readonly Vector2f PosOnShip;
            public readonly int WeaponMinWeight;
            public readonly int WeaponMaxWeight;
            public Template (Vector2f _PosOnShip, int _minw, int _maxw) {
                PosOnShip = _PosOnShip;
                WeaponMinWeight = _minw;
                WeaponMaxWeight = _maxw;
            }
        }
        
        /*
        thrown when we try to fit an outsize weapon on this slot
        */
        public class FittingException : Exception {
            public FittingException() : base() {
            }
            public FittingException(string msg) : base(msg) {
            }
            public FittingException(string msg, Exception ie) : base(msg, ie) {
            }
        }
    }
}