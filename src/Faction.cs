using System.Collections.Generic;

namespace JohnnySpaceGame {
    public class Faction {
        public readonly string Name;
        
        private Dictionary<Faction, int> RelationDict;
        
        public Faction(string _Name) {
            Name = _Name;
            RelationDict = new Dictionary<Faction, int>();
        }
        
        public void SetRelation(Faction other, int val) {
            RelationDict[other] = val;
        }
        
        public int GetRelation(Faction other) {
            if (RelationDict.ContainsKey(other)) {
                return RelationDict[other];
            } else {
                return 0;
            }
        }
    }
}