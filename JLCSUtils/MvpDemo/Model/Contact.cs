using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MvpDemo.Model
{
    public class Contact  // could also be called ContactModel
    {
        [MaxLength(20)]   // sets maximum length of edit box control
        [Required]        // Not handled yet
        public string Name { get; set; }

        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }
        protected string _address;
    }

}
