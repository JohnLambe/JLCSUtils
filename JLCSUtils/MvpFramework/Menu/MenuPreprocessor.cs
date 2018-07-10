using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Collections;

namespace MvpFramework.Menu
{
    public class MenuPreprocessor : IMenuPreprocessor
    {
        public virtual void Apply(MenuItemModel item)
        {
            if (item != null)
            {
                var prototype = _prototypes.TryGetValue(item.Id);
                if(prototype != null)
                {
                    if (prototype.Order != 0)
                        item.Order = prototype.Order;
                    if (prototype.HotKey != KeyboardKey.None)
                        item.HotKey = prototype.HotKey;
                    if (prototype.IconId != null)
                        item.IconId = prototype.IconId;
                    if (prototype.ContextKey != KeyboardKey.None)
                        item.ContextKey = prototype.ContextKey;
                    if (prototype.IsCancel)
                        item.IsCancel = prototype.IsCancel;
                    if (prototype.IsDefault)
                        item.IsCancel = prototype.IsDefault;
                    if (prototype.Style != null)
                        item.Style = prototype.Style;
                }
            }
        }

        public virtual void AddPrototype(MenuItemModel item)
        {
            _prototypes.Add(item.Id, item);
        }

        protected Dictionary<string,MenuItemModel> _prototypes;
    }
}
