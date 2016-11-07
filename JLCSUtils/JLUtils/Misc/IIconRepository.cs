using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Misc
{
    /// <summary>
    /// Possible states of an icon (conditions that affect its appearance).
    /// </summary>
    [Flags]
    public enum IconState
    {
        Normal = 0,
        HotTracked,
        Pressed,
        Disabled,
        Selected,
        Custom
    }

    /// <summary>
    /// Interface for getting icons with a key.
    /// </summary>
    /// <typeparam name="TKeyType">The type of the ID/key of icons.</typeparam>
    /// <typeparam name="TImageType">The type of the icons returned.</typeparam>
    public interface IIconRepository<TKeyType,TImageType>
    {
        /// <summary>
        /// Returns the requested icon.
        /// If it is not available in the requested state,
        /// this may return the closest available state,
        /// or the Normal state.
        /// </summary>
        /// <param name="iconId"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        TImageType GetIcon(TKeyType iconId, IconState state = IconState.Normal);
    }
}
