using SFML.Window;
using SFML.Graphics;
using System;

namespace JohnnySpaceGame {
    public abstract class EncounterObject {
    
        /*
        converts a point in the ship's local coordinates to global (encounter) coordinates)
        */
        public Vector2f LocalToEncounter(Vector2f localPos) {
            Vector2f retVal = new Vector2f();
            float rightang = Ang+Util.fHalfPi;
            retVal.X = Pos.X + (float)Math.Cos(Ang)*localPos.X + (float)Math.Cos(rightang)*localPos.Y;
            retVal.Y = Pos.Y + (float)Math.Sin(Ang)*localPos.X + (float)Math.Sin(rightang)*localPos.Y;
            return retVal;
        }
        
        public Vector2f Pos {get; set;}
        
        /*
        angle that self-normalizes between 0 and 2Pi
        */
        private float ang;
        public float Ang {
            get {
                return ang;
            }
            set {
                ang = value;
                if (ang>Util.f2Pi) {
                    Console.WriteLine("Clamp upper");
                    ang = ang % Util.f2Pi;
                }
                else if (ang<0) {
                    Console.WriteLine("Clamp lower");
                    ang += Util.f2Pi;
                }
            }
        }
        
        /*
        whether or not the object 'exists', this should be used whenever another object wants to interact with it
        */
        //private bool alive;
        public bool Alive {get; private set;}
        
        /*
        the encounter that this object exists in
        please use this instead of using Encounter.Add/Remove, as Location takes care of the bookkeeping itself
        */
        private Encounter location;
        public Encounter Location {
            get {
                return location;
            }
            set {
                if (location != null) location.Remove(this);
                if (value != null) value.Add(this);
                location = value;
            }
        }
        
        //update the state of the object for the new frame
        public abstract void Update(float dt);
        //perform drawing operations for this object
        public abstract void Draw(RenderTarget rt);
        //take some damage
        public virtual void TakeDamage(int damage) {
        }
        //return whether the object logically intersects a point
        public abstract bool CollidesPoint(Vector2f point);
        
        //set alive to false, and clean up ourselves from location
        public void Destroy() {
            Alive = false;
            Location = null;
        }
        
        public EncounterObject(Vector2f _Pos, float _Ang) {
            Pos = _Pos;
            Ang = _Ang;
            Alive = true;
            Location = null;
        }
        
    }
}