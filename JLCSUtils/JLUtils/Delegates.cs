﻿// Author: John Bernard Lambe
//   See licence.
////////////////////////////////////////

namespace JohnLambe.Util
{
    // Common delegates:

    /// <summary>
    /// Function with no return value.
    /// </summary>
    public delegate void VoidDelegate();

    /// <summary>
    /// Function with no return value, and one argument.
    /// </summary>
    public delegate void VoidDelegate<P>(P arg);

    /// <summary>
    /// Function with no return value, and two arguments.
    /// </summary>
    public delegate void VoidDelegate<P1, P2>(P1 arg1, P2 arg2);

    /// <summary>
    /// Function with no return value, and three arguments.
    /// </summary>
    public delegate void VoidDelegate<P1, P2, P3>(P1 arg1, P2 arg2, P3 arg3);

    /// <summary>
    /// Function with no return value, and four arguments.
    /// </summary>
    public delegate void VoidDelegate<P1, P2, P3, P4>(P1 arg1, P2 arg2, P3 arg3, P4 arg4);

    /// <summary>
    /// Function with no return value, and five arguments.
    /// </summary>
    public delegate void VoidDelegate<P1, P2, P3, P4, P5>(P1 arg1, P2 arg2, P3 arg3, P4 arg4, P5 arg5);

}
