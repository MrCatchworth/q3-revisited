using SFML.Graphics;
using SFML.Window;

namespace JohnnySpaceGame {
    /*
    a big readonly object containing data about a particular class, or design, of ship
    */
    public class ShipClass {
        public readonly string Name;
        
        public readonly float MaxForwardSpeed;
        public readonly float MaxTurningSpeed;
        public readonly float MaxStrafeSpeed;

        public readonly float ForwardAccel;
        public readonly float TurningAccel;
        public readonly float StrafeAccel;
        
        public readonly int MaxHull;
        
        public readonly WeaponSlot.Template[] SlotTemplates;
        
        public readonly Texture HullTexture;
        public readonly float CollideRadius;
        
        public readonly EngineFlareData[] EngineFlares;
        public readonly Color EngineFlareColor;
        
        public ShipClass(   string _name,
        
                            float _mfs,
                            float _mts,
                            float _mss,
                            
                            float _fa,
                            float _ta,
                            float _sa,
                            
                            int _mh,
                            
                            WeaponSlot.Template[] _wslots,
                            
                            Texture _hullt,
                            float _collRadius,
                            
                            EngineFlareData[] _eflares,
                            Color _efc
                        ) {
            Name = _name;
            
            MaxForwardSpeed = _mfs;
            MaxTurningSpeed = _mts;
            MaxStrafeSpeed = _mss;
            
            ForwardAccel = _fa;
            TurningAccel = _ta;
            StrafeAccel = _sa;
            
            MaxHull = _mh;
            
            SlotTemplates = _wslots;
            
            HullTexture = _hullt;
            CollideRadius = _collRadius;
            
            EngineFlares = _eflares;
            EngineFlareColor = _efc;
        }
    }
}