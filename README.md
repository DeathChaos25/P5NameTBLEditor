# P5NameTBLEditor 

**NAME.TBL Editor for Persona 5/Royal**  
*A simple drag-and-drop tool for editing P5(R) name tables*

[![Latest Release](https://img.shields.io/github/v/release/DeathChaos25/P5NameTBLEditor?style=for-the-badge&label=Download)](https://github.com/DeathChaos25/P5NameTBLEditor/releases/latest)
[![Requires .NET 9+](https://img.shields.io/badge/.NET-9.0+-blue?style=for-the-badge)](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)

## How to Use

1. **Download** the [latest release](https://github.com/DeathChaos25/P5NameTBLEditor/releases/latest) (or click the download button above)
2. **Install** [.NET Desktop Runtime 9.0.0](https://dotnet.microsoft.com/en-us/download/dotnet/9.0) (or newer) if not already installed  (or click the .NET button above)
3. Extract the program from the downloaded 7z (using something like 7zip)

### Basic Usage (Drag & Drop)
- Drag a `NAME.TBL` file onto the tool's .EXE file to extract
- Drag an extracted `NAME` folder onto the tool's .EXE file to repack

### Advanced Usage (Command Line)
You can also run the program with command line arguments:

P5NameTBLEditor.exe "path\to\NAME.TBL\file\or\folder" [encoding]

Examples:  
P5NameTBLEditor.exe "C:\game_files\NAME.TBL"  
P5NameTBLEditor.exe "C:\game_files\NAME" P5R_CHS  

### Supported Encodings
- `P5`
- `P5_Chinese`
- `P5_Korean`
- `P5R_EFIGS` *(default)*
- `P5R_CHS` (Chinese Simplified)
- `P5R_CHT` (Chinese Traditional)
- `P5R_Japanese`
- `P5R_Korean`

## Credits
- Amicitia.IO: [https://github.com/tge-was-taken/Amicitia.IO](https://github.com/tge-was-taken/Amicitia.IO)
- Atlus-Script-Tools (for the character encoding): [https://github.com/tge-was-taken/Atlus-Script-Tools](https://github.com/tge-was-taken/Atlus-Script-Tools)
