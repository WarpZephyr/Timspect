﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using Timspect.Unpacker.Exceptions;

namespace Timspect.Unpacker.Parsing.Xml
{
    internal static class XmlNodeValueParser
    {
        #region Node

        public static XmlNode GetNodeOrThrow(this XmlNode node, string xpath)
            => node.SelectSingleNode(xpath) ?? throw new XmlValueParseException($"Node {node.Name} does not contain: {xpath}");

        public static XmlAttributeCollection GetAttributesOrThrow(this XmlNode node)
            => node.Attributes ?? throw new XmlValueParseException($"Node {node.Name} is missing attributes.");

        public static XmlAttribute GetAttributeOrThrow(this XmlNode node, string name)
            => node.GetAttributesOrThrow()[name] ?? throw new XmlValueParseException($"Node {node.Name} does not contain attribute: {name}");

        public static bool TryGetXmlValue(this XmlNode node, string name, [NotNullWhen(true)] out string? result)
        {
            XmlAttribute? attribute = node.Attributes?[name];
            if (attribute != null)
            {
                result = attribute.InnerText;
                return true;
            }

            XmlNode? childNode = node.SelectSingleNode(name);
            if (childNode != null)
            {
                result = childNode.InnerText;
                return true;
            }

            result = null;
            return false;
        }

        public static bool TryGetXmlValueOrContents(this XmlNode node, string xmlValueName, [NotNullWhen(true)] out string? result)
        {
            if (node.TryGetXmlValue(xmlValueName, out result))
            {
                return true;
            }

            result = node.InnerText;
            if (string.IsNullOrEmpty(result))
            {
                return false;
            }

            return true;
        }

        public static string GetXmlValue(this XmlNode node, string name)
        {
            XmlAttribute? attribute = node.Attributes?[name];
            if (attribute != null)
            {
                return attribute.InnerText;
            }

            XmlNode? childNode = node.SelectSingleNode(name);
            if (childNode != null)
            {
                return childNode.InnerText;
            }

            throw new XmlValueParseException($"Node {node.Name} does not have a node or attribute named: {name}");
        }

        public static string GetXmlValueOrContents(this XmlNode node, string xmlValueName)
        {
            XmlAttribute? attribute = node.Attributes?[xmlValueName];
            if (attribute != null)
            {
                return attribute.InnerText;
            }

            XmlNode? childNode = node.SelectSingleNode(xmlValueName);
            if (childNode != null)
            {
                return childNode.InnerText;
            }

            string text = node.InnerText;
            if (!string.IsNullOrEmpty(text))
            {
                return text;
            }

            throw new XmlValueParseException($"Node {node.Name} has no contents and does not have a node or attribute named: {xmlValueName}");
        }

        #endregion

        #region Generic

        public static TValue ReadValue<TValue>(this XmlNode node, string xpath, IFormatProvider? provider = null) where TValue : IParsable<TValue>
            => node.GetNodeOrThrow(xpath).InnerText.ToValue<TValue>(xpath, provider);

        [return: NotNullIfNotNull(nameof(defaultValue))]
        public static TValue? ReadValueOrDefault<TValue>(this XmlNode node, string xpath, TValue? defaultValue = default, IFormatProvider? provider = null) where TValue : IParsable<TValue>
        {
            XmlNode? child = node.SelectSingleNode(xpath);
            return child == null ? defaultValue : child.InnerText.ToValue<TValue>(xpath, provider);
        }

        public static TValue? ReadValueIfExists<TValue>(this XmlNode node, string xpath, IFormatProvider? provider = null) where TValue : struct, IParsable<TValue>
            => node.SelectSingleNode(xpath)?.InnerText.ToValue<TValue>(xpath, provider);

        public static TEnum ReadEnum<TEnum>(this XmlNode node, string xpath) where TEnum : struct
            => node.GetNodeOrThrow(xpath).InnerText.ToEnum<TEnum>(xpath);

        public static TEnum ReadEnumOrDefault<TEnum>(this XmlNode node, string xpath, TEnum defaultValue = default) where TEnum : struct
        {
            XmlNode? child = node.SelectSingleNode(xpath);
            return child == null ? defaultValue : child.InnerText.ToEnum<TEnum>(xpath);
        }

        public static TEnum? ReadEnumIfExists<TEnum>(this XmlNode node, string xpath) where TEnum : struct
            => node.SelectSingleNode(xpath)?.InnerText.ToEnum<TEnum>(xpath);

        #endregion

        #region SByte

        public static sbyte ReadSByte(this XmlNode node, string xpath, IFormatProvider? provider = null)
            => node.ReadValue<sbyte>(xpath, provider);

