using BinaryMemory;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using Timspect.Core.Graphics;

namespace Timspect.Core.Formats
{
    /// <summary>
    /// The texture format used in PS1 games. Implementation not complete.
    /// </summary>
    public class TIM : FileFormat<TIM>
    {
        #region Types

        /// <summary>
        /// The different types of TIM.
        /// </summary>
        public enum ColorType : uint
        {
            /// <summary>
            /// 4-bits per pixel with no color lookup table.
            /// </summary>
            BPP4NoCLUT = 0,

            /// <summary>
            /// 8-bits per pixel with no color lookup table.
            /// </summary>
            BPP8NoCLUT = 1,

            /// <summary>
            /// 16-bits per pixel, does not need a color lookup table.
            /// </summary>
            BPP16 = 2,

            /// <summary>
            /// 24-bits per pixel, does not need a color lookup table.
            /// </summary>
            BPP24 = 3,

            /// <summary>
            /// 4-bits per pixel.
            /// </summary>
            BPP4 = 8,

            /// <summary>
            /// 8-bits per pixel.
            /// </summary>
            BPP8 = 9
        }

        #endregion

        #region Members

        /// <summary>
        /// The type, determining how many bits per pixel and if there is a color lookup table.
        /// </summary>
        public ColorType Type { get; set; }

        /// <summary>
        /// Color lookup table X location in memory if one is present.
        /// </summary>
        public ushort CLUT_X { get; set; }

        /// <summary>
        /// Color lookup table Y location in memory if one is present.
        /// </summary>
        public ushort CLUT_Y { get; set; }

        /// <summary>
        /// Image X location in memory.
        /// </summary>
        public ushort ImageX { get; set; }

        /// <summary>
        /// Image Y location in memory.
        /// </summary>
        public ushort ImageY { get; set; }

        /// <summary>
        /// The width of the image.
        /// </summary>
        public ushort Width { get; set; }

        /// <summary>
        /// The height of the image.
        /// </summary>
        public ushort Height { get; set; }

        /// <summary>
        /// All of the different palettes in this TIM.
        /// <para>Will be empty if not supported.</para>
        /// </summary>
        public List<Color[]> Palettes { get; set; }

        /// <summary>
        /// All of the pixels in this TIM.
        /// <para>Will be set to the first palette if supported.</para>
        /// </summary>
        public Pixel[] Image { get; set; }

        /// <summary>
        /// Gets the bits per pixel depending on the version.
        /// </summary>
        public int BitsPerPixel
        {
            get
            {
                switch (Type)
                {
                    case ColorType.BPP4NoCLUT:
                    case ColorType.BPP4:
                        return 4;
                    case ColorType.BPP8NoCLUT:
                    case ColorType.BPP8:
                        return 8;
                    case ColorType.BPP16:
                        return 16;
                    case ColorType.BPP24:
                        return 24;
                    default:
                        throw new NotSupportedException($"{nameof(Type)} {Type} is not supported for {nameof(BitsPerPixel)}");
                }
            }
        }

        #endregion

        #region Constructors

        public TIM()
        {
            Palettes = [];
            Image = [];
        }

        #endregion

        #region Is

        /// <summary>
        /// Returns true if data appears to be a TIM texture.
        /// </summary>
        protected override bool Is(BinaryStreamReader br)
        {
            if (br.Length - br.Position < 20)
            {
                return false;
            }

            uint magic = br.ReadUInt32();
            ColorType type = (ColorType)br.ReadUInt32();
            bool bTypeAssert = type == ColorType.BPP8NoCLUT || type == ColorType.BPP4NoCLUT || type == ColorType.BPP16 || type == ColorType.BPP24 || type == ColorType.BPP4 || type == ColorType.BPP8;

            return magic == 0x10 && bTypeAssert;
        }

        #endregion

        #region Read

        /// <summary>
        /// Reads a <see cref="TIM"/> from a stream.
        /// </summary>
        /// <param name="br">The stream reader.</param>
        protected override void Read(BinaryStreamReader br)
        {
            br.BigEndian = false;
            br.AssertUInt32(0x10);
            Type = br.ReadEnum32<ColorType>();

            // Get CLUT info
            Color[] clut;
            switch (Type)
            {
                case ColorType.BPP4:
                case ColorType.BPP8:
                    br.ReadInt32(); // CLUT length
                    CLUT_X = br.ReadUInt16();
                    CLUT_Y = br.ReadUInt16();
                    Palettes = ReadPaletteColors(br);
                    if (Palettes.Count < 1)
                    {
                        throw new InvalidDataException($"Cannot use an indexed type without a palette: {Type}");
                    }

                    clut = Palettes[0];
                    break;
                default:
                    clut = [];
                    break;
            }

            // Read image info
            int dataLength = br.ReadInt32();
            ImageX = br.ReadUInt16();
            ImageY = br.ReadUInt16();
            Width = GetTrueWidth(br.ReadUInt16());
            Height = br.ReadUInt16();

            // Read image
            byte[] image = br.ReadBytes(dataLength - 12);
            Image = ReadPixels(Width, Height, Type, image, clut);
        }

