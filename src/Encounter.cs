using System.Collections.Generic;
using System.Collections;
using SFML.Graphics;
using System;

namespace JohnnySpaceGame {
    /*
    a point in a solar system in which space objects are interacting (eg. a battle between some ships with bullets and wreckage flying about)
    */
    public class Encounter : ISpaceObjContainer, System.Collections.Generic.IEnumerable<EncounterObject> {
        /*
        set of objects participating in the encounter
        */
        private ISet<EncounterObject> Participants;
        /*
        queues to add/remove objects from the encounter
        queues are needed because the collection can't be modified when enumerating through it (ie. when we're updating the participants)
        so we need to save the changes we want to make, and then when we're done enumerating we apply the changes
        */
        private Queue<EncounterObject> InQueue;
        private Queue<EncounterObject> OutQueue;
        
        public Encounter() {
            Participants = new HashSet<EncounterObject>();
            InQueue = new Queue<EncounterObject>();
            OutQueue = new Queue<EncounterObject>();
        }
        
        public void Add(EncounterObject so) {
            if (Participants.Contains(so)) {
                return;
            }
            InQueue.Enqueue(so);
        }
        public void Remove(EncounterObject so) {
            if (!Participants.Contains(so)) {
                return;
            }
            OutQueue.Enqueue(so);
        }
        
        public void UpdateAll(float dt) {
            foreach (EncounterObject so in Participants) {
                if (so.Alive) {
                    so.Update(dt);
                }
            }
            while (InQueue.Count != 0) {
                EncounterObject newobj = InQueue.Dequeue();
                Console.WriteLine(newobj.GetType().Name+" entered the encounter");
                Participants.Add(newobj);
            }
            while (OutQueue.Count != 0) {
                EncounterObject oldobj = OutQueue.Dequeue();
                Console.WriteLine(oldobj.GetType().Name+" left the encounter");
                Participants.Remove(oldobj);
            }
        }
        
        public void DrawAll(RenderTarget rt) {
            foreach (EncounterObject so in Participants) {
                if (so.Alive) {
                    so.Draw(rt);
                }
            }
        }
        
        public IEnumerator<EncounterObject> GetEnumerator() {
            return Participants.GetEnumerator();
        }
        
        IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return Participants.GetEnumerator();
        }
    }
}