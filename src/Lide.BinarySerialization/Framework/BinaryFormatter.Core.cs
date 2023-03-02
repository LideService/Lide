// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Runtime.Serialization;
using Lide.BinarySerialization.Additions;

namespace Lide.BinarySerialization.Framework;

public sealed partial class BinaryFormatter : IFormatter
{
    public object Deserialize(Stream serializationStream)
    {
        if (serializationStream == null)
        {
            throw new ArgumentNullException(nameof(serializationStream));
        }
        if (serializationStream.CanSeek && (serializationStream.Length == 0))
        {
            throw new SerializationException(SR.Serialization_Stream);
        }

        var formatterEnums = new InternalFE()
        {
            _typeFormat = _typeFormat,
            _serializerTypeEnum = InternalSerializerTypeE.Binary,
            _assemblyFormat = _assemblyFormat,
            _securityLevel = _securityLevel,
        };

        var reader = new ObjectReader(serializationStream, _surrogates, _context, formatterEnums, _binder)
        {
            _crossAppDomainArray = _crossAppDomainArray
        };
        try
        {
            BinaryFormatterEventSource.Log.DeserializationStart();
            var parser = new BinaryParser(serializationStream, reader);
            return reader.Deserialize(parser);
        }
        catch (SerializationException)
        {
            throw;
        }
        catch (Exception e)
        {
            throw new SerializationException(SR.Serialization_CorruptedStream, e);
        }
        finally
        {
            BinaryFormatterEventSource.Log.DeserializationStop();
        }
    }

    public void Serialize(Stream serializationStream, object graph)
    {
        if (serializationStream == null)
        {
            throw new ArgumentNullException(nameof(serializationStream));
        }

        var formatterEnums = new InternalFE()
        {
            _typeFormat = _typeFormat,
            _serializerTypeEnum = InternalSerializerTypeE.Binary,
            _assemblyFormat = _assemblyFormat,
        };

        try
        {
            BinaryFormatterEventSource.Log.SerializationStart();
            var sow = new ObjectWriter(_surrogates, _context, formatterEnums, _binder);
            BinaryFormatterWriter binaryWriter = new BinaryFormatterWriter(serializationStream, sow, _typeFormat);
            sow.Serialize(graph, binaryWriter);
            _crossAppDomainArray = sow._crossAppDomainArray;
        }
        finally
        {
            BinaryFormatterEventSource.Log.SerializationStop();
        }
    }
}