using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Amicitia.IO.Binary;
using AtlusScriptLibrary.Common.Text.Encodings;

namespace P5NameTBLEditor
{
    internal class Program
    {
        public static int tblNumber = 38; // number of Name TBL sections, 34 for P5, 38 for P5R

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                System.Console.WriteLine("P5RNameTBLEditor:\nUsage:\nDrag and Drop either a NAME.TBL or a NAME tbl folder into the program's exe\nPress any key to exit");
                Console.ReadKey();
            }
            else
            {
                FileInfo arg0 = new FileInfo(args[0]);
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                if (arg0.Name.ToLower().Contains("name") && arg0.Name.ToLower().Contains(".tbl"))
                {
                    DumpNameTBL(args);
                }
                else if (arg0.Name == "NAME")
                {
                    Console.WriteLine("NAME tbl folder was input\n");
                    CreateNameTBLFromFolder(args);
                }
                else Console.WriteLine("https://youtu.be/huTMNGaqoAA");
            }
        }

        static void DumpNameTBL(string[] args)
        {
            FileInfo arg0 = new FileInfo(args[0]);
            Console.WriteLine($"Attempting to convert {arg0.Name}");

            var allNameTBLLists = new List<List<string>>();

            using (BinaryObjectReader NAMETBLFile = new BinaryObjectReader(args[0], Endianness.Big, AtlusEncoding.Persona5RoyalEFIGS))
            {
                for (int i = 0; i < tblNumber / 2; i++)
                {
                    Console.WriteLine($"Reading name data for {GetTBLDirName(tblNumber, i)}");

                    var NameTBLStrings = new List<String>();
                    List<UInt16> StringPointers = new List<UInt16>();

                    int filesize = NAMETBLFile.ReadInt32();

                    int numOfPointers = filesize / 2;

                    for (int j = 0; j < numOfPointers; j++)
                    {
                        StringPointers.Add(NAMETBLFile.ReadUInt16());
                        //if ( i == 0 ) Console.WriteLine($"Current Position after reading pointer is { NAMETBLFile.Position }\n");
                    }

                    int targetPadding = (int)((0x10 - NAMETBLFile.Position % 0x10) % 0x10);
                    if (targetPadding > 0)
                    {
                        //Console.WriteLine($"Padding at address { NAMETBLFile.Position }\n");
                        NAMETBLFile.Seek(targetPadding, SeekOrigin.Current);
                    }

                    long basePos = NAMETBLFile.Position; // save position before strings
                                                         //Console.WriteLine($"Current Position is { basePos }\n");

                    for (int j = 0; j < numOfPointers; j++)
                    {
                        //Console.WriteLine($"Following String pointer to { basePos + StringPointers[j] + 4 }\n");
                        NAMETBLFile.Seek(basePos + StringPointers[j] + 4, SeekOrigin.Begin);

                        var TargetString = NAMETBLFile.ReadString(StringBinaryFormat.NullTerminated);

                        if ((byte)TargetString[TargetString.Length - 1] == 10)
                        {
                            //Console.WriteLine($"Found newline end on line { j + 1 }\n");
                            TargetString = TargetString.Remove(TargetString.Length - 1, 1);
                        }
                        NameTBLStrings.Add(TargetString);
                    }

                    //Console.WriteLine($"Current Position after reading all strings is { NAMETBLFile.Position }\n");

                    targetPadding = (int)((0x10 - NAMETBLFile.Position % 0x10) % 0x10);
                    if (targetPadding > 0)
                    {
                        NAMETBLFile.Seek(targetPadding, SeekOrigin.Current);
                    }

                    allNameTBLLists.Add(NameTBLStrings);
                }
            }

            Console.WriteLine();

            for (int i = 0; i < allNameTBLLists.Count; i++)
            {
                var savePath = Path.Combine(Path.GetDirectoryName(args[0]), "NAME");
                Directory.CreateDirectory(savePath);
                savePath = Path.Combine(savePath, $"{i:D2}" + " - " + GetTBLDirName(tblNumber, i) + ".txt");
                File.WriteAllLines(savePath, allNameTBLLists[i], Encoding.UTF8);
                Console.WriteLine($"Saving {i:D2} - {GetTBLDirName(tblNumber, i)}.txt");
            }
            System.Threading.Thread.Sleep(1000);
        }
        
        static void CreateNameTBLFromFolder(string[] args)
        {
            FileInfo arg0 = new FileInfo(args[0]);
            var savePath = Path.Combine(arg0.FullName + ".TBL");
            Console.WriteLine(savePath);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using (BinaryObjectWriter NAMETBLFile = new BinaryObjectWriter(savePath, Endianness.Big, AtlusEncoding.Persona5RoyalEFIGS))
            {
                for (int i = 0; i < tblNumber / 2; i++)
                {
                    string[] NameTBLStrings = Array.Empty<string>();

                    string targetTXTFile = Path.TrimEndingDirectorySeparator(arg0.FullName) + "\\" + $"{i:D2} - " + GetTBLDirName(tblNumber, i) + ".txt";
                    Console.WriteLine("Reading txt file " + targetTXTFile);

                    NameTBLStrings = File.ReadAllLines(targetTXTFile, AtlusEncoding.Persona5RoyalEFIGS);

                    List<long> StringPointers = new List<long>();

                    long fileSizePosition = NAMETBLFile.Position;
                    NAMETBLFile.WriteUInt32(0); // filesize

                    int numOfPointers = NameTBLStrings.Length;
                    //Console.WriteLine($"Expecting { numOfPointers } pointers for section {i}\n");

                    long StringPointersLocation = NAMETBLFile.Position;
                    for (int j = 0; j < numOfPointers; j++)
                    {
                        NAMETBLFile.WriteUInt16(0); // write dummy pointers
                                                    //if ( i == 0 ) Console.WriteLine($"Current Position after reading pointer is { NAMETBLFile.Position }\n");
                    }

                    uint filesize = (uint)(NAMETBLFile.Position - fileSizePosition) - 4;

                    int targetPadding = (int)((0x10 - NAMETBLFile.Position % 0x10) % 0x10);
                    if (targetPadding > 0)
                    {
                        for (int j = 0; j < targetPadding; j++)
                        {
                            NAMETBLFile.WriteByte((byte)0);
                        }
                    }

                    long basePos = NAMETBLFile.Position; // save position before strings

                    NAMETBLFile.Seek(fileSizePosition, SeekOrigin.Begin); // seek back to fix filesize
                    NAMETBLFile.WriteUInt32(filesize); // filesize
                    NAMETBLFile.Seek(basePos, SeekOrigin.Begin); // go back to where we were


                    fileSizePosition = NAMETBLFile.Position;

                    //Write Strings
                    NAMETBLFile.WriteUInt32(0); // filesize

                    for (int j = 0; j < numOfPointers; j++)
                    {
                        StringPointers.Add(NAMETBLFile.Position - (fileSizePosition + 4));
                        NAMETBLFile.WriteString(StringBinaryFormat.NullTerminated, NameTBLStrings[j]);
                    }
                    filesize = (uint)(NAMETBLFile.Position - fileSizePosition) - 4;

                    //Console.WriteLine($"Current Position after reading all strings is { NAMETBLFile.Position }\n");

                    targetPadding = (int)((0x10 - NAMETBLFile.Position % 0x10) % 0x10);
                    if (targetPadding > 0)
                    {
                        for (int j = 0; j < targetPadding; j++)
                        {
                            NAMETBLFile.WriteByte((byte)0);
                        }
                    }

                    basePos = NAMETBLFile.Position; // save position before strings

                    NAMETBLFile.Seek(fileSizePosition, SeekOrigin.Begin); // seek back to fix filesize
                    NAMETBLFile.WriteUInt32(filesize); // filesize

                    NAMETBLFile.Seek(StringPointersLocation, SeekOrigin.Begin); // seek back to write Pointers
                    for (int j = 0; j < numOfPointers; j++)
                    {
                        NAMETBLFile.WriteUInt16((ushort)StringPointers[j]);
                    }


                    NAMETBLFile.Seek(basePos, SeekOrigin.Begin); // go back to where we were
                }
            }
        }
        static string GetTBLDirName(int tblNumber, int index)
        {
            // Initialization of array
            string[] tblNames = new string[] { "Arcanas", "Skills", "Enemies", "Personas", "Accessories", "Protectors", "Consumables",
                                               "Key Items", "Materials", "Melee Weapons", "Battle Actions", "Outfits", "Skill Cards",
                                               "Party FirstNames", "Party LastNames", "Confidant Names", "Ranged Weapons", "17",
                                               "18", "19", "20" };

            string[] tblNamesR = new string[] { "Arcanas", "Skills", "Skills Again", "Enemies", "Personas", "Traits", "Accessories",
                                                "Protectors", "Consumables", "Key Items", "Materials", "Melee Weapons", "Battle Actions",
                                                "Outfits", "Skill Cards", "Party FirstNames", "Party LastNames", "Confidant Names", 
                                                "Ranged Weapons", "39", "39", "39", "39" };

            if ( tblNumber == 34 )
            {
                return tblNames[index];
            }
            else return tblNamesR[index];
        }
    }
}
