namespace Lide.BinarySerialization.Additions;

internal static partial class SR
{
    /// <summary>Type '{0}' in Assembly '{1}' is not marked as serializable.</summary>
    internal static string @Serialization_NonSerType => GetResourceString("Serialization_NonSerType");
    /// <summary>Parameters 'members' and 'data' must have the same length.</summary>
    internal static string @Argument_DataLengthDifferent => GetResourceString("Argument_DataLengthDifferent");
    /// <summary>Member at position {0} was null.</summary>
    internal static string @ArgumentNull_NullMember => GetResourceString("ArgumentNull_NullMember");
    /// <summary>Only FieldInfo, PropertyInfo, and SerializationMemberInfo are recognized.</summary>
    internal static string @Serialization_UnknownMemberInfo => GetResourceString("Serialization_UnknownMemberInfo");
    /// <summary>Object has never been assigned an objectID.</summary>
    internal static string @Serialization_NoID => GetResourceString("Serialization_NoID");
    /// <summary>The internal array cannot expand to greater than Int32.MaxValue elements.</summary>
    internal static string @Serialization_TooManyElements => GetResourceString("Serialization_TooManyElements");
    /// <summary>The FieldInfo object is not valid.</summary>
    internal static string @Argument_InvalidFieldInfo => GetResourceString("Argument_InvalidFieldInfo");
    /// <summary>A fixup is registered to the object with ID {0}, but the object does not appear in the graph.</summary>
    internal static string @Serialization_NeverSeen => GetResourceString("Serialization_NeverSeen");
    /// <summary>The object with ID {0} implements the IObjectReference interface for which all dependencies cannot be resolved. The likely cause is two instances of IObjectReference that have a mutual dependency on each other.</summary>
    internal static string @Serialization_IORIncomplete => GetResourceString("Serialization_IORIncomplete");
    /// <summary>The object with ID {0} was referenced in a fixup but does not exist.</summary>
    internal static string @Serialization_ObjectNotSupplied => GetResourceString("Serialization_ObjectNotSupplied");
    /// <summary>{0}.SetObjectData returns a value that is neither null nor equal to the first parameter. Such Surrogates cannot be part of cyclical reference.</summary>
    internal static string @Serialization_NotCyclicallyReferenceableSurrogate => GetResourceString("Serialization_NotCyclicallyReferenceableSurrogate");
    /// <summary>The implementation of the IObjectReference interface returns too many nested references to other objects that implement IObjectReference.</summary>
    internal static string @Serialization_TooManyReferences => GetResourceString("Serialization_TooManyReferences");
    /// <summary>The object with ID {0} was referenced in a fixup but has not been registered.</summary>
    internal static string @Serialization_MissingObject => GetResourceString("Serialization_MissingObject");
    /// <summary>A fixup on an object implementing ISerializable or having a surrogate was discovered for an object which does not have a SerializationInfo available.</summary>
    internal static string @Serialization_InvalidFixupDiscovered => GetResourceString("Serialization_InvalidFixupDiscovered");
    /// <summary>Unable to load type {0} required for deserialization.</summary>
    internal static string @Serialization_TypeLoadFailure => GetResourceString("Serialization_TypeLoadFailure");
    /// <summary>ValueType fixup on Arrays is not implemented.</summary>
    internal static string @Serialization_ValueTypeFixup => GetResourceString("Serialization_ValueTypeFixup");
    /// <summary>Fixing up a partially available ValueType chain is not implemented.</summary>
    internal static string @Serialization_PartialValueTypeFixup => GetResourceString("Serialization_PartialValueTypeFixup");
    /// <summary>Cannot perform fixup.</summary>
    internal static string @Serialization_UnableToFixup => GetResourceString("Serialization_UnableToFixup");
    /// <summary>objectID cannot be less than or equal to zero.</summary>
    internal static string @ArgumentOutOfRange_ObjectID => GetResourceString("ArgumentOutOfRange_ObjectID");
    /// <summary>An object cannot be registered twice.</summary>
    internal static string @Serialization_RegisterTwice => GetResourceString("Serialization_RegisterTwice");
    /// <summary>The given object does not implement the ISerializable interface.</summary>
    internal static string @Serialization_NotISer => GetResourceString("Serialization_NotISer");
    /// <summary>The constructor to deserialize an object of type '{0}' was not found.</summary>
    internal static string @Serialization_ConstructorNotFound => GetResourceString("Serialization_ConstructorNotFound");
    /// <summary>The ObjectManager found an invalid number of fixups. This usually indicates a problem in the Formatter.</summary>
    internal static string @Serialization_IncorrectNumberOfFixups => GetResourceString("Serialization_IncorrectNumberOfFixups");
    /// <summary>A member fixup was registered for an object which implements ISerializable or has a surrogate. In this situation, a delayed fixup must be used.</summary>
    internal static string @Serialization_InvalidFixupType => GetResourceString("Serialization_InvalidFixupType");
    /// <summary>Object IDs must be greater than zero.</summary>
    internal static string @Serialization_IdTooSmall => GetResourceString("Serialization_IdTooSmall");
    /// <summary>The ID of the containing object cannot be the same as the object ID.</summary>
    internal static string @Serialization_ParentChildIdentical => GetResourceString("Serialization_ParentChildIdentical");
        
