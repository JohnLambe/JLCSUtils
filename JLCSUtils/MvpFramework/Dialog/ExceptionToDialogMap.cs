﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.Collections;
using JohnLambe.Util.Reflection;
using System.Reflection;
using JohnLambe.Util;
using JohnLambe.Util.Types;
using JohnLambe.Util.Exceptions;
using SimpleInjector;
using DiExtension;

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

        //TODO: Consider nullability of parameters.
        // Consider all parameters non-nullable when not specified, but this may change in a later version.

        /// <summary>
        /// Determine a mapping (without uusing the cache).
        /// The results of this are cached.
        /// </summary>
        /// <param name="exceptionType"></param>
        /// <param name="dialogType"></param>
        /// <returns>true if the exception type can be mapped to a dialog type.</returns>
        protected virtual bool InternalGetDialogTypeForExceptionType(Type exceptionType, [Nullable] out Type dialogType)
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
                throw new AmbiguousMatchException("Ambiguous match for handler for " + exceptionType?.Name
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
        [return: Nullable]
        public virtual Type GetDialogModelTypeForDialogType([Nullable] Type dialogType)
        {
            if (dialogType == null)
                return null;
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
        /// The type of message dialog is chosen based on the exception type, after applying <see cref="ExceptionUtil.ExtractException(Exception)"/>.
        /// </summary>
        /// <param name="exception">The exception type to convert.
        /// If null, null is returned.</param>
        /// <returns></returns>
        [return: Nullable]
        public virtual MessageDialogModel<string> GetMessageDialogModelForException([Nullable] Exception exception)
        {
            if (exception == null)
                return null;

            var dialogType = GetDialogTypeForExceptionType(ExtractException(exception).GetType());
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

        /// <summary>
        /// Remove any mapping for a given exception type.
        /// </summary>
        /// <param name="exceptionType"></param>
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

        /// <summary>
        /// Drop the cache of mappings. (They will be repopulated on or before the call to get a mapping.)
        /// </summary>
        protected virtual void DropCache()
        {
            InitialiseCache();
        }

        /// <summary>
        /// Before mapping, 
        /// </summary>
        /// <param name="exception">The original exception to be mapped.</param>
        /// <returns>The exception used for the mapping.</returns>
        [return: Nullable("Iff passed null")]
        public virtual Exception ExtractException([Nullable] Exception exception)
            => ExceptionUtil.ExtractException(exception, WrappingExceptionsAssignable, WrappingExceptions);

        /// <summary>
        /// Exceptions for which the <see cref="Exception.InnerException"/> is extracted before mapping. Exceptions of types assignable to these types are also included.
        /// <see cref="ExceptionUtil.ExtractException(Exception, Type[], Type[])"/>
        /// </summary>
        [Nullable("none")]
        public virtual Type[] WrappingExceptionsAssignable { get; set; } = new Type[] { typeof(TargetInvocationException), typeof(ActivationException) };
        /// <summary>
        /// Exceptions for which the <see cref="Exception.InnerException"/> is extracted before mapping.
        /// <see cref="ExceptionUtil.ExtractException(Exception, Type[], Type[])"/>
        /// </summary>
        [Nullable("none")]
        public virtual Type[] WrappingExceptions { get; set; } = new Type[] { typeof(Exception), typeof(DependencyInjectionException) };


        /// <summary>
        /// The defined mappings.
        /// </summary>
        protected IDictionary<Type, Type> _mappings = new Dictionary<Type,Type>();  //TODO Use TypeMap
        /// <summary>
        /// Cache of resolved mappings.
        /// </summary>
        protected CachedSimpleLookup<Type,Type> CachedMappings;
    }
}
