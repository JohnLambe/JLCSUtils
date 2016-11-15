using SimpleInjector.Advanced;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Reflection;
using DiExtension.Attributes;
using DiExtension.ConfigInject;

namespace DiExtension.SimpleInject
{
    /*
    public class ConstructorInjectionBehavior : InjectionBehaviorBase,
        IConstructorInjectionBehavior, IConstructorVerificationBehavior
    {
        public ConstructorInjectionBehavior(IConfigProvider provider) : base(provider)
        {
        }

        public virtual Expression BuildParameterExpression(ParameterInfo parameter)
        {
            return null;

            /-*
            Expression expr;
            TryGetExpression(parameter, parameter.ParameterType, out expr);

            var attrib = parameter.GetCustomAttribute<InjectAttribute>();
            if(attrib != null)
                if(attrib.Enabled && attrib.ByKey)
                {
                    string key = attrib.Key == InjectAttribute.CodeName ? parameter.Name : attrib.Key;
                    object value;
                    if(_container.GetValue(key,parameter.ParameterType, out value))
                        return value;
                }
            return Expression.Constant(
                _container.GetInstance<object>(parameter.GetType()),
                parameter.GetType());
                *-/
        }

        public virtual void Verify(ParameterInfo parameter)
        {
//            throw new NotImplementedException();
        }

        protected readonly SiExtendedDiContext _container;
    }
*/
}
