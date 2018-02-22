using System;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Reflection;
using System.Runtime.CompilerServices;

using JohnLambe.Util.Reflection;

namespace JohnLambe.Util.Misc
{
    /// <summary>
    /// A proxy that populates the wrapped object on the first attempt to access it (any member or the value of the object).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LazyPopulatedProxy<T> : DynamicProxyBase<T>
        where T : class
    {
        public LazyPopulatedProxy(Func<T> populateDelegate) : base()
        {
            PopulateDelegate = populateDelegate;
        }

        protected virtual void Populate()
        {
            Wrapped = PopulateDelegate();
            Populated = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void LazyPopulated()
        {
            if (!Populated)
                Populate();
        }

        // These methods will populate this object even on a failed attempt to invoke a member (when the member does not exist).
        // We could first check that the invocation attempt is valid based on typeof(T),
        // but invalid calls are probably rare (not worth optimising for)
        // and that would give different behaviour where the delegate returns a subclass of T.

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            LazyPopulated();
            return base.TryInvokeMember(binder, args, out result);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            LazyPopulated();
            return base.TryGetMember(binder, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            LazyPopulated();
            return base.TrySetMember(binder, value);
        }

        protected virtual Func<T> PopulateDelegate { get; set; }
        protected bool Populated { get; set; }
    }
}
