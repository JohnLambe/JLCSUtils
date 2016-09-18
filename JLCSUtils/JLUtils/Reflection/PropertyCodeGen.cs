using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace JohnLambe.Util.Reflection
{
    public class PropertyCodeGen<TCodeGen,T>
        where TCodeGen : ClassCodeGen<IClassMapping>
        where T : IPropertyMapping
    {

        public void Init(TCodeGen codeGen)
        {
            _codeGen = codeGen;
        }

        protected T _propertDetails;
        protected TCodeGen _codeGen;

        public virtual string GenerateDefinition(T definition)
        {
            _propertDetails = definition;

            return "";
        }

        protected virtual string FillTemplate(string template)
        {
            return template
                .Replace("<%Comment%>", Comment)
                .Replace("<%Type%>", TypeName)
                .Replace("<%NameLower%>", NameLower)
                .Replace("<%Name%>", Name);
        }

        public virtual string Name
        {
            get { return _propertDetails.Destination.Name;  }
        }

        public virtual string TypeName
        {
            get
            {
                return _propertDetails.Destination.PropertyType.Name;
            }
        }

        public virtual string Comment
        {
            get { return ""; }
        }

        /// <summary>
        /// Property name with first letter changed to lower case.
        /// </summary>
        protected virtual string NameLower
        {
            get
            {
                return Name.Substring(0, 1).ToLower() + Name.Substring(1);
            }
        }
    }


    public class JavaPropertyCodeGen<TCodeGen,T> : PropertyCodeGen<TCodeGen,T>
        where TCodeGen : ClassCodeGen<IClassMapping>
        where T : IPropertyMapping
    {
        protected const string GetterTemplate =
              "/** <%Comment%> */\n"
            + "public <%Type%> get<%Name%>()\n"
            + "{ return _<%NameLower%>; }\n";

        protected const string SetterTemplate =
              "/** <%Comment%> */\n"
            + "public set<%Name%>(<%Type%> value)\n"
            + "{ _<%NameLower%> = value; }\n";

        protected const string FieldTemplate =
            "protected <%Type%> _<%NameLower%>;\n";

        protected const string PropertyTemplate =
              GetterTemplate + "\n"
            + SetterTemplate + "\n"
            + FieldTemplate + "\n";

        public override string GenerateDefinition()
        {
            return FillTemplate(PropertyTemplate);
        }

        public override string TypeName
        {
            get
            {
                var type = _propertDetails.Destination.PropertyType;
                if (type.IsPrimitive)
                {
                    if (type == typeof(Boolean))
                        return "boolean";
                    else if (type == typeof(int) || type == typeof(uint))
                        return "int";
                    else if (type == typeof(sbyte) || type == typeof(byte))
                        return "byte";
                    else if (type == typeof(short) || type == typeof(ushort))
                        return "short";
                    else if (type == typeof(long) || type == typeof(ulong))
                        return "long";
                    else if (type == typeof(char))
                        return "char";
                }
                else if(type.IsEnum)
                {
                    _codeGen.AddType(type);
                }
                else
                {

                }

                return type.Name;
            }
        }


        protected const string DeserializeTemplate =
              "_<%NameLower%> = Deserialize<%Type%>(stream);\n";

        protected const string SerializeTemplate =
              "Serialize<%Type%>(stream, _<%NameLower%>);\n";

//        protected T Definition { get; set; }
    }
}
