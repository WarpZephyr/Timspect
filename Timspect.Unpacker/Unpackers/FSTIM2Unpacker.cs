﻿using System;
using System.Drawing;
using System.IO;
using System.Xml;
using Timspect.Core.Formats;
using Timspect.Core.Graphics;
using Timspect.Unpacker.Exceptions;
using Timspect.Unpacker.Helpers;
using Timspect.Unpacker.Parsing.Xml;
using static Timspect.Core.Formats.FSTIM2;

namespace Timspect.Unpacker.Unpackers
{
    public static class FSTIM2Unpacker
    {
        public static void Unpack(string path, string folder, FSTIM2 file)
        {
            string filename = Path.GetFileName(path);
            string extensionless = Path.GetFileNameWithoutExtension(filename);

            var xws = new XmlWriterSettings
            {
                Indent = true
            };

            var xw = XmlWriter.Create(Path.Combine(folder, "_fstim2.xml"), xws);
            xw.WriteStartElement("fstim2");
            xw.WriteElementString("decoder", Program.ProgramName);
            xw.WriteElementString("filename", filename);
            xw.WriteElementString("formatversion", $"{file.FormatVersion}");
            xw.WriteElementString("formatid", $"{(byte)file.FormatID}");
            xw.WriteStartElement("pictures");

            bool indexName = file.Pictures.Count > 1;
            for (int picIndex = 0; picIndex < file.Pictures.Count; picIndex++)
            {
                string outName = indexName ? $"{extensionless}{picIndex}.png" : $"{extensionless}.png";
                string picOutPath = Path.Combine(folder, outName);
                var picture = file.Pictures[picIndex];

                // Write PNG
                bool indexed = picture.Indexed;
                bool hasAlpha = picture.HasAlpha;
                int bitDepth = indexed ? picture.BitDepth : 8;
                ImageHelper.WritePNG(picOutPath, picture.Width, picture.Height, bitDepth, hasAlpha, indexed, picture.Image, picture.Clut);

                xw.WriteStartElement("picture");
                xw.WriteElementString("filename", outName);
                xw.WriteElementString("clutcolors", $"{picture.Clut.Length}");
                xw.WriteElementString("pictformat", $"{picture.PictureFormat}");
                xw.WriteElementString("mipmaptextures", $"{1 + picture.Mipmaps.Count}");
                xw.WriteElementString("imagetype", $"{Picture.GetColorTypeName(picture.ImageColorType)}");
                xw.WriteElementString("width", $"{picture.Width}");
                xw.WriteElementString("height", $"{picture.Height}");
                xw.WriteElementString("extendedheader", $"{picture.WriteExtendedHeader}");
                xw.WriteElementString("comment", picture.Comment);

                xw.WriteStartElement("cluttype");
                xw.WriteElementString("clutcolortype", $"{Picture.GetColorTypeName(picture.ClutType.ClutColorType)}");
                xw.WriteElementString("clutcompound", $"{picture.ClutType.ClutCompound}");
                xw.WriteElementString("csm", $"{picture.ClutType.ClutStorageMode}");
                xw.WriteEndElement();

                xw.WriteStartElement("gstex0");
                xw.WriteElementString("tbp0", $"{picture.GsTex.TextureBasePointer}");
                xw.WriteElementString("tbw", $"{picture.GsTex.TextureBufferWidth}");
                xw.WriteElementString("psm", $"{picture.GsTex.PixelStorageMode}");
                xw.WriteElementString("tw", $"{picture.GsTex.TextureWidth}");
                xw.WriteElementString("th", $"{picture.GsTex.TextureHeight}");
                xw.WriteElementString("tcc", $"{picture.GsTex.TextureColorComponent}");
                xw.WriteElementString("tfx", $"{picture.GsTex.TextureFunction}");
                xw.WriteElementString("cbp", $"{picture.GsTex.ClutBasePointer}");
                xw.WriteElementString("cpsm", $"{picture.GsTex.ClutPixelStorageMode}");
                xw.WriteElementString("csm", $"{picture.GsTex.ClutStorageMode}");
                xw.WriteElementString("csa", $"{picture.GsTex.ClutStartAddress}");
                xw.WriteElementString("cld", $"{picture.GsTex.ClutLoadControl}");
                xw.WriteEndElement();

                xw.WriteStartElement("gstex1");
                xw.WriteElementString("lcm", $"{picture.GsTex.LODCalculationMethod}");
                xw.WriteElementString("mxl", $"{picture.GsTex.MipLevelMax}");
                xw.WriteElementString("mmag", $"{picture.GsTex.MipMag}");
                xw.WriteElementString("mmin", $"{picture.GsTex.MipMin}");
                xw.WriteElementString("mtba", $"{picture.GsTex.MipmapTextureBaseAddress}");
                xw.WriteElementString("l", $"{picture.GsTex.LODParameterL}");
                xw.WriteElementString("k", $"{picture.GsTex.LODParameterK}");
                xw.WriteEndElement();

                xw.WriteStartElement("gstexafbapabe");
                xw.WriteElementString("ta0", $"{picture.GsTex.TA0}");
                xw.WriteElementString("aem", $"{picture.GsTex.AEM}");
                xw.WriteElementString("ta1", $"{picture.GsTex.TA1}");
                xw.WriteElementString("pabe", $"{picture.GsTex.PABE}");
                xw.WriteElementString("fba", $"{picture.GsTex.FBA}");
                xw.WriteEndElement();

                xw.WriteStartElement("gstexclut");
                xw.WriteElementString("cbw", $"{picture.GsTex.ClutBufferWidth}");
                xw.WriteElementString("cou", $"{picture.GsTex.ClutOffsetU}");
                xw.WriteElementString("cov", $"{picture.GsTex.ClutOffsetV}");
                xw.WriteEndElement();

                if (picture.Mipmaps.Count > 0)
                {
                    xw.WriteStartElement("mipmaps");
                    xw.WriteStartElement("gsmiptbp1");
                    for (int i = 0; i < 3; i++)
                    {
                        int level = i + 1;
                        xw.WriteElementString($"tbp{level}", $"{picture.MipmapTextureBasePointers[i]}");
                        xw.WriteElementString($"tbw{level}", $"{picture.MipmapTextureBufferWidths[i]}");
                    }
                    xw.WriteEndElement();

                    xw.WriteStartElement("gsmiptbp2");
                    for (int i = 3; i < 6; i++)
                    {
                        int level = i + 1;
                        xw.WriteElementString($"tbp{level}", $"{picture.MipmapTextureBasePointers[i]}");
                        xw.WriteElementString($"tbw{level}", $"{picture.MipmapTextureBufferWidths[i]}");
                    }
                    xw.WriteEndElement();

                    // Write PNGs
                    xw.WriteStartElement("filenames");
                    for (int mipIndex = 0; mipIndex < picture.Mipmaps.Count; mipIndex++)
                    {
                        int level = mipIndex + 1;
                        outName = indexName ? $"{extensionless}{picIndex}_LV{level}.png" : $"{extensionless}_LV{level}.png";
                        picOutPath = Path.Combine(folder, outName);
                        ImageHelper.WritePNG(picOutPath, picture.Width >>> level, picture.Height >>> level, bitDepth, hasAlpha, indexed, picture.Mipmaps[mipIndex], picture.Clut);
                        xw.WriteElementString("filename", outName);
                    }
                    xw.WriteEndElement();
                    xw.WriteEndElement();
                }

                if (picture.UserData.Length > 0)
                {
                    xw.WriteStartElement("userdata");
                    xw.WriteElementString("data", Convert.ToBase64String(picture.UserData));
                    xw.WriteEndElement();
                }

                if (indexed)
                {
                    xw.WriteStartElement("clut");
                    for (int i = 0; i < picture.Clut.Length; i++)
                    {
                        var color = picture.Clut[i];
                        xw.WriteElementString("color", $"[{color.R:X2}{color.G:X2}{color.B:X2}{color.A:X2}]");
                    }
                    xw.WriteEndElement();
                }
                xw.WriteEndElement();
            }
            xw.WriteEndElement();
            xw.WriteEndElement();
            xw.Close();
        }

