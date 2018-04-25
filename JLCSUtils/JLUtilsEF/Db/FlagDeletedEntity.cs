using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Db
{
    public class FlagDeletedEntityBase : IHasActiveFlag
    {
        [Key]
        public int Id { get; set; }

        public virtual bool IsActive { get; set; }
    }

}
