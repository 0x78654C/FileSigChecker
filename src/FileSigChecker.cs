/*
 * Author: x_Coding
 * Description: File signature checker.
 */

using System.Text;

namespace FileSigChecker
{
    class FileTypeChecker
    {
        private const string ExtFileName = "ext_list.txt";
        private static string s_helpMessage { get; set; }
        private static bool s_isIterated = false;


        // Main method.
        static void Main(string[] args)
        {
            s_helpMessage = @" Usage of file extension tool:
 	FileSigChecker.exe <file_path>      : Display file path, extension, hex signature, and signature description.
 	FileSigChecker.exe <file_path> -ext : Display extension only.
 	FileSigChecker.exe -h               : Display this help message.
 	
Version: 1.0.1
Author: x_Coding (xcoding.dev@gmail.com)
 	";
            if (!CheckExtFile(ExtFileName))
            {
                ColorConsoleWriteLine(ConsoleColor.Red,
                    "Error: File ext_list.txt is not present with file type signature. File must be located with the FileSigChecker executable!");
                return;
            }

            if (args.Length < 1)
            {
                Console.WriteLine("At least one param is required");
                return;
            }

            var extOnly = string.Empty;
            var fileParam = args[0];
            if (args.Length > 1)
            {
                extOnly = args[1];
            }

            if (fileParam.Contains("-h"))
            {
                Console.WriteLine(s_helpMessage);
                return;
            }

            if (!File.Exists(fileParam))
            {
                ColorConsoleWriteLine(ConsoleColor.Red,
                    $"Error: File '{fileParam}' does not exist!");
                return;
            }


            switch (extOnly)
            {
                case "-ext":
                    CheckFileSignature(ExtFileName, fileParam, true);
                    return;
                case "":
                    CheckFileSignature(ExtFileName, fileParam, false);
                    return;
                default:
                    CheckFileSignature(ExtFileName, fileParam, false);
                    return;
            }
        }

        /// <summary>
        /// Check if a file exist.
        /// </summary>
        /// <param name="extFileName"> File path.</param>
        /// <returns>void</returns>
        private static bool CheckExtFile(string extFileName) => File.Exists(extFileName);

        /// <summary>
        /// Change color of a specific text in console.
        /// </summary>
        /// <param name="color">Console color</param>
        /// <param name="text"></param>
        /// <returns>void</returns>
        private static void ColorConsoleWriteLine(ConsoleColor color, object data)
        {
            var currentForeground = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(data);
            Console.ForegroundColor = currentForeground;
        }


        /// <summary>
        /// Check file signature by magic numbers compared to a whitelist of singatures.
        /// </summary>
        /// <param name="extFileName">File list with file type signature.</param>
        /// <param name="fileName">File path to be checked.</param>
        /// <param name="extOnly">Check if display extension only.</param>
        /// <returns>void</returns>
        private static void CheckFileSignature(string extFileName, string filePath, bool extOnly)
        {
            var outMessage = "Unknown signature.";
            var extLines = File.ReadAllLines(extFileName);
            var hexFile = HexDump.GetHex(filePath);
            var fileInfo = new FileInfo(filePath);
            var nameExt = fileInfo.Extension.Replace(".", "");
            var isExtFound = false;
            foreach (var line in extLines.Where(line => !string.IsNullOrEmpty(line)))
            {
                var hex = line.Split('|')[0];
                var check = hexFile.Contains(hex);
                var ext = line.Split('|')[1];
                var description = line.Split('|')[2];

                if (!check || (!s_isIterated && !ext.Contains(nameExt))) continue;
                isExtFound = true;
                outMessage = extOnly
                    ? ext
                    : $@"
-------------------------------------------------------------	
File:          {filePath}
Extension(s):  {ext}
Hex signature: {hex}
Description:   {description}
-------------------------------------------------------------";
                break;
            }

            if (isExtFound)
                Console.WriteLine(outMessage);
            else if (!s_isIterated)
            {
                s_isIterated = true;
                CheckFileSignature(extFileName, filePath, extOnly);
            }
        }
    }

    public class HexDump
    {
        /// <summary>
        /// Hex dump
        /// </summary>
        /// <param name="fileName">File name for dump hex</param>
        /// <returns>string</returns>
        public static string GetHex(string fileName)
        {
            var outHex = "";
            try
            {
                if (File.Exists(fileName))
                {
                    using (var stream = File.OpenRead(fileName))
                    {
                        var size = stream.Length < 50 ? (int) stream.Length : 50;
                        var buffer = new byte[size];
                        var read = stream.Read(buffer, 0, size);
                        outHex = Hex(buffer, size);
                    }
                }
            }
            catch (Exception e)
            {
                outHex = e.Message;
            }

            return outHex;
        }

        private static char[] s_hexChars { get; } = "0123456789ABCDEF".ToCharArray();

        /// <summary>
        /// Hex dump
        /// </summary>
        /// <param name="bytes">Bytes input from file</param>
        /// <param name="bytesPerLine">Bytes per line. Default 16</param>
        /// <returns>string</returns>
        private static string Hex(byte[] bytes, int bytesPerLine = 50)
        {
            if (bytes is null) return "<null>";
            var bytesLength = bytes.Length;

            var firstCharColumn = 0
                                  + bytesPerLine * 3
                                  + (bytesPerLine - 1) / 8;


            var lineLength = firstCharColumn
                             + bytesPerLine
                             + Environment.NewLine.Length;

            var line = (new string(' ', lineLength - Environment.NewLine.Length) + Environment.NewLine).ToCharArray();
            var expectedLines = (bytesLength + bytesPerLine - 1) / bytesPerLine;
            var result = new StringBuilder(expectedLines * lineLength);

            for (var i = 0; i < bytesLength; i += bytesPerLine)
            {
                var hexColumn = 0;

                for (var j = 0; j < bytesPerLine; j++)
                {
                    if (i + j >= bytesLength)
                    {
                        line[hexColumn] = ' ';
                        line[hexColumn + 1] = ' ';
                    }
                    else
                    {
                        var b = bytes[i + j];
                        line[hexColumn] = s_hexChars[(b >> 4) & 0xF];
                        line[hexColumn + 1] = s_hexChars[b & 0xF];
                    }

                    hexColumn += 3;
                }

                result.Append(line);
            }

            return result.ToString();
        }
    }
}