using System;

namespace MvpFramework.Binding
{
    /// <summary>
    /// Arguments to <see cref="GetNameDelegate"/>.
    /// </summary>
    public class GetNameEventArgs : EventArgs
    {
        /// <summary>
        /// Empty arguments for this event.
        /// (This may or may not return the same instance on each call.)
        /// </summary>
        public new static GetNameEventArgs Empty { get; }  = new GetNameEventArgs();
        // Note: This implementation works only while this is immutable.
    }

    /// <summary>
    /// Delegate to return a name/ID of an item.
    /// This is typically used when the name matches the name of a type member in code.
    /// </summary>
    /// <returns>The name/ID.</returns>
    public delegate string GetNameDelegate(object sender, GetNameEventArgs args);
}