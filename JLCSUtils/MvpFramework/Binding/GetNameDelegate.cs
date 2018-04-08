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
        public new static GetNameEventArgs Empty { get; } = new GetNameEventArgs();
        // Note: This implementation works only while this is immutable.
    }

    /// <summary>
    /// Delegate to return a name/ID of an item.
    /// This is typically used when the name matches the name of a type member in code.
    /// </summary>
    /// <returns>The name/ID.</returns>
    /// <remarks>
    /// When used for names of type members, the recommended way to write the handler is like:
    /// <code>
    /// private string uiName(object sender, GetNameEventArgs args)
    ///     => $"{nameof(Model.Property1)}!.{nameof(Model.Property1.Property2)}";
    /// </code>
    /// Using "$" is more readable that an expression that builds the string, and using the name of the instance ensures that changing the type of the instance
    /// to an incompatible one, will result in a compile-time error.
    /// </remarks>
    public delegate string GetNameDelegate(object sender, GetNameEventArgs args);
}