        public static sbyte ReadSByteOrDefault(this XmlNode node, string xpath, sbyte defaultValue = default, IFormatProvider? provider = null)
            => node.ReadValueOrDefault(xpath, defaultValue, provider);

        public static sbyte? ReadSByteIfExists(this XmlNode node, string xpath, IFormatProvider? provider = null)
            => node.ReadValueIfExists<sbyte>(xpath, provider);

        #endregion

        #region Byte

        public static byte ReadByte(this XmlNode node, string xpath, IFormatProvider? provider = null)
            => node.ReadValue<byte>(xpath, provider);

        public static byte ReadByteOrDefault(this XmlNode node, string xpath, byte defaultValue = default, IFormatProvider? provider = null)
            => node.ReadValueOrDefault(xpath, defaultValue, provider);

        public static byte? ReadByteIfExists(this XmlNode node, string xpath, IFormatProvider? provider = null)
            => node.ReadValueIfExists<byte>(xpath, provider);

        #endregion

        #region Int16

        public static short ReadInt16(this XmlNode node, string xpath, IFormatProvider? provider = null)
            => node.ReadValue<short>(xpath, provider);

        public static short ReadInt16OrDefault(this XmlNode node, string xpath, short defaultValue = default, IFormatProvider? provider = null)
            => node.ReadValueOrDefault(xpath, defaultValue, provider);

        public static short? ReadInt16IfExists(this XmlNode node, string xpath, IFormatProvider? provider = null)
            => node.ReadValueIfExists<short>(xpath, provider);

        #endregion

        #region UInt16

        public static ushort ReadUInt16(this XmlNode node, string xpath, IFormatProvider? provider = null)
            => node.ReadValue<ushort>(xpath, provider);

        public static ushort ReadUInt16OrDefault(this XmlNode node, string xpath, ushort defaultValue = default, IFormatProvider? provider = null)
            => node.ReadValueOrDefault(xpath, defaultValue, provider);

        public static ushort? ReadUInt16IfExists(this XmlNode node, string xpath, IFormatProvider? provider = null)
            => node.ReadValueIfExists<ushort>(xpath, provider);

        #endregion

        #region Int32

        public static int ReadInt32(this XmlNode node, string xpath, IFormatProvider? provider = null)
            => node.ReadValue<int>(xpath, provider);

        public static int ReadInt32OrDefault(this XmlNode node, string xpath, int defaultValue = default, IFormatProvider? provider = null)
            => node.ReadValueOrDefault(xpath, defaultValue, provider);

        public static int? ReadInt32IfExists(this XmlNode node, string xpath, IFormatProvider? provider = null)
            => node.ReadValueIfExists<int>(xpath, provider);

        #endregion

        #region UInt32

        public static uint ReadUInt32(this XmlNode node, string xpath, IFormatProvider? provider = null)
            => node.ReadValue<uint>(xpath, provider);

        public static uint ReadUInt32OrDefault(this XmlNode node, string xpath, uint defaultValue = default, IFormatProvider? provider = null)
            => node.ReadValueOrDefault(xpath, defaultValue, provider);

        public static uint? ReadUInt32IfExists(this XmlNode node, string xpath, IFormatProvider? provider = null)
            => node.ReadValueIfExists<uint>(xpath, provider);

        #endregion

        #region Int64

        public static long ReadInt64(this XmlNode node, string xpath, IFormatProvider? provider = null)
            => node.ReadValue<long>(xpath, provider);

        public static long ReadInt64OrDefault(this XmlNode node, string xpath, long defaultValue = default, IFormatProvider? provider = null)
            => node.ReadValueOrDefault(xpath, defaultValue, provider);

        public static long? ReadInt64IfExists(this XmlNode node, string xpath, IFormatProvider? provider = null)
            => node.ReadValueIfExists<long>(xpath, provider);

        #endregion

        #region UInt64

        public static ulong ReadUInt64(this XmlNode node, string xpath, IFormatProvider? provider = null)
            => node.ReadValue<ulong>(xpath, provider);

        public static ulong ReadUInt64OrDefault(this XmlNode node, string xpath, ulong defaultValue = default, IFormatProvider? provider = null)
            => node.ReadValueOrDefault(xpath, defaultValue, provider);

        public static ulong? ReadUInt64IfExists(this XmlNode node, string xpath, IFormatProvider? provider = null)
            => node.ReadValueIfExists<ulong>(xpath, provider);

        #endregion

        #region Int128