        public static void Repack(string folder, string outFolder)
        {
            var file = new FSTIM2();
            var xml = new XmlDocument();
            xml.Load(Path.Combine(folder, "_fstim2.xml"));
            string decoder = xml.ReadString("fstim2/decoder");
            if (decoder != Program.ProgramName)
                Console.WriteLine($"Unrecognized decoder: {decoder}");

            string outName = xml.ReadString("fstim2/filename");
            file.FormatVersion = xml.ReadByte("fstim2/formatversion");
            file.FormatID = (FormatAlignment)xml.ReadByte("fstim2/formatid");

            var picturesNode = xml.SelectNodes("fstim2/pictures/picture");
            if (picturesNode != null)
            {
                foreach (XmlNode pictureNode in picturesNode)
                {
                    // Read PNG
                    // TODO: Check clut and transform data if necessary
                    string pngName = pictureNode.ReadString("filename");
                    string pngPath = Path.Combine(folder, pngName);
                    if (!File.Exists(pngPath))
                        throw new FileNotFoundException($"Could not find PNG: {pngPath}", pngName);
                    ImageHelper.ReadPNG(pngPath, ushort.MaxValue, ushort.MaxValue, out Pixel[] image, out Color[] clut, out bool pngIndexed, out int pngBitDepth, out int lv0Width, out int lv0Height);
                    
                    byte mipmapTextures = pictureNode.ReadByte("mipmaptextures");
                    ushort width = pictureNode.ReadUInt16("width");
                    ushort height = pictureNode.ReadUInt16("height");
                    var imageType = Picture.GetColorTypeByName(pictureNode.ReadString("imagetype"));
                    bool indexed = imageType == Picture.ColorType.IndexColor4 || imageType == Picture.ColorType.IndexColor8;
                    int bitDepth = imageType == Picture.ColorType.IndexColor4 ? 4 : 8;
                    ImageHelper.NormalizePixelFormat(width, height, pngBitDepth, bitDepth, pngIndexed, indexed, true, image, clut, out image, out clut);

                    var picture = new Picture(width, height, (byte)(mipmapTextures - 1))
                    {
                        Image = image,
                        Clut = clut,
                        PictureFormat = pictureNode.ReadByte("pictformat"),
                        ImageColorType = imageType,
                        WriteExtendedHeader = pictureNode.ReadBooleanOrDefault("extendedheader", false),
                        Comment = pictureNode.ReadStringOrDefault("comment", string.Empty)
                    };

                    if (indexed)
                    {
                        picture.ClutType = new Picture.ClutTypeConfig(
                            Picture.GetColorTypeByName(pictureNode.ReadString("cluttype/clutcolortype")),
                            pictureNode.ReadBoolean("cluttype/clutcompound"),
                            pictureNode.ReadEnum<Picture.ClutStorageModeType>("cluttype/csm"));
                    }

                    picture.GsTex.TextureBasePointer = pictureNode.ReadUInt16("gstex0/tbp0");
                    picture.GsTex.TextureBufferWidth = pictureNode.ReadByte("gstex0/tbw");
                    picture.GsTex.PixelStorageMode = pictureNode.ReadEnum<Picture.PixelStorageModeType>("gstex0/psm");
                    picture.GsTex.TextureWidth = pictureNode.ReadByte("gstex0/tw");
                    picture.GsTex.TextureHeight = pictureNode.ReadByte("gstex0/th");
                    picture.GsTex.TextureColorComponent = pictureNode.ReadEnum<Picture.TextureColorComponentType>("gstex0/tcc");
                    picture.GsTex.TextureFunction = pictureNode.ReadEnum<Picture.TextureFunctionType>("gstex0/tfx");
                    picture.GsTex.ClutBasePointer = pictureNode.ReadByte("gstex0/cbp");
                    picture.GsTex.ClutPixelStorageMode = pictureNode.ReadEnum<Picture.PixelStorageModeType>("gstex0/cpsm");
                    picture.GsTex.ClutStorageMode = pictureNode.ReadEnum<Picture.ClutStorageModeType>("gstex0/csm");
                    picture.GsTex.ClutStartAddress = pictureNode.ReadByte("gstex0/csa");
                    picture.GsTex.ClutLoadControl = pictureNode.ReadByte("gstex0/cld");

                    var gstex1Node = pictureNode.SelectSingleNode("gstex1");
                    if (gstex1Node != null)
                    {
                        picture.GsTex.LODCalculationMethod = gstex1Node.ReadByte("lcm");
                        picture.GsTex.MipLevelMax = gstex1Node.ReadByte("mxl");
                        picture.GsTex.MipMag = gstex1Node.ReadByte("mmag");
                        picture.GsTex.MipMin = gstex1Node.ReadByte("mmin");
                        picture.GsTex.MipmapTextureBaseAddress = gstex1Node.ReadByte("mtba");
                        picture.GsTex.LODParameterL = gstex1Node.ReadByte("l");
                        picture.GsTex.LODParameterK = gstex1Node.ReadUInt16("k");
                    }

                    var gstexafbapabeNode = pictureNode.SelectSingleNode("gstexafbapabe");
                    if (gstexafbapabeNode != null)
                    {
                        picture.GsTex.TA0 = gstexafbapabeNode.ReadByte("ta0");
                        picture.GsTex.AEM = gstexafbapabeNode.ReadByte("aem");
                        picture.GsTex.TA1 = gstexafbapabeNode.ReadByte("ta1");
                        picture.GsTex.PABE = gstexafbapabeNode.ReadByte("pabe");
                        picture.GsTex.FBA = gstexafbapabeNode.ReadByte("fba");
                    }

                    var gstexclutNode = pictureNode.SelectSingleNode("gstexclut");
                    if (gstexclutNode != null)
                    {
                        picture.GsTex.ClutBufferWidth = gstexclutNode.ReadByte("cbw");
                        picture.GsTex.ClutOffsetU = gstexclutNode.ReadByte("cou");
                        picture.GsTex.ClutOffsetV = gstexclutNode.ReadUInt16("cov");
                    }

                    if (mipmapTextures > 1)
                    {
                        var gsmiptbp1Node = pictureNode.SelectSingleNode("mipmaps/gsmiptbp1");
                        if (gsmiptbp1Node != null)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                int level = i + 1;
                                picture.MipmapTextureBasePointers[i] = gsmiptbp1Node.ReadUInt16($"tbp{level}");
                                picture.MipmapTextureBufferWidths[i] = gsmiptbp1Node.ReadByte($"tbw{level}");
                            }
                        }

                        var gsmiptbp2Node = pictureNode.SelectSingleNode("mipmaps/gsmiptbp2");
                        if (gsmiptbp2Node != null)
                        {
                            for (int i = 3; i < 6; i++)
                            {
                                int level = i + 1;
                                picture.MipmapTextureBasePointers[i] = gsmiptbp2Node.ReadUInt16($"tbp{level}");
                                picture.MipmapTextureBufferWidths[i] = gsmiptbp2Node.ReadByte($"tbw{level}");
                            }
                        }

                        var filenameNodes = pictureNode.SelectNodes("mipmaps/filenames/filename");
                        if (filenameNodes != null)
                        {
                            foreach (XmlNode filenameNode in filenameNodes)
                            {
                                // Read PNG
                                // TODO: Check clut and transform data if necessary
                                pngName = pictureNode.ReadString("filename");
                                pngPath = Path.Combine(folder, pngName);
                                if (!File.Exists(pngPath))
                                    throw new FriendlyException($"Could not find mipmap PNG: {pngPath}");
                                ImageHelper.ReadPNG(pngPath, ushort.MaxValue, ushort.MaxValue, out image, out clut, out pngIndexed, out pngBitDepth, out int mipWidth, out int mipHeight);
                                ImageHelper.NormalizePixelFormat(mipWidth, mipHeight, pngBitDepth, bitDepth, pngIndexed, indexed, false, image, clut, out image, out clut);
                                picture.Mipmaps.Add(image);
                            }
                        }
                    }

                    var userdataNode = pictureNode.SelectSingleNode("userdata/data");
                    if (userdataNode != null)
                    {
                        picture.UserData = Convert.FromBase64String(userdataNode.InnerText);
                    }

                    file.Pictures.Add(picture);
                }
            }

            string outPath = Path.Combine(outFolder, outName);
            FileHelper.Backup(outPath);
            file.Write(outPath);
        }
    }
}
