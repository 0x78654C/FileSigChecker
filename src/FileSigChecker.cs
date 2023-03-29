/*
 * Author: x_Coding
 * Description: File signature checker.
 */
 
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;


namespace FileType
{
    class FileTypeChecker
    {
        private static string _binaryName = Assembly.GetExecutingAssembly().Location;
        private static string _extFileName = Path.GetDirectoryName(_binaryName) + @"\ext_list.txt";
        private static string _helpMessage { get; set; }

        // Main method.
        static void Main(string[] args)
        {
        	Console.WriteLine(_extFileName);
            _helpMessage = @" Usage of file extension tool:
 	FileSigChecker.exe <file_path>      : Display file path, extension, hex signature, and signature description.
 	FileSigChecker.exe <file_path> -ext : Display extension only.
 	FileSigChecker.exe -h               : Display this help message.";

            if (!CheckExtFile(_extFileName))
            {
                ColorConsoleWriteLine(ConsoleColor.Red,
                    "Error: File ext_list.txt is not present with file type signature. File must be located with the FileSigChecker executable!");
                return;
            }

            var extOnly = string.Empty;
            var fileParam = string.Empty;

            try
            {
                fileParam = args[0];
                extOnly = args[1];
            }
            catch (Exception e)
            {
                // Ignore.
            }

            if (fileParam.Contains("-h"))
            {
                Console.WriteLine(_helpMessage);
                return;
            }

            switch (extOnly)
            {
                case "-ext":
                    CheckFileSignature(_extFileName, fileParam, true);
                    return;
                case "":
                    CheckFileSignature(_extFileName, fileParam, false);
                    return;
                default:
                    CheckFileSignature(_extFileName, fileParam, false);
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
        public static void ColorConsoleWriteLine(ConsoleColor color, object data)
        {
            ConsoleColor currentForeground = Console.ForegroundColor;
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
            var extLines = File.ReadAllLines(extFileName);
            string hexFile = HexDump.GetHex(filePath);
            foreach (var line in extLines)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    var hex = line.Split('|')[0];
                    bool check = false;
                    if(hex.Length < 4) // In case hex signature is 1 byte.
                    	check  = hexFile.StartsWith(hex);
                    else
                    	check  = hexFile.Contains(hex);
                    	
                    if (check)
                    {
                        var ext = line.Split('|')[1];
                        var description = line.Split('|')[2];
                        string outMessage;
                        if (extOnly)
                            outMessage = ext;
                        else
                            outMessage = $@"
-------------------------------------------------------------	
File:          {filePath}
Extension(s): {ext}
Hex signature: {hex}
Description:   {description}
-------------------------------------------------------------";
                        Console.WriteLine(outMessage);
                        break;
                    }
                }
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
            string outHex = "";
            try
            {
                if (File.Exists(fileName))
                {
                    using (var stream = File.OpenRead(fileName))
                    {
                        int size = stream.Length < 50 ? (int)stream.Length : 50;
                        byte[] buffer = new byte[size];
                        var read = stream.Read(buffer, 0, size);
                        outHex = Hex(buffer, 50);
                    }
                }
            }
            catch (Exception e)
            {
                outHex = e.Message;
            }

            return outHex;
        }

        /// <summary>
        /// Hex dump
        /// </summary>
        /// <param name="bytes">Bytes input from file</param>
        /// <param name="bytesPerLine">Bytes per line. Default 16</param>
        /// <returns>string</returns>
        private static string Hex(byte[] bytes, int bytesPerLine = 50)
        {
            if (bytes == null) return "<null>";
            int bytesLength = bytes.Length;

            char[] HexChars = "0123456789ABCDEF".ToCharArray();


            int firstCharColumn = 0
                                  + bytesPerLine * 3
                                  + (bytesPerLine - 1) / 8;


            int lineLength = firstCharColumn
                             + bytesPerLine
                             + Environment.NewLine.Length;

            char[] line = (new String(' ', lineLength - Environment.NewLine.Length) + Environment.NewLine).ToCharArray();
            int expectedLines = (bytesLength + bytesPerLine - 1) / bytesPerLine;
            StringBuilder result = new StringBuilder(expectedLines * lineLength);

            for (int i = 0; i < bytesLength; i += bytesPerLine)
            {
                int hexColumn = 0;
                int charColumn = firstCharColumn;

                for (int j = 0; j < bytesPerLine; j++)
                {
                    if (i + j >= bytesLength)
                    {
                        line[hexColumn] = ' ';
                        line[hexColumn + 1] = ' ';
                    }
                    else
                    {
                        byte b = bytes[i + j];
                        line[hexColumn] = HexChars[(b >> 4) & 0xF];
                        line[hexColumn + 1] = HexChars[b & 0xF];
                    }

                    hexColumn += 3;
                }

                result.Append(line);
            }

            return result.ToString();
        }
    }
}