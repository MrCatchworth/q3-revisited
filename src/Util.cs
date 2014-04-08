using SFML.Window;
using System;

namespace JohnnySpaceGame {
    public abstract class Util {
        //some defs for floating point pi, 2pi, and halfpi
        public static readonly float fPi = (float)Math.PI;
        public static readonly float f2Pi = (float)Math.PI*2f;
        public static readonly float fHalfPi = (float)Math.PI*0.5f;
        
        public static float AngleDiff(float ang1, float ang2) {
            float angDif = ang2-ang1;
            if (angDif < fPi*-1f) {
                angDif += f2Pi;
            }
            if (angDif > fPi) {
                angDif -= f2Pi;
            }
            return angDif;
        }
    }
}