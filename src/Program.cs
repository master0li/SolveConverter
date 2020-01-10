using CommandLine;
using System;
using System.IO;

namespace SolveConverter
{
    class Program
    {
        public class Options
        {
            [Option('i', "Input", Required = true, HelpText = "File to convert")]
            public string InputFile { get; set; }
            [Option('o', "Output", Default = "", Required = false, HelpText = "File to output")]
            public string OutputFile { get; set; }
        }

        static void Main(string[] args)
        {

            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                       if (o.InputFile != "")
                       {
                           Console.WriteLine("Converting - {0}",o.InputFile);

                           ConvertTwistyTimerToCSTimer(o.InputFile);

                           Console.WriteLine("Finished {0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
                       }
                   });
            
        }

        public static void ConvertTwistyTimerToCSTimer(string solvesFile)
        {

            string line = "";
            string strTime = "";
            string strScramble = "";
            string strDate = "";

            const string minPrefix = "00:0";
            const string hourPrefix = "00:";

            TimeSpan tsTime;
            DateTime dtDate;

            FileStream fileStream = new FileStream(solvesFile, FileMode.Open);
            using (StreamReader streamReader = new StreamReader(fileStream))
            {
                while (!streamReader.EndOfStream)
                {
                    line = streamReader.ReadLine();

                    string[] solve = line.Split(',');

                    if (solve.Length >= 3)
                    {
                        strTime = solve[0].Replace("\"","");

                        if (strTime.Contains(":"))
                        {
                            strTime = hourPrefix + strTime;
                        }
                        else
                        {
                            strTime = hourPrefix + minPrefix + strTime;
                        }


                        try 
                        { 
                            tsTime = TimeSpan.ParseExact(strTime, "g", null); 
                        }
                        catch (Exception ex)
                        {
                            string m = ex.Message;

                            tsTime = new TimeSpan();
                        }
                        
                        strScramble = solve[1].Replace("\"", "");


                        strDate = solve[2].Replace("\"", "");

                        try
                        {
                            dtDate = DateTime.Parse(strDate);
                        }
                        catch (Exception ex)
                        {
                            string m = ex.Message;

                            dtDate = new DateTime();
                        }

                        DateTimeOffset dtoDate = dtDate;

                        Console.Write(",[[0,{0}],\"{1}\",{2}]", tsTime.TotalMilliseconds.ToString(), strScramble, dtoDate.ToUnixTimeSeconds() );
                    }
                    else
                    {
                        Console.WriteLine("Invalid Solve!");
                    }

                }                                               
            }
        }
    }
}
