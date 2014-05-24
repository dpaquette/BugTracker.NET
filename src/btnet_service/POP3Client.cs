/*

[Corey Trager] I downloaded this code from the URL below September 14, 2003:

http://www.codeproject.com/csharp/pop3client.asp

On that page Bill Dean writes:

"I hope some of you find this useful.
I'd love some feedback / comments.
Please be aware that this code come with no warranty of any sort, express or implied.
It is provided strictly "as is" and is indended solely for educational purposes.
YOU USE IT AT YOUR OWN RISK.
By using this code you agree to hold the Author and Restek blameless for any loss
resulting from the use of the code."


Here's a usage example from that page.
static void Main(string[] args)
{

    POP3Client.POP3client  Demo = new POP3Client.POP3client();
    Console.WriteLine ("****connecting to server:");
    Console.WriteLine (Demo.connect ("your_pop3_server"));
    Console.WriteLine ("****Issuing USER");
    Console.WriteLine (Demo.USER ("user_id"));
    Console.WriteLine ("****Issuing PASS");
    Console.WriteLine (Demo.PASS ("password"));
    Console.WriteLine ("****Issuing STAT");
    Console.WriteLine (Demo.STAT () );
    Console.WriteLine ("****Issuing LIST");
    Console.WriteLine (Demo.LIST () );
    Console.WriteLine ("****Issuing RETR 700...this will cause the POP3 server to gack a "
                            + "hairball since there is no message 700");
    Console.WriteLine (Demo.RETR (700) );    // this will cause the pop3 server to throw
                                                       // an error since there is no message 700
    Console.WriteLine ("****Issuing RETR 7");
    Console.WriteLine (Demo.RETR (7) );
    Console.WriteLine ("****Issuing QUIT");
    Console.WriteLine (Demo.QUIT ());

    Console.ReadLine ();
}



*/
using System;

namespace POP3Client
{
	using System.IO ;
	using System.Net;
	using System.Net.Sockets ;
	using System.Net.Security;
	//Please note that all code is copyright 2002 by William J Dean


	public class POP3client
	{
		public enum connect_state {disc,AUTHORIZATION,TRANSACTION,UPDATE};

		public string user;
		public string pwd;
		public string pop;
		public int popPort = 110;
		public bool popSSL = false;
		public bool error;
		public connect_state state=connect_state.disc ;
		public bool bReadInputStreamCharByChar = false;

		//borrowed from Agus Kurniawan's article:"Retrieve Mail From a POP3 Server Using C#"  at http://www.codeproject.com/csharp/popapp.asp
		private TcpClient Server;
		private Stream NetStrm;
		private StreamReader  RdStrm;
		private string Data;
		private byte[] szData;
		private string CRLF = "\r\n";

		public POP3client()
		{
			//nothing to do..just create to object
		}

		// This constructor added by Corey Trager Mar 2007
		public POP3client(int ReadInputStreamCharByChar)
		{
			bReadInputStreamCharByChar = ReadInputStreamCharByChar == 1 ? true : false;
		}

		public POP3client(string pop_server,int pop_port,bool pop_ssl,string user_name,string password)
		{
			//put the specied server (pop_server), port (pop_port), if the server is an ssl server (pop_ssl),
			//user (user_name) and password (password) into the appropriate properties.
			pop=pop_server;
			popPort=pop_port;
			popSSL=pop_ssl;
			user=user_name;
			pwd=password;
		}

		#region Utility Methods, some public, some private
		public string connect(string pop_server, int pop_port, bool pop_ssl)
		{
			pop=pop_server;    //put the specified server into the pop property
			popPort=pop_port;    //put the specified port into the popPort property
			popSSL=pop_ssl;    //put the ssl into the popSSL property
			return(connect()); //call the connect method
		}
		public string connect()
		{
			//Initialize to the pop server.  This code snipped "borrowed"
			//with some modifications...
			//from the article "Retrieve Mail From a POP3 Server Using C#" at
			//www.codeproject.com by Agus Kurniawan
			//http://www.codeproject.com/csharp/popapp.asp

			// create server with port 110

			try
			{
				Server = new TcpClient(pop,popPort);
				// initialization
				if (!popSSL)
				{
				// initialization
				NetStrm = Server.GetStream();

				} 
				else
				{
					NetStrm = new SslStream(Server.GetStream());
					((SslStream)NetStrm).AuthenticateAsClient(pop);
				}
				RdStrm = new StreamReader(NetStrm);

				//The pop session is now in the AUTHORIZATION state
				state=connect_state.AUTHORIZATION ;
				return(RdStrm.ReadLine ());
			}
			catch(Exception err)
			{
				return("Error: "+err.ToString());
			}

		}
		private string disconnect ()
		{
			string temp="disconnected successfully.";
			if(state !=connect_state.disc)
			{

				//close connection
				NetStrm.Close();
				RdStrm.Close();
				state=connect_state.disc ;
			}
			else
			{
				temp="Not Connected.";
			}
			return(temp);
		}

