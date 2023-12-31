// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;

namespace Lide.BinarySerialization.Framework;

public sealed partial class BinaryFormatter : IFormatter
{
    private static readonly ConcurrentDictionary<Type, TypeInformation> s_typeNameCache = new ConcurrentDictionary<Type, TypeInformation>();

    internal ISurrogateSelector? _surrogates;
    internal StreamingContext _context;
    internal SerializationBinder? _binder;
    internal FormatterTypeStyle _typeFormat = FormatterTypeStyle.TypesAlways; // For version resiliency, always put out types
    internal FormatterAssemblyStyle _assemblyFormat = FormatterAssemblyStyle.Simple;
    internal TypeFilterLevel _securityLevel = TypeFilterLevel.Full;
    internal object[]? _crossAppDomainArray;

    public FormatterTypeStyle TypeFormat { get { return _typeFormat; } set { _typeFormat = value; } }
    public FormatterAssemblyStyle AssemblyFormat { get { return _assemblyFormat; } set { _assemblyFormat = value; } }
    public TypeFilterLevel FilterLevel { get { return _securityLevel; } set { _securityLevel = value; } }
    public ISurrogateSelector? SurrogateSelector { get { return _surrogates; } set { _surrogates = value; } }
    public SerializationBinder? Binder { get { return _binder; } set { _binder = value; } }
    public StreamingContext Context { get { return _context; } set { _context = value; } }

    public BinaryFormatter() : this(null, new StreamingContext(StreamingContextStates.All))
    {
    }

    public BinaryFormatter(ISurrogateSelector? selector, StreamingContext context)
    {
        _surrogates = selector;
        _context = context;
    }

    internal static TypeInformation GetTypeInformation(Type type) =>
        s_typeNameCache.GetOrAdd(type, t =>
        {
            var method1Params = new object[] {t, false };
            var method2Params = new object[] {t };
            var method1 = typeof(FormatterServices).GetMethods(BindingFlags.Static | BindingFlags.NonPublic).First(x => x.Name == "GetClrAssemblyName");
            var method2 = typeof(FormatterServices).GetMethods(BindingFlags.Static | BindingFlags.NonPublic).First(x => x.Name == "GetClrTypeFullName");
            var assemblyName = (string)method1.Invoke(null, method1Params);
            var fullName = (string)method2.Invoke(null, method2Params);
            return new TypeInformation(fullName, assemblyName, (bool)method1Params[1]);
        });
}