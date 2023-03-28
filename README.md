# FileSigChecker
 Simple CLI applicaiton for file type (signature) check by magical numbers.

## Requirements:

.NET 6 Runtime

## OS Support

Windows, Linux and MacOS

## Description

FileSigChecker will check the first 50 bytes of a file and searches in ext_list.txt for it and returns da found file type.
The file ext_list.txt must be created in the FileSigChecker binary location and data should look similar to the following image:

![alt text](https://github.com/0x78654C/FileSigChecker/blob/main/Media/ext_list.png?raw=true)

The data in line is must be splited by '|' caracter.