        private static List<Color[]> ReadPaletteColors(BinaryStreamReader br)
        {
            ushort colorCount = br.ReadUInt16();
            ushort paletteCount = br.ReadUInt16();
            var palettes = new List<Color[]>(paletteCount);
            for (int i = 0; i < paletteCount; i++)
            {
                var palette = new Color[colorCount];
                for (int colorIndex = 0; colorIndex < colorCount; colorIndex++)
                {
                    palette[colorIndex] = ColorDecode.FromA1B5G5R5(br.ReadUInt16());
                }

                palettes.Add(palette);
            }

            return palettes;
        }

        private static Pixel[] ReadPixels(ushort width, ushort height, ColorType colorType, byte[] image, Color[] clut)
        {
            int imageLength = width * height;
            Pixel[] pixels = new Pixel[imageLength];

            int pixelIndex = 0;
            for (int i = 0; i < imageLength; i++)
            {
                switch (colorType)
                {
                    case ColorType.BPP4NoCLUT:
                        // Do two at once
                        byte rawValue4 = image[i];
                        int value1 = rawValue4 & 0b00001111;
                        int value2 = rawValue4 >> 4;
                        pixels[pixelIndex++] = new Pixel(Color.FromArgb(value1, value1, value1));
                        pixels[pixelIndex++] = new Pixel(Color.FromArgb(value2, value2, value2));
                        i++;
                        break;
                    case ColorType.BPP4:
                        // Do two at once
                        byte indexRawValue = image[i];
                        int index1 = indexRawValue & 0b00001111;
                        int index2 = indexRawValue >> 4;
                        pixels[pixelIndex++] = new Pixel(clut[index1], index1);
                        pixels[pixelIndex++] = new Pixel(clut[index2], index2);
                        i++;
                        break;
                    case ColorType.BPP8NoCLUT:
                        byte value8 = image[i];
                        pixels[pixelIndex++] = new Pixel(Color.FromArgb(value8, value8, value8));
                        i++;
                        break;
                    case ColorType.BPP8:
                        int index = image[i];
                        pixels[pixelIndex++] = new Pixel(clut[index], index);
                        i++;
                        break;
                    case ColorType.BPP16:
                        int rawValue16 = image[i + 1] << 8 | image[i];
                        pixels[pixelIndex++] = new Pixel(ColorDecode.FromA1B5G5R5(rawValue16), -1);
                        i += 2;
                        break;
                    case ColorType.BPP24:
                        pixels[pixelIndex++] = new Pixel(Color.FromArgb(image[i], image[i + 1], image[i + 2]), -1);
                        i += 3;
                        break;
                    default:
                        throw new NotSupportedException($"Invalid {nameof(ColorType)} for {nameof(ReadPixels)}: {colorType}");
                }
            }

            return pixels;
        }

        private ushort GetTrueWidth(ushort width)
        {
            switch (Type)
            {
                case ColorType.BPP4NoCLUT:
                case ColorType.BPP4:
                    return (ushort)(width * 4);
                case ColorType.BPP8NoCLUT:
                case ColorType.BPP8:
                    return (ushort)(width * 2);
                case ColorType.BPP16:
                    return width;
                case ColorType.BPP24:
                    return (ushort)(width / 2);
                default:
                    throw new NotSupportedException($"{nameof(Type)} {Type} is not supported or implemented.");
            }
        }

        #endregion

        #region Write

        /// <summary>
        /// Writes a TIM to a stream.
        /// </summary>
        protected override void Write(BinaryStreamWriter bw)
        {
            throw new NotSupportedException("Writing is not supported for now.");
        }

        #endregion

        #region Pixels

        public Pixel GetPixel(int x, int y)
        {
            return Image[MathHelper.GetIndex2D(x, y, Width)];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Color GetPixelColor(int x, int y)
        {
            return GetPixel(x, y).Color;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetPixelIndex(int x, int y)
        {
            return GetPixel(x, y).Index;
        }

        public void SetPixel(Pixel pixel, int x, int y)
        {
            Image[MathHelper.GetIndex2D(x, y, Width)] = pixel;
        }

        public void SetPixelColor(Color color, int x, int y)
        {
            var pixel = GetPixel(x, y);
            pixel.Color = color;
            SetPixel(pixel, x, y);
        }

        public void SetPixelIndex(int index, int x, int y)
        {
            var pixel = GetPixel(x, y);
            pixel.Index = index;
            SetPixel(pixel, x, y);
        }

        #endregion

        #region Palette

        /// <summary>
        /// Switches pixels to use colors from another palette by index.
        /// </summary>
        /// <param name="paletteIndex">The index of the palette to use.</param>
        public void SwitchPalette(int paletteIndex)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(paletteIndex, Palettes.Count);
            var palette = Palettes[paletteIndex];

            int x = 0;
            int y = 0;
            for (int i = 0; i < Image.Length; i++)
            {
                if (x == Width)
                {
                    x = 0;
                    y++;
                }

                int pixelIndex = MathHelper.GetIndex2D(x, y, Width);
                Image[pixelIndex].Color = palette[Image[pixelIndex].Index];
            }
        }

        #endregion
    }
}
