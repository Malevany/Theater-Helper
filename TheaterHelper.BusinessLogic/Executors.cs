using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheaterHelper.BusinessLogic
{
    public class Executors
    {
        private int id;
        private int troupeId;
        private Role role;
        private Actor mainActor;
        private Actor doubler;

        public int Id { get { return id; } }
        public int TroupeId { get { return troupeId; } }
        public Role Role { get { return role; } }
        public Actor MainActor { get {  return mainActor; } }
        public Actor Doubler { get {  return doubler; } }
        
        public Executors(int id, int troupeId, Role role, Actor mainActor, Actor doubler)
        {
            this.id = id;
            this.troupeId = troupeId;
            this.role = role;
            this.mainActor = mainActor;
            this.doubler = doubler;
        }
    }
}
