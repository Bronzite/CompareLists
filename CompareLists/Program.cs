using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace CompareLists
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length == 0 )
            {
                Console.WriteLine("Generates TSV output indicating which lines are present in which files.");
                Console.WriteLine("SYNTAX: CompareLists file1 file2 [file3] ...");
                return;
            }
            string sDelimiterString = "\t"; //Default delimiter is tab.
            List<string> lstFiles = new List<string>(); //The files we'll be comparing.
            bool bFailedConfiguration = false;  //Set to true if a Configuration step failed fatally.
                                                // I do this so we can return everything that's wrong with
                                                // the statement the first time.

            string sOutfile = "";

            for(int iArgument=0;iArgument<args.Length;iArgument++)
            {
                if (args[iArgument].Trim().Length > 0) //Because this could get called with a zero-length argument from another application.
                {
                    if (args[iArgument].Substring(0, 1) == "-")
                    {
                        if (args[iArgument].Trim().Equals("-t", StringComparison.CurrentCulture))
                        {
                            //Explicit set Delimiter String to tab
                            sDelimiterString = "\t";
                        }
                        if (args[iArgument].Trim().Equals("--csv", StringComparison.CurrentCulture))
                        {
                            //Set Delimiter string to comma
                            sDelimiterString = ",";
                        }
                        if (args[iArgument].Trim().Equals("--custom", StringComparison.CurrentCulture))
                        {
                            //Set a customer delimiter string
                            if (iArgument == args.Length - 1) //There is no argument after this
                            {
                                Console.WriteLine("--custom requires a delimiter string specified after it.");
                                bFailedConfiguration = true;
                            }
                            sDelimiterString = args[++iArgument];
                        }
                        if (args[iArgument].Trim().Equals("-o", StringComparison.CurrentCulture))
                        {
                            //Set a customer delimiter string
                            if (iArgument == args.Length - 1) //There is no argument after this
                            {
                                Console.WriteLine("-o requires an output file name to be specified.");
                                bFailedConfiguration = true;
                            }
                            sOutfile = args[++iArgument];
                        }

                    }
                    else
                    {
                        if(File.Exists(args[iArgument]))
                            lstFiles.Add(args[iArgument]);
                        else
                        {
                            Console.WriteLine("File {0} not found.", args[iArgument]);
                            bFailedConfiguration = true;
                        }

                    }
                }
            }
            if(bFailedConfiguration) { return; } //If configuration failed, exit.
            Dictionary<string, int[]> dicStringPresenceArray = new Dictionary<string, int[]>();

            for(int iFileIndex = 0;iFileIndex < lstFiles.Count;iFileIndex++)
            {
                string[] sFileLines = File.ReadAllLines(lstFiles[iFileIndex]);  // TODO: We should look at the file size and
                                                                                // only use ReadAllLines on small files.
                for(int iLineIndex =0; iLineIndex < sFileLines.Length;iLineIndex++)
                {
                    string sTrimmedLine = sFileLines[iLineIndex].Trim();

                    if (!dicStringPresenceArray.ContainsKey(sTrimmedLine))  //If this line isn't already in the dictionary best add it.
                    {
                        dicStringPresenceArray.Add(sTrimmedLine, new int[lstFiles.Count]);
                    }

                    dicStringPresenceArray[sTrimmedLine][iFileIndex]++;
                }
            }

            //Configure output
            StreamWriter swOutput = null;
            if(sOutfile != "")
            {
                swOutput = new StreamWriter(sOutfile);
            }

            //Write out the output
            foreach(string sKey in dicStringPresenceArray.Keys)
            {
                string sOutputLine = sKey;
                for(int i=0;i<dicStringPresenceArray[sKey].Length;i++)
                {
                    sOutputLine = sOutputLine + sDelimiterString + dicStringPresenceArray[sKey][i].ToString();
                }
                if (swOutput != null)
                    swOutput.WriteLine(sOutputLine);
                else
                    Console.WriteLine(sOutputLine);
            }

            if(swOutput!=null)
            {
                //Make sure we wrote out every character.
                swOutput.Flush();
                swOutput.Close();
            }

        }
    }
}
