using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graf
{
    public class route
    {

        public int id { get; set; }
        public List<int> motes { get; set; }

        public route(int ID)
        {
            this.id = ID;
            motes = new List<int>();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            route objAsroute = obj as route;
            if (objAsroute == null) return false;
            else return Equals(objAsroute);
        }
        public override int GetHashCode()
        {
            return id;
        }
        public bool Equals(route other)
        {
            if (other == null) return false;
            return (this.id.Equals(other.id));
        }
    
    }

}
