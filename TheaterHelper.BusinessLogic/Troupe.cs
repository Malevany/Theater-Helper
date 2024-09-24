using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheaterHelper.BusinessLogic
{
    public class Troupe
    {
        private int id;
        private List<Executors> troupeComposition;

        public int Id {  get { return id; } }
        public List<Executors> TroupeComposition { get { return troupeComposition; } }

        public Troupe(int id, List<Executors> troupeComposition)
        {
            this.id = id;
            this.troupeComposition = troupeComposition;
        }   
    }
}
