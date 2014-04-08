using SFML.Window;
using System;

namespace JohnnySpaceGame {
    /*
    contains data about a particular class, or design, of weapon
    */
    public class WeaponClass {
        //what the bullets shot by this weapon design are like
        public readonly BulletProps ShotProps;
        //relative scale of 'weight' used to restrict what slots can fit this weapon design
        public readonly int Weight;
        //minimum seconds between shots
        public readonly float ShotInterval;
        //pretty-print name
        public readonly string Name;
        
        public WeaponClass(string _name, BulletProps _bps, int _weight, float _interval) {
            Name = _name;
            ShotProps = _bps;
            Weight = _weight;
            ShotInterval = _interval;
        }
    }
}