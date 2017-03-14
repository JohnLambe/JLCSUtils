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
        /// <summary>
        /// The normal state of an icon.
        /// </summary>
        Normal = 0,
        /// <summary>
        /// The 'Hot tracked' state, e.g. on for a mouse over.
        /// </summary>
        HotTracked,
        /// <summary>
        /// An appearance like a button being pushed down, for when they mouse etc. is held down on the icon.
        /// </summary>
        Pressed,
        /// <summary>
        /// An appearance that indicates that the action represented by the icon is not available.
        /// </summary>
        Disabled,
        /// <summary>
        /// 'Selected' appearance, for when an item can be selected from a group/list of items.
        /// </summary>
        Selected,
        /// <summary>
        /// Can be used by consumers of this library.
        /// </summary>
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
        /// <param name="iconId">Identifier of the icon. default(<typeparamref name="TKeyType"/>) and "" (when <typeparamref name="TKeyType"/> is <see cref="string"/>) are valid and cause default(<typeparamref name="TImageType"/>) to be returned.</param>
        /// <param name="state"></param>
        /// <returns>The requested icon, or default(<typeparamref name="TImageType"/>) if no icon is available.</returns>
        TImageType GetIcon(TKeyType iconId, IconState state = IconState.Normal);
    }


    /// <summary>
    /// Flags an item as holding an IconId, as used by <see cref="IIconRepository"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class IconIdAttribute : Attribute
    {
    }


    /// <summary>
    /// Null object implementation of <see cref="IIconRepository{TKeyType, TImageType}"/>.
    /// </summary>
    public sealed class NullIconRepository<TKeyType, TImageType> : IIconRepository<TKeyType, TImageType>
    //| Sealed because if it did anything else, it wouldn't be a null object.
    {
        public TImageType GetIcon(TKeyType iconId, IconState state = IconState.Normal)
        {
            return default(TImageType);
        }
    }
}
