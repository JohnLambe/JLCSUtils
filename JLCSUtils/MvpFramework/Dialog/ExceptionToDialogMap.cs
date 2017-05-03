using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.Collections;
using JohnLambe.Util.Reflection;
using System.Reflection;
using JohnLambe.Util;

namespace MvpFramework.Dialog
{
    /// <summary>
    /// Maps exception types to dailog types.
    /// </summary>
    public class ExceptionToDialogMap
    {
        public ExceptionToDialogMap()
        {
            InitialiseCache();
        }

        /// <summary>
        /// Determine a mapping (without uusing the cache).
        /// The results of this are cached.
        /// </summary>
        /// <param name="exceptionType"></param>
        /// <param name="dialogType"></param>
        /// <returns></returns>
        protected virtual bool InternalGetDialogTypeForExceptionType(Type exceptionType, out Type dialogType)
        {
            KeyValuePair<Type,Type> bestMatch = new KeyValuePair<Type, Type>();
            bool match = false;
            var ambiguousMatches = new LinkedList<KeyValuePair<Type, Type>>();

            foreach (var mapping in _mappings)
            {
                if (mapping.Key == exceptionType)            // exact match
                {
                    dialogType = mapping.Value;                    // stop searching
                    return true;
                }

                if (mapping.Key.IsAssignableFrom(exceptionType))
                {
                    match = true;       // match found, searching for a better one
                    int moreSpecific = TypeUtil.IsMoreSpecific(mapping.Key, bestMatch.Key);
                    if (moreSpecific > 0)
                    {
                        bestMatch = mapping;
                        ambiguousMatches.Clear();   // no ambiguous match yet
                    }
                    else if (moreSpecific == 0)
                    {
                        ambiguousMatches.AddLast(mapping);    // match is currently ambiguous, but there might be a better one.
                    }
                }
            }

            if (ambiguousMatches.Any())
            {
                throw new AmbiguousMatchException("Ambiguous match for handler for " + exceptionType.Name
                    + ": " + CollectionUtil.CollectionToString(ambiguousMatches)  //TODO: format in string?
                    );
            }

            if(match)
            {
                dialogType = bestMatch.Value;
            }
            else
            {
                dialogType = null;
            }
            return match;
        }

        /// <summary>
        /// Get the dialog type (<see cref="MessageDialogType"/> or subclass) to use for a given exception type.
        /// </summary>
        /// <param name="exceptionType"></param>
        /// <returns></returns>
        public virtual Type GetDialogTypeForExceptionType(Type exceptionType)
        {
            return CachedMappings[exceptionType];
        }

        /// <summary>
        /// Get the message dialog model type (<see cref="MessageDialogModel{TResult}"/> or subclass)
        /// for the given dialog type (<see cref="MessageDialogType"/> or subclass).
        /// </summary>
        /// <param name="dialogType"></param>
        /// <returns></returns>
        public virtual Type GetDialogModelTypeForDialogType(Type dialogType)
        {
            if (typeof(MessageDialogModel<string>).IsAssignableFrom(dialogType))   //TODO: Handle any generic parameter
                return dialogType;                     // already a model type

            // Map by attribute if there is one:
            var attrib = dialogType.GetCustomAttribute<DefaultDialogModelTypeAttribute>();
            if (attrib != null)              // if the attribute is present
                return attrib.DialogType;    // use it even if it maps to null.

            // Map by naming convention if possible:
            if (dialogType.Name.EndsWith(MessageDialogType.ClassNameSuffix))  // if the class name has the conventional suffix
            {
                // Form the class name by convention:
                var name = dialogType.Namespace
                    + ".Dialogs."
                    + dialogType.Name.RemoveSuffix(MessageDialogType.ClassNameSuffix)
                        + MessageDialogModel<string>.ClassNameSuffix;         // change the suffix of the class name
                var modelType = Type.GetType(name);
                if (modelType != null)
                    return modelType;
            }

            if (dialogType == typeof(MessageDialogType))
                return null;               // to prevent recursing indefinitely if misconfigured

            return GetDialogModelTypeForDialogType(dialogType.BaseType);   // use mapping of the base class (recursively)
        }

        /// <summary>
        /// Returns a populated <see cref="MessageDialogModel{TResult}"/> instance with the details 
        /// of the given exception.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public virtual MessageDialogModel<string> GetMessageDialogModelForException(Exception exception)
        {
            if (exception == null)
                return null;

            var dialogType = GetDialogTypeForExceptionType(exception.GetType());
            var dialogModelType = GetDialogModelTypeForDialogType(dialogType);
            var dialogModel = ReflectionUtil.Create<MessageDialogModel<string>>(dialogModelType);
            dialogModel.Exception = exception;

            return dialogModel;
        }

        #region Explicit Mapping

        public virtual void AddMapping(Type exceptionType, Type dialogType)
        {
            _mappings[exceptionType] = dialogType;
            DropCache();
        }

        public virtual void RemoveMapping(Type exceptionType)
        {
            _mappings.Remove(exceptionType);
            DropCache();
        }

        public virtual void RemoveMappingsByDialogType(Type dialogType)
        {
            var mappings = _mappings.Where(m => m.Value == dialogType);
            foreach (var mapping in mappings)
            {
                _mappings.Remove(mapping);
            }
            DropCache();
        }

        #endregion

        #region Scanning for implicit mappings

        /// <summary>
        /// Scan a list of assemblies and register mappings.
        /// </summary>
        /// <param name="assemblies">The list of assemblies to scan. If empty, the calling assembly is scanned.</param>
        public virtual void ScanAssemblies(params Assembly[] assemblies)
        {
            if (assemblies.Length == 0)
                assemblies = new Assembly[] { Assembly.GetCallingAssembly() };
            Scan(assemblies);
        }

        public virtual void Scan(IEnumerable<Assembly> assemblies)
        {
            foreach(var attrib in assemblies.SelectMany(a => a.GetTypes()).SelectMany(t => t.GetAttributesWithMember<MappedExceptionAttribute, Type>()))
            {
                AddMapping(attrib.Attribute.ExceptionClass, attrib.DeclaringMember);
            }

            foreach (var attrib in assemblies.SelectMany(a => a.GetTypes()).SelectMany(t => t.GetAttributesWithMember<MappedDialogTypeAttribute, Type>()))
            {
                AddMapping(attrib.DeclaringMember, attrib.Attribute.DialogType);
            }
        }

        #endregion

        /// <summary>
        /// Initialise the cache, dropping it if it is already initialsed.
        /// </summary>
        protected virtual void InitialiseCache()
        {
            CachedMappings = new CachedSimpleLookup<Type, Type>(InternalGetDialogTypeForExceptionType);
        }

        protected virtual void DropCache()
        {
            InitialiseCache();
        }

        /// <summary>
        /// The defined mappings.
        /// </summary>
        protected IDictionary<Type, Type> _mappings = new Dictionary<Type,Type>();
        /// <summary>
        /// Cache of resolved mappings.
        /// </summary>
        protected CachedSimpleLookup<Type,Type> CachedMappings;
    }
}
