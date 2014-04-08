using SFML.Window;
using System;
using System.Collections.Generic;

namespace JohnnySpaceGame {
    /*
    a special kind of weapon slot which can turn to point at different angles, instead of just pointing forwards
    */
    public class TurretWeaponSlot : WeaponSlot {
        private float ShootAngle;
        
        public readonly float MinShootAngle;
        public readonly float MaxShootAngle;
        public readonly float TurningAccel;
        
        private static readonly float targetCheckInterval = 1.0f;
        private float TargetCheckCooldown;
        
        private Ship Target;
        
        public TurretWeaponSlot(Ship _owner, TurretTemplate _ttempl) : base(_owner, _ttempl.BaseTemplate) {
            MinShootAngle = _ttempl.MinShootAngle;
            MaxShootAngle = _ttempl.MaxShootAngle;
            TurningAccel = _ttempl.TurningAccel;
            
            ShootAngle = (MinShootAngle+MaxShootAngle)/2f;
            
            TargetCheckCooldown = 0f;
        }
        
        public override float GetShootAngle() {
            return Owner.Ang+ShootAngle;
        }
        
        private bool IsValidEnemy(EncounterObject so) {
            if (so.Alive && so != Owner && so is Ship && ((Ship)so).MyFaction.GetRelation(Owner.MyFaction) < 0) {
                Vector2f shootPos = GetShootPos();
                float angToTarget = (float)Math.Atan2(so.Pos.Y-shootPos.Y, so.Pos.X-shootPos.X);
                float angDif = Util.AngleDiff(Owner.Ang, angToTarget);
                
                if (angDif >= MinShootAngle && angDif <= MaxShootAngle) {
                    return true;
                }
            }
            return false;
        }
        
        private static readonly Random targetChoiceRandom = new Random();
        private Ship ChooseRandomEnemy() {
            List<EncounterObject> possibleTargets = new List<EncounterObject>();
            
            foreach (EncounterObject so in Owner.Location) {
                if (IsValidEnemy(so)) {
                    possibleTargets.Add(so);
                }
            }
            
            if (possibleTargets.Count > 0) {
                return (Ship)possibleTargets[targetChoiceRandom.Next(possibleTargets.Count)];
            } else {
                return null;
            }
        }
        
        /*
        note: triggerPulled is used in turrets to say whether or not the turret is active and looking for enemies to shoot
        */
        public override void Update (float dt, bool triggerPulled) {
            if (TargetCheckCooldown > 0f) {
                TargetCheckCooldown -= dt;
            }
            if (!triggerPulled) {
                if (FittedWeapon != null) {
                    FittedWeapon.Update(dt, false);
                }
                return;
            }
            
            bool validTarget = Target != null && IsValidEnemy(Target);
            if (validTarget) {
                ShootAngle = (float)Math.Atan2(Target.Pos.Y-Owner.Pos.Y, Target.Pos.X-Owner.Pos.X);
            } else {
                Target = null;
                if (TargetCheckCooldown <= 0f) {
                    TargetCheckCooldown = targetCheckInterval;
                    Target = ChooseRandomEnemy();
                }
            }
            
            FittedWeapon.Update(dt, validTarget);
        }
        /*
        extra template data for a turret, giving the angles it can shoot at and how fast it turns
        */
        public struct TurretTemplate {
            public readonly Template BaseTemplate;
            public readonly float MinShootAngle;
            public readonly float MaxShootAngle;
            public readonly float TurningAccel;
            public TurretTemplate (Template _basetempl, float _mina, float _maxa, float _accel) {
                BaseTemplate = _basetempl;
                MinShootAngle = _mina;
                MaxShootAngle = _maxa;
                TurningAccel = _accel;
            }
        }
    }
}