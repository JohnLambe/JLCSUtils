using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpDemo.Model
{
    public class Contact  // could also be called ContactModel
    {
        public string Name { get; set; }

        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }
        protected string _address;
    }

}
