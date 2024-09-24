using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheaterHelper.BusinessLogic
{
    public class Role
    {
        private int id;
        private string playTitle;
        private string name;
        private string roleImportance;
        
        public int Id { get { return id; } }
        public string PlayTitle { get {  return playTitle; } }
        public string Name { get { return name; } }
        public string RoleImportance { get {  return roleImportance; } }

        public Role(int id, string playTitle, string name, string roleImportance)
        {
            this.id = id;
            this.playTitle = playTitle;
            this.name = name;
            this.roleImportance = roleImportance;
        }
    }
}