		private void issue_command(string command)
		{
			//send the command to the pop server.  This code snipped "borrowed"
			//with some modifications...
			//from the article "Retrieve Mail From a POP3 Server Using C#" at
			//www.codeproject.com by Agus Kurniawan
			//http://www.codeproject.com/csharp/popapp.asp
			Data= command + CRLF;
			szData = System.Text.Encoding.ASCII.GetBytes(Data.ToCharArray());
			NetStrm.Write(szData,0,szData.Length);

		}
		private string read_single_line_response()
		{
			//read the response of the pop server.  This code snipped "borrowed"
			//with some modifications...
			//from the article "Retrieve Mail From a POP3 Server Using C#" at
			//www.codeproject.com by Agus Kurniawan
			//http://www.codeproject.com/csharp/popapp.asp
			string temp;
			try
			{
				temp = RdStrm.ReadLine();
				was_pop_error(temp);
				return(temp);
			}
			catch(Exception err)
			{
				return("Error in read_single_line_response(): " + err.ToString ()) ;
			}

		}

		// This was the original, but it didn't handle some UTF8 characters - Corey Trager, March 03, 2007
		private string read_multi_line_response()
		{
			//read the response of the pop server.  This code snipped "borrowed"
			//with some modifications...
			//from the article "Retrieve Mail From a POP3 Server Using C#" at
			//www.codeproject.com by Agus Kurniawan
			//http://www.codeproject.com/csharp/popapp.asp
			System.Text.StringBuilder temp=new System.Text.StringBuilder(5000);
			string szTemp;

			try
			{
				szTemp = RdStrm.ReadLine();
				was_pop_error(szTemp);
				if(!error)
				{

					while(szTemp!=".")
					{
						temp.Append(szTemp+CRLF);
						szTemp = RdStrm.ReadLine();
					}
				}
				else
				{
					return szTemp;
				}
				return(temp.ToString());
			}
			catch(Exception err)
			{
				return("Error in read_multi_line_response(): " + err.ToString ());
			}
		}

		// written by Corey Trager, March 03, 2007
		private string NEW_read_multi_line_response()
		{
			System.Text.StringBuilder temp=new System.Text.StringBuilder(4096);

			try
			{

				byte[] b = new byte[4096];
				int bytes_read = 0;

				bytes_read = Server.GetStream().Read(b,0,b.Length);

				while (bytes_read > 0)
				{
					for (int i = 0; i < bytes_read; i++)
					{
						temp.Append(Convert.ToChar(b[i])); // Does work
					}


					if (temp.Length > 4
					&& temp[temp.Length-1] == 0x0A
					&& temp[temp.Length-2] == 0x0D
					&& temp[temp.Length-3] == '.'
					&& temp[temp.Length-4] == 0x0A
					&& temp[temp.Length-5] == 0x0D)
					{
						temp[temp.Length-3] = '\0';
						bytes_read = 0;
					}
					else
					{
						bytes_read = Server.GetStream().Read(b,0,b.Length);
					}

				}

				return temp.ToString();

			}
			catch(Exception err)
			{
				return("Error in read_multi_line_response(): " + err.ToString ());
			}
		}
		private void was_pop_error(string response)
		{
			//detect if the pop server that issued the response believes that
			//an error has occured.

			if(response.StartsWith ("-"))
			{
				//if the first character of the response is "-" then the
				//pop server has encountered an error executing the last
				//command send by the client
				error=true;
			}
			else
			{
				//success
				error=false;
			}
		}
		#endregion
		#region POP commands
		public string DELE(int msg_number)
		{
			string temp;

			if (state != connect_state.TRANSACTION )
			{
				//DELE is only valid when the pop session is in the TRANSACTION STATE
				temp="Connection state not = TRANSACTION";
			}
			else
			{
				issue_command("DELE " + msg_number.ToString ());
				temp=read_single_line_response();
			}
			return(temp);
		}

