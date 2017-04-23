using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.TypeConversion
{
    // see System.ComponentModel.TypeConverter

    public class TypeConverter
    {
        public T Convert<T>(object source)
        {
            foreach(var converterRegistration in _converters.Values)
            {
//                if(converterRegistration.SourceType.)
//                converterRegistration.Converter
            }

            //TODO: Handle destination nullable types

            throw new NotImplementedException();  //TODO
        }

        public virtual void RegisterConverter<S,D>(IConverter<S,D> converter, int priority)
        {
            _converters.Add(priority, new ConverterRegistration() { SourceType = typeof(S), DestinationType = typeof(D), Converter = converter });
        }

        protected struct ConverterRegistration
        {
            public Type SourceType;
            public Type DestinationType;
            public object Converter;
        }

        protected SortedList<int, ConverterRegistration> _converters = new SortedList<int, ConverterRegistration>();
    }

}