        public static Int128 ReadInt128(this XmlNode node, string xpath, IFormatProvider? provider = null)
            => node.ReadValue<Int128>(xpath, provider);

        public static Int128 ReadInt128OrDefault(this XmlNode node, string xpath, Int128 defaultValue = default, IFormatProvider? provider = null)
            => node.ReadValueOrDefault(xpath, defaultValue, provider);

        public static Int128? ReadInt128IfExists(this XmlNode node, string xpath, IFormatProvider? provider = null)
            => node.ReadValueIfExists<Int128>(xpath, provider);

        #endregion

        #region UInt128

        public static UInt128 ReadUInt128(this XmlNode node, string xpath, IFormatProvider? provider = null)
            => node.ReadValue<UInt128>(xpath, provider);

        public static UInt128 ReadUInt128OrDefault(this XmlNode node, string xpath, UInt128 defaultValue = default, IFormatProvider? provider = null)
            => node.ReadValueOrDefault(xpath, defaultValue, provider);

        public static UInt128? ReadUInt128IfExists(this XmlNode node, string xpath, IFormatProvider? provider = null)
            => node.ReadValueIfExists<UInt128>(xpath, provider);

        #endregion

        #region Half

        public static Half ReadHalf(this XmlNode node, string xpath, IFormatProvider? provider = null)
            => node.ReadValue<Half>(xpath, provider);

        public static Half ReadHalfOrDefault(this XmlNode node, string xpath, Half defaultValue = default, IFormatProvider? provider = null)
            => node.ReadValueOrDefault(xpath, defaultValue, provider);

        public static Half? ReadHalfIfExists(this XmlNode node, string xpath, IFormatProvider? provider = null)
            => node.ReadValueIfExists<Half>(xpath, provider);

        #endregion

        #region Single

        public static float ReadSingle(this XmlNode node, string xpath, IFormatProvider? provider = null)
            => node.ReadValue<float>(xpath, provider);

        public static float ReadSingleOrDefault(this XmlNode node, string xpath, float defaultValue = default, IFormatProvider? provider = null)
            => node.ReadValueOrDefault(xpath, defaultValue, provider);

        public static float? ReadSingleIfExists(this XmlNode node, string xpath, IFormatProvider? provider = null)
            => node.ReadValueIfExists<float>(xpath, provider);

        #endregion

        #region Double

        public static double ReadDouble(this XmlNode node, string xpath, IFormatProvider? provider = null)
            => node.ReadValue<double>(xpath, provider);

        public static double ReadDoubleOrDefault(this XmlNode node, string xpath, double defaultValue = default, IFormatProvider? provider = null)
            => node.ReadValueOrDefault(xpath, defaultValue, provider);

        public static double? ReadDoubleIfExists(this XmlNode node, string xpath, IFormatProvider? provider = null)
            => node.ReadValueIfExists<double>(xpath, provider);

        #endregion

        #region Boolean

        public static bool ReadBoolean(this XmlNode node, string xpath, IFormatProvider? provider = null)
            => node.ReadValue<bool>(xpath, provider);

        public static bool ReadBooleanOrDefault(this XmlNode node, string xpath, bool defaultValue = default, IFormatProvider? provider = null)
            => node.ReadValueOrDefault(xpath, defaultValue, provider);

        public static bool? ReadBooleanIfExists(this XmlNode node, string xpath, IFormatProvider? provider = null)
            => node.ReadValueIfExists<bool>(xpath, provider);

        #endregion

        #region String

        public static string ReadString(this XmlNode node, string xpath)
            => node.GetNodeOrThrow(xpath).InnerText;

        [return: NotNullIfNotNull(nameof(defaultValue))]
        public static string? ReadStringOrDefault(this XmlNode node, string xpath, string? defaultValue = default)
        {
            XmlNode? child = node.SelectSingleNode(xpath);
            return child?.InnerText ?? defaultValue;
        }

        public static string? ReadStringIfExists(this XmlNode node, string xpath)
            => node.SelectSingleNode(xpath)?.InnerText;

        public static string ReadStringOrThrowIfEmpty(this XmlNode node, string xpath)
        {
            string value = node.ReadString(xpath);
            if (string.IsNullOrEmpty(value))
            {
                throw new XmlValueParseException($"{xpath} cannot be null or empty.");
            }
            return value;
        }

        public static string ReadStringOrThrowIfWhiteSpace(this XmlNode node, string xpath)
        {
            string value = node.ReadString(xpath);
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new XmlValueParseException($"{xpath} cannot be null, empty, or whitespace.");
            }
            return value;
        }

        #endregion

    }
}
