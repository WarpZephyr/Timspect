using System;
using System.IO;
using Timspect.Core.Formats;
using Timspect.Unpacker.Exceptions;
using Timspect.Unpacker.Unpackers;

namespace Timspect.Unpacker
{
    internal class Program
    {
        internal static string ProgramName = "Timspect.Unpacker";
        internal static string ProgramShortName = "Tsc";

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                return;
            }

            bool error = false;
            foreach (string path in args)
            {
#if !DEBUG
                try
                {
#endif
                if (File.Exists(path))
                {
                    error |= ProcessFile(path);
                }
                else if (Directory.Exists(path))
                {
                    error |= ProcessFolder(path);
                }
#if !DEBUG
                }
                catch (FriendlyException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    error = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error:\n {ex}");
                    error = true;
                }
#endif
            }

            if (error)
            {
                Console.WriteLine("One or more errors were encountered and displayed above.\nPress any key to exit.");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Process a path containing a file.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>Whether or not a minor error occurred.</returns>
        /// <exception cref="FriendlyException">An error occurred.</exception>
        private static bool ProcessFile(string path)
        {
            if (TIM2.IsRead(path, out TIM2? tim2))
            {
                Console.WriteLine("Unpacking TIM2...");
                string filename = Path.GetFileName(path);
                string? folder = Path.GetDirectoryName(path) ?? throw new FriendlyException($"Could not get folder path of: \"{path}\"");
                string outFolder = GetValidFolderName(folder, filename);
                Directory.CreateDirectory(outFolder);
                TIM2Unpacker.Unpack(filename, outFolder, tim2);
            }
            else if (path.EndsWith("_tim2.xml", StringComparison.CurrentCultureIgnoreCase))
            {
                Console.WriteLine("Repacking TIM2...");
                string? folder = Path.GetDirectoryName(path) ?? throw new FriendlyException($"Could not get folder path of: \"{path}\"");
                string? outFolder = Path.GetDirectoryName(folder) ?? throw new FriendlyException($"Could not get folder path of: \"{folder}\"");
                TIM2Unpacker.Repack(folder, outFolder);
            }
            else if (path.EndsWith("_fstim2.xml", StringComparison.CurrentCultureIgnoreCase))
            {
                Console.WriteLine("Repacking FromSoftware TIM2...");
                string? folder = Path.GetDirectoryName(path) ?? throw new FriendlyException($"Could not get folder path of: \"{path}\"");
                string? outFolder = Path.GetDirectoryName(folder) ?? throw new FriendlyException($"Could not get folder path of: \"{folder}\"");
                FSTIM2Unpacker.Repack(folder, outFolder);
            }
            else
            {
                try
                {
                    var fstim2 = FSTIM2.Read(path);

                    Console.WriteLine("Unpacking FromSoftware TIM2...");
                    string filename = Path.GetFileName(path);
                    string? folder = Path.GetDirectoryName(path) ?? throw new FriendlyException($"Could not get folder path of: \"{path}\"");
                    string outFolder = GetValidFolderName(folder, filename);
                    Directory.CreateDirectory(outFolder);
                    FSTIM2Unpacker.Unpack(filename, outFolder, fstim2);
                }
                catch
                {
                    Console.WriteLine($"File format not recognized: \"{Path.GetFileName(path)}\"");
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Process a path containing a folder.
        /// </summary>
        /// <param name="folder">The path to the folder.</param>
        /// <returns>Whether or not a minor error occurred.</returns>
        /// <exception cref="FriendlyException">An error occurred.</exception>
        private static bool ProcessFolder(string folder)
        {
            if (File.Exists(Path.Combine(folder, "_tim2.xml")))
            {
                Console.WriteLine("Repacking TIM2...");
                string? outFolder = Path.GetDirectoryName(folder) ?? throw new FriendlyException($"Could not get folder path of: \"{folder}\"");
                TIM2Unpacker.Repack(folder, outFolder);
            }
            else if (File.Exists(Path.Combine(folder, "_fstim2.xml")))
            {
                Console.WriteLine("Repacking FromSoftware TIM2...");
                string? outFolder = Path.GetDirectoryName(folder) ?? throw new FriendlyException($"Could not get folder path of: \"{folder}\"");
                FSTIM2Unpacker.Repack(folder, outFolder);
            }
            else
            {
                Console.WriteLine($"Found nothing to process for folder: \"{folder}\"");
                return true;
            }

            return false;
        }

        private static string GetValidFolderName(string folder, string filename)
        {
            string outFolder;
            if (!filename.Contains('.'))
            {
                outFolder = Path.Combine(folder, filename + $"-{ProgramShortName}");
            }
            else
            {
                outFolder = Path.Combine(folder, filename.Replace('.', '-'));
            }

            return outFolder;
        }
    }
}