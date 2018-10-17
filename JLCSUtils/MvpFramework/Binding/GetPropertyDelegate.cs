using System;

namespace MvpFramework.Binding
{
    /// <summary>
    /// Arguments to <see cref="GetNameDelegate"/>.
    /// </summary>
    public class GetPropertyEventArgs : EventArgs
    {
        /// <summary>
        /// Empty arguments for this event.
        /// (This may or may not return the same instance on each call.)
        /// </summary>
        public new static GetPropertyEventArgs Empty { get; } = new GetPropertyEventArgs();
        // Note: This implementation works only while this is immutable.
    }

    /// <summary>
    /// Returns a delegate to get the property value, given the model object.
    /// </summary>
    /// <returns>The delegate.</returns>
    /// <remarks>
    /// When used for names of type members, the recommended way to write the handler is like:
    /// <code>
    /// private string uiName(object sender, GetNameEventArgs args)
    ///     => $"{nameof(Model.Property1)}!.{nameof(Model.Property1.Property2)}";
    /// </code>
    /// Using "$" is more readable that an expression that builds the string, and using the name of the instance ensures that changing the type of the instance
    /// to an incompatible one, will result in a compile-time error.
    /// </remarks>
    public delegate Func<TModel,TProperty> GetPropertyDelegate<TModel, TProperty>(object sender, GetPropertyEventArgs args);

    public delegate Func<object,object> GetPropertyEventHandler(object sender, GetPropertyEventArgs args);
}