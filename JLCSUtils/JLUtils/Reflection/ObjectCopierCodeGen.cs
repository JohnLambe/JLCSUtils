using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Collections;

namespace JohnLambe.Util.Reflection
{
    public interface IClassMapping
    {
        Type SourceType { get; }
        Type DestinationType { get; }
        IEnumerable<IPropertyMapping> PropertyMappings { get; }
    }

    public interface IPropertyMapping
    {
        PropertyInfo Source { get; set; }

        PropertyInfo Destination { get; set; }

    }


    public abstract class ClassCodeGen<T>
        where T : class, IClassMapping
    {
        protected T _definition;

        public virtual void AddType(Type type)
        {
        }

        public virtual string Generate(T definition)
        {
            _definition = definition;
            try
            {
                StringBuilder code = new StringBuilder();
                foreach (var property in _definition.PropertyMappings)
                {
                    code.Append(GetPropertyCodeGen(property).GenerateDefinition());
                }
                return GetClassTemplate()
                    .Replace("<%Accessors%>", code.ToString())
                    .Replace("<%Class%>", GetClassName())
                    .Replace("<%BaseClass%>", GetBaseClassName());
            }
            finally
            {
                _definition = null;
            }
        }

        protected virtual string GetClassTemplate()
        {
            return "";
        }

        protected virtual string GetClassName()
        {
            return _definition.DestinationType.Name;
        }

        protected virtual string GetBaseClassName()
        {
            return _definition.DestinationType.BaseType.Name;
        }

        protected abstract PropertyCodeGen<IPropertyMapping> GetPropertyCodeGen(IPropertyMapping property);
//            where P : PropertyMapping;
    }

    public class JavaClassCodeGen<T> : ClassCodeGen<T>
        where T : class, IClassMapping
    {
        public JavaClassCodeGen()
        {
            Enums = new LazyInitializeDictionary<Type, string>(
                        key => GenerateEnum(key));
        }

        protected const string DeserializeMethodTemplate =
              "public Deserialize(InputStream stream)\n"
            + "{\n"
            + "<%DeserializeCode%>\n"
            + "}\n";

        protected const string ClassTemplate =
              "public class <%Class%> : <%BaseClass%>\n"
            + "{\n"
//            + DeserializeMethodTemplate + "\n"
//            + SerializeMethodTemplate + "\n"
            + "<%Accessors%>\n"
//            + "<%Custom%>\n"
            + "}\n";


        protected override string GetClassTemplate()
        {
            return ClassTemplate;
        }

        protected override PropertyCodeGen<IPropertyMapping> GetPropertyCodeGen(IPropertyMapping property)
        {
            var codeGen = new JavaPropertyCodeGen<JavaClassCodeGen<IClassMapping>,IPropertyMapping>();
            codeGen.Init(property);
            return codeGen;
        }

        public override void AddType(Type type)
        {
            if(type.IsEnum)
            {
                string dummy;
                Enums.TryGetValue(type, out dummy);    // add enum to list of enums in generated code
            }
        }

        #region Enums

        /// <summary>
        /// Generate code for a given Enum type.
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        protected virtual string GenerateEnum(Type enumType)
        {
            StringBuilder code = new StringBuilder();
            foreach(var enumMember in Enum.GetValues(enumType))
            {
                code.Append(
                    EnumTemplate.Replace("<%EnumMember%>",enumMember.ToString())
                    );
            }
            return EnumTemplate
                .Replace("<%Type%>", enumType.Name)
                .Replace("<%EnumMembers%>", code.ToString());
        }

        public virtual IDictionary<Type, string> Enums { get; protected set; }

        protected const string EnumTemplate =
            "public enum <%Type%>\n"
            + "{\n<%EnumMembers%>\n}\n";
        protected const string EnumMemberTemplate =
            "<%EnumMember%>, ";

        #endregion
    }

}
