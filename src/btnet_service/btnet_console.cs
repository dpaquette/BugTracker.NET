//compile like so:
//csc btnet_console.cs POP3Main.cs POP3Client.cs

using System;
using System.Timers;

namespace btnet
{

    public class console
    {
        ///////////////////////////////////////////////////////////////////////
        public static void Main(string[] args)
        {

            // check the command line
            if (args.Length != 1)
            {
                Console.WriteLine(
                    "usage\nbtnet_console.exe [path to btnet_service.exe.config file]");
                Console.WriteLine(
                    "example\nbtnet_console.exe btnet_service.exe.config");

                return;
            }

            // Get the configuration settings

            bool verbose = true;
            POP3Main pop3 = new POP3Main(args[0], verbose);
            pop3.start();

            Console.WriteLine("Hit enter to quit.");
            Console.Read();
            pop3.stop();

        }

    }
}