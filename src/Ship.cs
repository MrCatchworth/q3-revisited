using System;
using SFML.Graphics;
using SFML.Window;

namespace JohnnySpaceGame {
    
    public class Ship : EncounterObject {
    
        public struct ControlsState {
            public float ForwardThrottle;
            public float TurningThrottle;
            public float StrafeThrottle;
            public bool TriggerPulled;
            public ControlsState (float ft, float tt, float st, bool tp) {
                ForwardThrottle = ft;
                TurningThrottle = tt;
                StrafeThrottle = st;
                TriggerPulled = tp;
            }
        };
            
        private readonly Sprite shipgfxSpr;
        
        public EncounterAI myAI {get; protected set;}
        public ControlsState MyControlsState {get; protected set;}
        
        public float ForwardSpeed {get; protected set;}
        public float TurningSpeed {get; protected set;}
        public float StrafeSpeed {get; protected set;}
        
        public readonly ShipClass MyClass;
        
        public readonly Faction MyFaction;
        
        public int Hull {get; protected set;}
        
        private WeaponSlot[] WeaponSlots;
        
        private CircleShape[] EngineFlareShapes;
        
        //RNG used to flicker the size+alpha of the engine flares
        private static Random efRandom = new Random();
        
        public Ship(Vector2f _pos, float _ang, ShipClass _class, EncounterAI _ai, Faction _faction) : base(_pos, _ang) {
            myAI = _ai;
            MyClass = _class;
            MyFaction = _faction;
            
            shipgfxSpr = new Sprite(MyClass.HullTexture);
            Vector2u texSize = MyClass.HullTexture.Size;
            shipgfxSpr.Origin = new Vector2f(texSize.X/2, texSize.Y/2);
            
            ForwardSpeed = 0f;
            TurningSpeed = 0f;
            StrafeSpeed = 0f;
            
            WeaponSlots = new WeaponSlot[MyClass.SlotTemplates.Length];
            for (int i=0; i<WeaponSlots.Length; i++) {
                WeaponSlots[i] = new WeaponSlot(this, MyClass.SlotTemplates[i]);
            }
            //dummy: just put in a random weapon
            foreach (WeaponSlot ws in WeaponSlots) {
                ws.FitWeapon(new Weapon(ws, mncTest));
            }
            
            myAI.SetOwner(this);
            
            Hull = MyClass.MaxHull;
            
            EngineFlareShapes = new CircleShape[MyClass.EngineFlares.Length];
            for (int i=0; i<EngineFlareShapes.Length; i++) {
                CircleShape newShape = new CircleShape(MyClass.EngineFlares[i].Radius, 10);
                newShape.FillColor = MyClass.EngineFlareColor;
                newShape.Origin = new Vector2f(newShape.Radius, newShape.Radius);
                EngineFlareShapes[i] = newShape;
            }
        }
        
        private static readonly Texture bullTex = new Texture("bestbullet.png");
        
        private static readonly WeaponClass mncTest = new WeaponClass(
            "LD-1 Malcontent Neutron Cannon",
            new BulletProps(10, 1500f, 0.7f, bullTex),
            1,
            0.15f
        );
            
        public override void Update(float dt) {
            MyControlsState = myAI.Update(dt);
            
            foreach (WeaponSlot ws in WeaponSlots) {
                ws.Update(dt, MyControlsState.TriggerPulled);
            }
            
            float targetForwardSpeed = MyControlsState.ForwardThrottle * MyClass.MaxForwardSpeed;
            float targetTurningSpeed = MyControlsState.TurningThrottle * MyClass.MaxTurningSpeed;
            float targetStrafeSpeed = MyControlsState.StrafeThrottle * MyClass.MaxStrafeSpeed;
            
            //calculate how much the speeds want to change this frame
            float forwardAccel = (targetForwardSpeed > ForwardSpeed ? 1 : -1) * MyClass.ForwardAccel * dt;
            float turningAccel = (targetTurningSpeed > TurningSpeed ? 1 : -1) * MyClass.TurningAccel * dt;
            float strafeAccel = (targetStrafeSpeed > StrafeSpeed ? 1 : -1) * MyClass.StrafeAccel * dt;
            
            //if we can reach the wanted speed this frame then do so, otherwise apply speed change and clamp
            if (Math.Abs(ForwardSpeed-targetForwardSpeed) <= Math.Abs(forwardAccel)) {
                ForwardSpeed = targetForwardSpeed;
            } else {
                ForwardSpeed += forwardAccel;
            }
            ForwardSpeed = Math.Max(Math.Min(ForwardSpeed, MyClass.MaxForwardSpeed), -1*MyClass.MaxForwardSpeed);
            
            if (Math.Abs(TurningSpeed-targetTurningSpeed) <= Math.Abs(turningAccel)) {
                TurningSpeed = targetTurningSpeed;
            } else {
                TurningSpeed += turningAccel;
            }
            TurningSpeed = Math.Max(Math.Min(TurningSpeed, MyClass.MaxTurningSpeed), -1*MyClass.MaxTurningSpeed);
            
            if (Math.Abs(StrafeSpeed-targetStrafeSpeed) <= Math.Abs(strafeAccel)) {
                StrafeSpeed = targetStrafeSpeed;
            } else {
                StrafeSpeed += strafeAccel;
            }
            StrafeSpeed = Math.Max(Math.Min(StrafeSpeed, MyClass.MaxStrafeSpeed), -1*MyClass.MaxStrafeSpeed);
            
            //got to read-modify-write because of Pos get/set
            Vector2f newPos = Pos;
            
            //apply forward speed
            newPos.X += (float)Math.Cos(Ang)*ForwardSpeed*dt;
            newPos.Y += (float)Math.Sin(Ang)*ForwardSpeed*dt;
            
            //apply turning speed
            Ang += TurningSpeed*dt;
            
            //apply strafing speed
            float halfpi = (float)Math.PI*0.5f;
            newPos.X += (float)Math.Cos(Ang+halfpi)*StrafeSpeed*dt;
            newPos.Y += (float)Math.Sin(Ang+halfpi)*StrafeSpeed*dt;
            
            Pos = newPos;
        }
        
        public override void Draw(RenderTarget rt) {
            float scaleMult = (float)efRandom.NextDouble()*0.4f + 0.8f;
            float efScale = (float)Math.Max((ForwardSpeed/MyClass.MaxForwardSpeed), 0f)*0.6f + 0.4f;
            efScale *= scaleMult;
            
            Color efColor = MyClass.EngineFlareColor;
            efColor.A = (byte)Math.Min(efScale*255f, 255f);
            for (int i=0; i<EngineFlareShapes.Length; i++) {
                EngineFlareShapes[i].Position = LocalToEncounter(MyClass.EngineFlares[i].Pos);
                EngineFlareShapes[i].Scale = new Vector2f(efScale,efScale);
                EngineFlareShapes[i].FillColor = efColor;
                rt.Draw(EngineFlareShapes[i]);
            }
            
            shipgfxSpr.Position = Pos;
            shipgfxSpr.Rotation = (Ang/Util.fPi)*180f;
            rt.Draw(shipgfxSpr);
        }
        
        public override void TakeDamage(int damage) {
            Hull -= damage;
            if (Hull <= 0) {
                Destroy();
            }
        }
        
        public override bool CollidesPoint(Vector2f point) {
            Vector2f dif = point-Pos;
            return (dif.X*dif.X)+(dif.Y*dif.Y) <= MyClass.CollideRadius*MyClass.CollideRadius;
        }
        
    }
}