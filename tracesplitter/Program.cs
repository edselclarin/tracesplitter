using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace tracesplitter
{
    class Program
    {
        static void Main(string[] args)
        {
            // Validate argument.
            if (args.Length == 0)
            {
                Console.WriteLine("ERROR: Missing filename to split.");
                Console.WriteLine("USAGE: tracesplitter <filename>");
                return;
            }

            // Abort if file is empty.
            if (!File.Exists(args[0]))
            {
                Console.WriteLine("ERROR: " + args[0] + " does not exist.");
                return;
            }

            // Read all lines from the file.
            string[] lines = File.ReadAllLines(args[0]);

            // Create output files.
            const int max_channels = 8;
            FileStream[] streams = new FileStream[max_channels];
            StreamWriter[] writers = new StreamWriter[max_channels];
            string dir_name = Path.GetDirectoryName(args[0]);
            if (dir_name.Length == 0)
                dir_name = System.Environment.CurrentDirectory;
            string name_of_file = Path.GetFileNameWithoutExtension(args[0]);
            string file_ext = Path.GetExtension(args[0]);
            string[] out_filenames = new string[max_channels];
            for (int ch = 0; ch < max_channels; ch++)
            {
                out_filenames[ch] = String.Format("{0}\\{1}_{2}{3}", dir_name, name_of_file, ch, file_ext);
                streams[ch] = new FileStream(out_filenames[ch], FileMode.Create);
                writers[ch] = new StreamWriter(streams[ch]);
            }

            // Write line to file of corresponding channel.
            foreach (string line in lines)
            {
                int i = line.IndexOf(":");
                if (i > -1)
                {
                    string ch_str = line.Substring(0, i);
                    int ch = Convert.ToInt32(ch_str);
                    if (ch > -1 && ch < max_channels)
                        writers[ch].WriteLine(line);
                }
            }

            // Close all streams.
            for(int i = 0; i < streams.Length; i++)
            {
                if (streams[i].Length > 0)
                {
                    Console.WriteLine(streams[i].Name + " (" + streams[i].Length + " bytes)");
                    streams[i].Close();
                }
                else
                {
                    streams[i].Close();

                    // Erase empty files.
                    if (File.Exists(streams[i].Name))
                        File.Delete(streams[i].Name);
                }
            }
        }
    }
}