    /// <summary>When supplying the ID of a containing object, the FieldInfo that identifies the current field within that object must also be supplied.</summary>
    internal static string @Argument_MustSupplyParent => GetResourceString("Argument_MustSupplyParent");
    /// <summary>Cannot supply both a MemberInfo and an Array to indicate the parent of a value type.</summary>
    internal static string @Argument_MemberAndArray => GetResourceString("Argument_MemberAndArray");
    /// <summary>Invalid BinaryFormatter stream.</summary>
    internal static string @Serialization_CorruptedStream => GetResourceString("Serialization_CorruptedStream");
    /// <summary>Attempting to deserialize an empty stream.</summary>
    internal static string @Serialization_Stream => GetResourceString("Serialization_Stream");
    /// <summary>Binary stream '{0}' does not contain a valid BinaryHeader. Possible causes are invalid stream or object version change between serialization and deserialization.</summary>
    internal static string @Serialization_BinaryHeader => GetResourceString("Serialization_BinaryHeader");
    /// <summary>Invalid expected type.</summary>
    internal static string @Serialization_TypeExpected => GetResourceString("Serialization_TypeExpected");
    /// <summary>End of Stream encountered before parsing was completed.</summary>
    internal static string @Serialization_StreamEnd => GetResourceString("Serialization_StreamEnd");
    /// <summary>Cross-AppDomain BinaryFormatter error; expected '{0}' but received '{1}'.</summary>
    internal static string @Serialization_CrossAppDomainError => GetResourceString("Serialization_CrossAppDomainError");
    /// <summary>No map for object '{0}'.</summary>
    internal static string @Serialization_Map => GetResourceString("Serialization_Map");
    /// <summary>No assembly information is available for object on the wire, '{0}'.</summary>
    internal static string @Serialization_Assembly => GetResourceString("Serialization_Assembly");
    /// <summary>Invalid ObjectTypeEnum {0}.</summary>
    internal static string @Serialization_ObjectTypeEnum => GetResourceString("Serialization_ObjectTypeEnum");
    /// <summary>No assembly ID for object type '{0}'.</summary>
    internal static string @Serialization_AssemblyId => GetResourceString("Serialization_AssemblyId");
    /// <summary>Invalid array type '{0}'.</summary>
    internal static string @Serialization_ArrayType => GetResourceString("Serialization_ArrayType");
    /// <summary>Invalid type code in stream '{0}'.</summary>
    internal static string @Serialization_TypeCode => GetResourceString("Serialization_TypeCode");
    /// <summary>Invalid write type request '{0}'.</summary>
    internal static string @Serialization_TypeWrite => GetResourceString("Serialization_TypeWrite");
    /// <summary>Invalid read type request '{0}'.</summary>
    internal static string @Serialization_TypeRead => GetResourceString("Serialization_TypeRead");
    /// <summary>Unable to find assembly '{0}'.</summary>
    internal static string @Serialization_AssemblyNotFound => GetResourceString("Serialization_AssemblyNotFound");
    /// <summary>The input stream is not a valid binary format. The starting contents (in bytes) are: {0} ...</summary>
    internal static string @Serialization_InvalidFormat => GetResourceString("Serialization_InvalidFormat");
    /// <summary>No top object.</summary>
    internal static string @Serialization_TopObject => GetResourceString("Serialization_TopObject");
    /// <summary>Invalid element '{0}'.</summary>
    internal static string @Serialization_XMLElement => GetResourceString("Serialization_XMLElement");
    /// <summary>Top object cannot be instantiated for element '{0}'.</summary>
    internal static string @Serialization_TopObjectInstantiate => GetResourceString("Serialization_TopObjectInstantiate");
    /// <summary>Array element type is Object, 'dt' attribute is null.</summary>
    internal static string @Serialization_ArrayTypeObject => GetResourceString("Serialization_ArrayTypeObject");
    /// <summary>Type is missing for member of type Object '{0}'.</summary>
    internal static string @Serialization_TypeMissing => GetResourceString("Serialization_TypeMissing");
    /// <summary>Object {0} has never been assigned an objectID.</summary>
    internal static string @Serialization_ObjNoID => GetResourceString("Serialization_ObjNoID");
    /// <summary>MemberInfo type {0} cannot be serialized.</summary>
    internal static string @Serialization_SerMemberInfo => GetResourceString("Serialization_SerMemberInfo");
    /// <summary>When supplying a FieldInfo for fixing up a nested type, a valid ID for that containing object must also be supplied.</summary>
    internal static string @Argument_MustSupplyContainer => GetResourceString("Argument_MustSupplyContainer");
    /// <summary>Parse error. Current element is not compatible with the next element, {0}.</summary>
    internal static string @Serialization_ParseError => GetResourceString("Serialization_ParseError");
    /// <summary>MemberInfo requested for ISerializable type.</summary>
    internal static string @Serialization_ISerializableMemberInfo => GetResourceString("Serialization_ISerializableMemberInfo");
    /// <summary>MemberInfo cannot be obtained for ISerialized Object '{0}'.</summary>
    internal static string @Serialization_MemberInfo => GetResourceString("Serialization_MemberInfo");
    /// <summary>Types not available for ISerializable object '{0}'.</summary>
    internal static string @Serialization_ISerializableTypes => GetResourceString("Serialization_ISerializableTypes");
    /// <summary>Member '{0}' in class '{1}' is not present in the serialized stream and is not marked with {2}.</summary>
    internal static string @Serialization_MissingMember => GetResourceString("Serialization_MissingMember");
    /// <summary>No MemberInfo for Object {0}.</summary>
    internal static string @Serialization_NoMemberInfo => GetResourceString("Serialization_NoMemberInfo");
    /// <summary>Type {0} and the types derived from it (such as {1}) are not permitted to be deserialized at this security level.</summary>
    internal static string @Serialization_DuplicateSelector => GetResourceString("Serialization_DuplicateSelector");
    /// <summary>Selector contained a cycle.</summary>
    internal static string @Serialization_SurrogateCycleInArgument => GetResourceString("Serialization_SurrogateCycleInArgument");
    /// <summary>Adding selector will introduce a cycle.</summary>
    internal static string @Serialization_SurrogateCycle => GetResourceString("Serialization_SurrogateCycle");
        

}