using System;
using SFML.Window;
using System.Collections.Generic;

namespace JohnnySpaceGame {
    /*
    an object on a ship which controls what it does during an encounter
    */
    public abstract class EncounterAI {
        protected Ship Owner;
        
        public EncounterAI() {
            Owner = null;
        }
        
        public EncounterAI(Ship _Owner) {
            SetOwner(_Owner);
        }
        
        public void SetOwner(Ship _Owner) {
            Owner = _Owner;
        }
        
        public abstract Ship.ControlsState Update(float dt);
    }
    
    public class DummyAI : EncounterAI {
        private float Ang;
        
        public DummyAI() : base() {
        }
        
        public DummyAI(Ship _Owner) : base(_Owner) {
            Ang = 0;
        }
        
        public override Ship.ControlsState Update(float dt) {
            Ang += (1f/3f)*Util.f2Pi*dt;
            Ship.ControlsState retval = new Ship.ControlsState (
                Math.Cos(Ang)>0?1f:0f,
                0f,
                Math.Cos(Ang)>0?1f:-1f,
                false
            );
            //Console.WriteLine(""+retval.TurningThrottle);
            return retval;
        }
    }
    
    public class KeyboardAI : EncounterAI {
    
        public KeyboardAI() : base() {
        }
        
        public KeyboardAI(Ship _Owner) : base(_Owner) {
        }
        
        public override Ship.ControlsState Update(float dt) {
            float fwd = Keyboard.IsKeyPressed(Keyboard.Key.W) ? 1f : 0f;
            float bck = Keyboard.IsKeyPressed(Keyboard.Key.S) ? 1f : 0f;
            float lft = Keyboard.IsKeyPressed(Keyboard.Key.A) ? 1f : 0f;
            float rgt = Keyboard.IsKeyPressed(Keyboard.Key.D) ? 1f : 0f;
            
            float st_lt = Keyboard.IsKeyPressed(Keyboard.Key.Q) ? 1f : 0f;
            float st_rt = Keyboard.IsKeyPressed(Keyboard.Key.E) ? 1f : 0f;
            return new Ship.ControlsState(
                fwd-bck,
                rgt-lft,
                st_rt-st_lt,
                Keyboard.IsKeyPressed(Keyboard.Key.Space)
            );
        }
    }
    
    public class FightRandomAI : EncounterAI {
        public FightRandomAI() : base() {
        }
        
        public FightRandomAI(Ship s) : base(s) {
        }
        
        private static readonly Random targetChoiceRandom = new Random();
        private Ship ChooseRandomTarget() {
            List<EncounterObject> possibleTargets = new List<EncounterObject>();
            
            foreach (EncounterObject so in Owner.Location) {
                if (so.Alive && so != Owner && so is Ship && ((Ship)so).MyFaction.GetRelation(Owner.MyFaction) < 0) {
                    possibleTargets.Add(so);
                }
            }
            
            if (possibleTargets.Count > 0) {
                return (Ship)possibleTargets[targetChoiceRandom.Next(possibleTargets.Count)];
            } else {
                return null;
            }
        }
        
        private float TurnThrottleTowardsAngle(float ang) {
            float distToTravel = Util.AngleDiff(Owner.Ang, ang);
            float initialSpeed = Owner.TurningSpeed;
            float accel = Owner.MyClass.TurningAccel;
            float stoppingDist = (initialSpeed*initialSpeed)/(2*accel);
            if (Math.Abs(stoppingDist) >= Math.Abs(distToTravel)) {
                return 0;
            } else {
                return distToTravel < 0 ? -1f : 1f;
            }
        }
        
        public enum State {IDLE, AVOID, ATTACK};
        private State MyState = State.IDLE;
        private Ship CurTarget = null;
        
        private float idleTimeLeft = 3f;
        
        private static readonly float avoidDist = 100f;
        
        public override Ship.ControlsState Update(float dt) {
            Ship.ControlsState cState = new Ship.ControlsState(0f,0f,0f,false);
            
            switch (MyState) {
                case State.IDLE:
                    idleTimeLeft -= dt;
                    if (idleTimeLeft <= 0) {
                        CurTarget = ChooseRandomTarget();
                        if (CurTarget == null) {
                            idleTimeLeft = 5f;
                        } else {
                            MyState = State.ATTACK;
                        }
                    }
                    cState.ForwardThrottle = 0.2f;
                    cState.TurningThrottle = 0f;
                    cState.StrafeThrottle = 0f;
                    cState.TriggerPulled = false;
                break;
                
                case State.ATTACK:
                    if (!CurTarget.Alive) {
                        idleTimeLeft = 2f;
                        MyState = State.IDLE;
                        break;
                    }
                    float angToTarget = (float)Math.Atan2(CurTarget.Pos.Y-Owner.Pos.Y, CurTarget.Pos.X-Owner.Pos.X);
                    float angDif = Util.AngleDiff(Owner.Ang, angToTarget);
                    
                    cState.ForwardThrottle = 1f;
                    cState.TurningThrottle = TurnThrottleTowardsAngle(angToTarget);
                    cState.StrafeThrottle = 0f;
                    cState.TriggerPulled = Math.Abs(angDif) < 0.1;
                break;
            }
            return cState;
        }
    }
    
    public class PointAtAI : EncounterAI {
        private Ship Subject;
        public PointAtAI(Ship subject) : base() {
            Subject = subject;
        }
        public PointAtAI(Ship owner, Ship subject) : base(owner) {
            Subject = subject;
        }
        
        private float TurnThrottleTowardsAngle(float ang) {
            float distToTravel = Util.AngleDiff(Owner.Ang, ang);
            float initialSpeed = Owner.TurningSpeed;
            float accel = Owner.MyClass.TurningAccel;
            float stoppingDist = (initialSpeed*initialSpeed)/(2*accel);
            if (Math.Abs(stoppingDist) >= Math.Abs(distToTravel)) {
                return 0;
            } else {
                return distToTravel < 0 ? -1f : 1f;
            }
        }
        
        public override Ship.ControlsState Update(float dt) {
            float xDiff = Subject.Pos.X-Owner.Pos.X;
            float yDiff = Subject.Pos.Y-Owner.Pos.Y;
            float angToSubject = (float)Math.Atan2(yDiff, xDiff);
            
            return new Ship.ControlsState(
                0f,
                Keyboard.IsKeyPressed(Keyboard.Key.X)
                ?
                TurnThrottleTowardsAngle(angToSubject)
                :
                (Util.AngleDiff(Owner.Ang, angToSubject) < 0 ? -1f : 1f),
                0f,
                false
            );
        }
    }
}