		public string LIST()
		{
			string temp="";
			if (state != connect_state.TRANSACTION )
			{
				//the pop command LIST is only valid in the TRANSACTION state
				temp="Connection state not = TRANSACTION";
			}
			else
			{
				issue_command ("LIST");
				temp=read_multi_line_response();
			}
			return(temp);
		}

		public string LIST(int msg_number)
		{
			string temp="";

			if (state != connect_state.TRANSACTION )
			{
				//the pop command LIST is only valid in the TRANSACTION state
				temp="Connection state not = TRANSACTION";
			}
			else
			{
				issue_command ("LIST " + msg_number.ToString ());
				temp=read_single_line_response();  //when the message number is supplied, expect a single line response
			}
			return(temp);

		}

		public string NOOP()
		{
			string temp;
			if (state != connect_state.TRANSACTION )
			{
				//the pop command NOOP is only valid in the TRANSACTION state
				temp="Connection state not = TRANSACTION";
			}
			else
			{
				issue_command ("NOOP");
				temp=read_single_line_response();

			}
			return(temp);

		}
		public string PASS()
		{
			string temp;
			if (state != connect_state.AUTHORIZATION)
			{
				//the pop command PASS is only valid in the AUTHORIZATION state
				temp="Connection state not = AUTHORIZATION";
			}
			else
			{
				if (pwd !=null)
				{
					issue_command ("PASS " + pwd);
					temp=read_single_line_response();

					if (!error)
					{
						//transition to the Transaction state
						state=connect_state.TRANSACTION;
					}
				}
				else
				{
					temp="No Password set.";
				}
			}
			return(temp);
		}
		public string PASS(string password)
		{
			pwd=password;   //put the supplied password into the appropriate property
			return(PASS()); //call PASS() with no arguement
		}

		public string QUIT()
		{
			//QUIT is valid in all pop states

			string temp;
			if (state !=connect_state.disc)
			{
				issue_command ("QUIT");
				temp=read_single_line_response();
				temp += CRLF + disconnect();

			}
			else
			{
				temp="Not Connected.";
			}
			return(temp);

		}
		public string RETR (int msg)
		{
			string temp="";
			if (state != connect_state.TRANSACTION )
			{
				//the pop command RETR is only valid in the TRANSACTION state
				temp="Connection state not = TRANSACTION";
			}
			else
			{
				// retrieve mail with number mail parameter
				issue_command ("RETR "+ msg.ToString ());
				if (bReadInputStreamCharByChar)
				{
					temp=NEW_read_multi_line_response();
				}
				else
				{
					temp=read_multi_line_response();
				}

			}
			return(temp);

		}

		public string RSET()
		{
			string temp;
			if (state != connect_state.TRANSACTION )
			{
				//the pop command STAT is only valid in the TRANSACTION state
				temp="Connection state not = TRANSACTION";
			}
			else
			{
				issue_command("RSET");
				temp=read_single_line_response();
			}
			return(temp);

		}

		public string STAT()
		{
			string temp;
			if (state==connect_state.TRANSACTION)
			{
				issue_command("STAT");
				temp=read_single_line_response();

				return(temp);
			}
			else

			{
				//the pop command STAT is only valid in the TRANSACTION state
				return ("Connection state not = TRANSACTION");
			}
		}

		public string USER()
		{
			string temp;
			if (state != connect_state.AUTHORIZATION)
			{
				//the pop command USER is only valid in the AUTHORIZATION state
				temp="Connection state not = AUTHORIZATION";
			}
			else
			{
				if (user !=null)
				{
					issue_command("USER "+ user);
					temp=read_single_line_response();
				}
				else
				{   //no user has been specified
					temp="No User specified.";
				}
			}
			return(temp);
		}

		public string USER(string user_name)
		{
			user=user_name;  //put the user name in the appropriate propertity
			return(USER());  //call USER with no arguements
		}
		#endregion
	}

}
