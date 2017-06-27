using System;

namespace MvpFramework.Binding
{
    /// <summary>
    /// Arguments to <see cref="GetNameDelegate"/>.
    /// </summary>
    public class GetNameEventArgs : EventArgs
    {
        public new static readonly GetNameEventArgs Empty = new GetNameEventArgs();
        // Note: This implementation works only while this is immutable.
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public delegate string GetNameDelegate(object sender, GetNameEventArgs args);
}