using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework.Menu
{
    public interface IMenuPreprocessor
    {
        /// <summary>
        /// Run the preprocessor on the given item.
        /// </summary>
        /// <param name="item"></param>
        void Apply(MenuItemModel item);
    }
}
