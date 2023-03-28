# FileSigChecker
 Simple CLI application for file type (signature) check by magical numbers.

## Requirements:

.NET 6 Runtime

## OS Support

Windows, Linux and MacOS

## Description

FileSigChecker will check the first 50 bytes of a file and searches in ext_list.txt for it and returns da found file type.
The file ext_list.txt must be created in the FileSigChecker binary location and data should look similar to the following image:

![alt text](https://github.com/0x78654C/FileSigChecker/blob/main/Media/ext_list.png?raw=true)

The data in line is must be split by ' | ' character.
Example of ext_list:

 ```
4D 5A|COM, DLL, DRV, EXE, PIF, QTS, QTX, SYS| Windows executable files.
FF D8 FF|JFIF, JPE, JPEG, JPG| 	JPEG/JFIF graphics file
25 50 44 46|PDF|Adobe Portable Document Format, Forms Document Format, and Illustrator graphics files
FF Ex|MP3|MPEG audio file frame synch pattern
FF Fx|MP3|MPEG audio file frame synch pattern
66 74 79 70 4D 53 4E 56|MP4|MPEG-4 video file
66 74 79 70 69 73 6F 6D|MP4|MPEG-4 video file
89 50 4E 47 0D 0A 1A 0A|PNG|Portable Network Graphics file
 ```