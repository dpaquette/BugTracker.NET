/*
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
*/

using System;
using System.Web;
using System.Collections.Specialized;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace btnet
{

	public class VersionControl
	{

		static void configure_startinfo(System.Diagnostics.ProcessStartInfo startInfo)
		{
			startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
			startInfo.UseShellExecute = false;
			startInfo.RedirectStandardOutput = true;
			startInfo.RedirectStandardError = true;
			startInfo.StandardOutputEncoding = Encoding.UTF8;		}
	
		///////////////////////////////////////////////////////////////////////
		public static string run_and_capture(string filename, string args, string working_dir)
		{
			System.Diagnostics.Process p = new System.Diagnostics.Process();

			p.StartInfo.WorkingDirectory = working_dir;
            p.StartInfo.Arguments = args;
			p.StartInfo.FileName = filename;

			configure_startinfo(p.StartInfo);

			Util.write_to_log(filename + " " + args);

			p.Start();
			string stdout = p.StandardOutput.ReadToEnd();
			p.WaitForExit();
			stdout += p.StandardOutput.ReadToEnd();

			string error = p.StandardError.ReadToEnd();

			if (error != "")
			{
				Util.write_to_log("stderr:" + error);
                Util.write_to_log("stdout:" + stdout);
			}

            if (error != "" 
            && !error.Contains("RUNTIME_PREFIX")) // ignore the git "RUNTIME_PREFIX" error
			{
				string msg = "<div style='color:red; font-weight: bold; font-size: 10pt;'>";
				msg += "<br>Error executing git or hg command:";
				msg += "<br>Error: " + error;
				msg += "<br>Command: " + filename + " " + args;
                HttpContext.Current.Response.Write(msg);
                HttpContext.Current.Response.End();
                return msg;
			}
			else
			{
				btnet.Util.write_to_log("stdout:" + stdout);
				return stdout;
			}
		}


		///////////////////////////////////////////////////////////////////////
		public static string run_git(string args, string working_dir)
		{
			string filename = Util.get_setting("GitPathToGit","[path to git.exe?]");
			return run_and_capture(filename, args, working_dir);
		}

		///////////////////////////////////////////////////////////////////////
		public static string run_hg(string args, string working_dir)
		{
			string filename = Util.get_setting("MercurialPathToHg","[path to hg.exe?]");
			return run_and_capture(filename, args, working_dir);
		}


		///////////////////////////////////////////////////////////////////////
		public static string run_svn(string args_without_password, string repo)
		{
			// run "svn.exe" and capture its output

			System.Diagnostics.Process p = new System.Diagnostics.Process();
			string filename = Util.get_setting("SubversionPathToSvn", "svn");
            p.StartInfo.FileName = filename;

			configure_startinfo(p.StartInfo);

			args_without_password += " --non-interactive";

			string more_args = Util.get_setting("SubversionAdditionalArgs", "");			
			if (more_args != "")
			{
				args_without_password += " " + more_args;
			}
			
			Util.write_to_log(filename + " " + args_without_password);
			
			string args_with_password = args_without_password;

			// add a username and password to the args
			string username_and_password_and_websvn = Util.get_setting(repo, "");
			
			string[] parts = btnet.Util.rePipes.Split(username_and_password_and_websvn);
			if (parts.Length > 1)
			{
				if (parts[0] != "" && parts[1] != "")
				{
					args_with_password += " --username ";
					args_with_password += parts[0];
					args_with_password += " --password ";
					args_with_password += parts[1];
				}
			}
			
			p.StartInfo.Arguments = args_with_password;
			p.Start();
			string stdout = p.StandardOutput.ReadToEnd();
			p.WaitForExit();
			stdout += p.StandardOutput.ReadToEnd();

			string error = p.StandardError.ReadToEnd();

			if (error != "")
			{
				Util.write_to_log("stderr:" + error);
                Util.write_to_log("stdout:" + stdout);
			}

			if (error != "")
            {
                string msg = "<div style='color:red; font-weight: bold; font-size: 10pt;'>";
                msg += "<br>Error executing svn command:";
                msg += "<br>Error: " + error;
                msg += "<br>Command: " + filename + " " + args_without_password;
                if (error.Contains("File not found"))
                {
                    msg += "<br><br>***** Has this file been deleted or renamed? See the following links:";
                    msg += "<br><a href=http://svn.apache.org/repos/asf/subversion/trunk/doc/user/svn-best-practices.html>";
					msg += "http://svn.apache.org/repos/asf/subversion/trunk/doc/user/svn-best-practices.html</a>";
                    msg += "</div>";
                }
                HttpContext.Current.Response.Write(msg);
                HttpContext.Current.Response.End();
                return msg;
            }
			else
            {
                Util.write_to_log("stdout:" + stdout);
                return stdout;
            }
		}


        ///////////////////////////////////////////////////////////////////////
		enum State
        { 
            None,
            CountingMinuses,
            CountingPluses,
        }

        ///////////////////////////////////////////////////////////////////////
		static void which_chars_changed(string s1, string s2, ref string part1, ref string part2, ref string part3)
        {
            // Input is two strings
            // Output is the second string divided up into three parts based on which chars in string 2
            // are different than string 1.

            // Starting from the beginning of string 2, what is the first char that's different from string 1? That's one divide.
            // Starting from the end of string 2, going in reverse, what's the first char that's different from string 1?  That's another divider.

            int first_diff_char = -1;
            int last_diff_char = -1;

            if (s1.Length <= s2.Length)
            {
                int i = 0;
                while (first_diff_char == -1
                && i < s1.Length)
                {
                    if (s1[i] != s2[i])
                    {
                        first_diff_char = i;
                        break;
                    }
                    i++;
                }
                // if chars were appended to s2
                if (first_diff_char == -1)
                {
                    first_diff_char = i;
                }
            }
            else
            {
                int i = 0;
                while (first_diff_char == -1
                && i < s2.Length)
                {
                    if (s1[i] != s2[i])
                    {
                        first_diff_char = i;
                        break;
                    }
                    i++;
                }
                // if chars were deleted off the end of s2, we won't mark anything as orange
            }

            if (first_diff_char != -1)
            { 
                int index1 = s1.Length - 1;
                int index2 = s2.Length - 1;

                if (s1.Length <= s2.Length)
                {
                    while (last_diff_char == -1
                    && index1 > -1)
                    {
                        if (s1[index1] != s2[index2])
                        {
                            last_diff_char = index2;
                            break;
                        }
                        index1--;
                        index2--;
                    }

                    // if chars were added onto the front of s2
                    if (last_diff_char == -1)
                    {
                        last_diff_char = index2;
                    }
                }
                else
                {
                    while (last_diff_char == -1
                    && index2 > -1)
                    {
                        if (s1[index1] != s2[index2])
                        {
                            last_diff_char = index2;
                            break;
                        }
                        index1--;
                        index2--;
                    }
                    
                    // if chars were deleted off the front of s2, we won't show anything in orange
                }
            }
            
            if (first_diff_char == -1 || last_diff_char == -1)
            {
                part1 = s2;
            }
            else
            {

                if (first_diff_char > last_diff_char)
                {
                    // here's an example
                    // ab c
                    // ab b c
                    // From the left, the first diff char is the second "b", at index 3
                    // From the right, the first (meaning last) diff char is the space, at index 2
                    last_diff_char = s2.Length - 1;
                }
                
                part1 = s2.Substring(0, first_diff_char);
                part2 = s2.Substring(first_diff_char, (last_diff_char + 1) - first_diff_char);
                part3 = s2.Substring(last_diff_char + 1);
            }        
        }

        ///////////////////////////////////////////////////////////////////////
		static void mark_change_lines(int i, string[] lines, int PlusCount)
        {

            // Modify the UDF to make it richer and easier to process.
            // The heuristic has identified some of the -/+ lines as really meaning a change.
            // Marking the before lines with a "P" and the after lines with an "M".


            // mark the "before" lines
            int prev_line = i - 1;
            for (int j = 0; j < PlusCount; j++)
            {
                int m = prev_line - j;
                string sub = "";
                if (lines[m].Length > 0)
                    sub = lines[m].Substring(1);
                lines[m] = "P" + sub;
            }

            // mark the "after" lineas
            for (int j = PlusCount; j < 2 * PlusCount; j++)
            {
                int m = prev_line - j;
                string sub = "";
                if (lines[m].Length > 0)
                    sub = lines[m].Substring(1);
                lines[prev_line - j] = "M" + lines[prev_line - j].Substring(1);
            }
        
        }

        ///////////////////////////////////////////////////////////////////////
		static void maybe_mark_change_lines_in_unified_diff(string[] lines)
        {
            // If there are N minuses followed by exactly N pluses, we'll call that a change

            int MinusCount = 0;
            int PlusCount = 0;
            State state = State.None;


            int len = lines.Length;

            int i = 0;
            for (i = 0; i < len; i++)
            {
                if (state == State.None)
                {
                    if (lines[i].StartsWith("-"))
                    {
                        state = State.CountingMinuses;
                        MinusCount++;
                    }
                }
                else if (state == State.CountingMinuses)
                {
                    if (lines[i].StartsWith("-"))
                    {
                        MinusCount++;
                    }
                    else if (lines[i].StartsWith("+"))
                    {
                        state = State.CountingPluses;
                        PlusCount++;
                    }
                    else
                    {
                        state = State.None;
                        MinusCount = 0;
                        PlusCount = 0;
                    }
                }
                else if (state == State.CountingPluses)
                {
                    if (lines[i].StartsWith("+"))
                    {
                        PlusCount++;
                    }
                    else
                    {
                        if (PlusCount > 0 && PlusCount < 20 && PlusCount == MinusCount)
                        {
                            mark_change_lines(i, lines, PlusCount);
                        }

                        state = State.None;
                        PlusCount = 0;
                        MinusCount = 0;

                        if (lines[i].StartsWith("-"))
                        {
                            state = State.CountingMinuses;
                            MinusCount++;
                        }
                    }
                }
            } // end loop thru lines
            if (PlusCount > 0 && PlusCount < 20 && PlusCount == MinusCount)
            {
                mark_change_lines(i, lines, PlusCount);
            }
        }

		///////////////////////////////////////////////////////////////////////
		public static string visual_diff(string unified_diff_text, string left_in, string right_in, ref string left_out, ref string right_out)
		{
			Regex regex = new Regex("\n");
			string line = "";

			string diff_text = unified_diff_text;

			// get rid of lines we don't need
			int pos = unified_diff_text.IndexOf("\n@@");
			if (pos > -1)
			{
				diff_text = unified_diff_text.Substring(pos+1);
			}

			if (diff_text == "")
			{
				return "No differences.";
			}

			// first, split everything into lines
			string[] diff_lines = regex.Split(diff_text.Replace("\r\n","\n"));

            maybe_mark_change_lines_in_unified_diff(diff_lines);

			// html encode
			string left_text = HttpUtility.HtmlEncode(left_in);
			string right_text = HttpUtility.HtmlEncode(right_in);
			// split into lines
			string[] left_lines = regex.Split(left_text.Replace("\r\n","\n"));
			string[] right_lines = regex.Split(right_text.Replace("\r\n","\n"));


			// for formatting line numbers
			int max_lines = left_lines.Length;
			if (right_lines.Length > left_lines.Length)
				max_lines = right_lines.Length;

			// I just want to pad left a certain number of places
			// probably any 5th grader would know how to do this better than me
			string blank = "";
			int digit_places = Convert.ToString(max_lines).Length;

			int lx = 0;
			int rx = 0;
			int dx = 0;

			StringBuilder sL = new StringBuilder();
			StringBuilder sR = new StringBuilder();

            List<string> changed_lines_saved_for_later_compare = new List<string>();

			// L E F T
			// L E F T
			// L E F T

			//sL.Append("<div class=difffile>" + "left"  + "</div>");
			//sR.Append("<div class=difffile>" + "right" + "</div>");

			while (dx < diff_lines.Length)
			{
				line = diff_lines[dx];
				if (line.StartsWith("@@ -") && line.Contains(" @@"))
				{

					// See comment at the top of this file explaining Unified Diff Format
					// Parse out the left start line.  For example, the "38" here:
					// @@ -38,18 +39,12 @@
					// Note that the range could also be specified as follows, with the number of lines assumed to be 1
					// @@ -1 +1,2 @@


					int pos1 = line.IndexOf("-");
					int comma_pos = line.IndexOf(",", pos1);
					if (comma_pos == -1)
					{
						comma_pos = 9999;
					}
					int pos2 = Math.Min(line.IndexOf(" ", pos1), comma_pos);
					string left_start_line_string = line.Substring(pos1 + 1, pos2 - (pos1 + 1));
					int start_line = Convert.ToInt32(left_start_line_string);
					start_line -= 1; // adjust for zero based index


					// advance through left file until we hit the starting line of the range
					while (lx < start_line)
					{
						sL.Append("<span class=diffnum>");
						sL.Append(Convert.ToString(lx+1).PadLeft(digit_places,'0'));
						sL.Append(" </span>");
						sL.Append(left_lines[lx++]);
						sL.Append("\n");
					}


					// we are positioned in the left file at the start of the diff blockk
					dx++;
					line = diff_lines[dx];
					while (dx < diff_lines.Length
					&& !(line.StartsWith("@@ -") && line.EndsWith(" @@")))
					{
						if (line.StartsWith("+"))
						{
							sL.Append("<span class=diffnum>");
							sL.Append(blank.PadLeft(digit_places,' '));
							sL.Append(" </span>");
							sL.Append("<span class=diffblank>&nbsp;&nbsp;&nbsp;&nbsp;</span>\n");
						}
						else if (line.StartsWith("-"))
						{

							sL.Append("<span class=diffnum>");
							sL.Append(Convert.ToString(lx+1).PadLeft(digit_places,'0'));
							sL.Append(" </span>");

							sL.Append("<span class=diffdel>");

                            sL.Append(left_lines[lx++]);
							sL.Append("</span>\n");
						}
                        else if (line.StartsWith("M"))
                        {

                            sL.Append("<span class=diffnum>");
                            sL.Append(Convert.ToString(lx + 1).PadLeft(digit_places, '0'));
                            sL.Append(" </span>");

                            sL.Append("<span class=diffchg>");

                            // Save the left lines for later, so that we can mark the changed chars in orange
                            changed_lines_saved_for_later_compare.Add(left_lines[lx]);

                            sL.Append(left_lines[lx++]);
                            sL.Append("</span>\n");
                        }
                        else if (line.StartsWith("\\") || line == "" || line.StartsWith("P"))
						{
						}
						else
						{
							sL.Append("<span class=diffnum>");
							sL.Append(Convert.ToString(lx+1).PadLeft(digit_places,'0'));
							sL.Append(" </span>");
							sL.Append(left_lines[lx++]);
							sL.Append("\n");
						}

						dx++;

						if (dx < diff_lines.Length)
						{
							line = diff_lines[dx];
						}
					} // end of range block
				}

				if (dx < diff_lines.Length && line.StartsWith("@@ -") && line.Contains(" @@"))
				{
					continue;
				}
				else
				{
					break;
				}

			} // end of all blocks

			// advance through left file until we hit the starting line of the range

			while (lx < left_lines.Length)
			{
				sL.Append("<span class=diffnum>");
				sL.Append(Convert.ToString(lx+1).PadLeft(digit_places,'0'));
				sL.Append(" </span>");
				sL.Append(left_lines[lx++]);
				sL.Append("\n");
			}



			// R I G H T
			// R I G H T
			// R I G H T
			dx = 0;
            int index_of_changed_lines = 0;

			while (dx < diff_lines.Length)
			{
				line = diff_lines[dx];
				if (line.StartsWith("@@ -") && line.Contains(" @@"))
				{

					// See comment at the top of this file explaining Unified Diff Format

					// parse out the right start line.  For example, the "39" here: @@ -38,18 +39,12 @@
					int pos1 = line.IndexOf("+");

					int pos2 = line.IndexOf(",", pos1);
					if (pos2 == -1) pos2 = line.IndexOf(" ", pos1);

					string right_start_line_string = line.Substring(pos1 + 1, pos2 - (pos1 + 1));
					int start_line = Convert.ToInt32(right_start_line_string);
					start_line -= 1; // adjust for zero based index

					// advance through right file until we hit the starting line of the range
					while (rx < start_line)
					{
						sR.Append("<span class=diffnum>");
						sR.Append(Convert.ToString(rx+1).PadLeft(digit_places,'0'));
						sR.Append(" </span>");
						sR.Append(right_lines[rx++]);
						sR.Append("\n");
					}


					// we are positioned in the right file at the start of the diff block
					dx++;
					line = diff_lines[dx];

                    while (dx < diff_lines.Length && !(line.StartsWith("@@ -") && line.Contains(" @@")))
					{
						if (line.StartsWith("-"))
						{
							sR.Append("<span class=diffnum>");
							sR.Append(blank.PadLeft(digit_places,' '));
							sR.Append(" </span>");
							sR.Append("<span class=diffblank>&nbsp;&nbsp;&nbsp;&nbsp;</span>\n");
						}
						else if (line.StartsWith("+"))
						{
							sR.Append("<span class=diffnum>");
							sR.Append(Convert.ToString(rx+1).PadLeft(digit_places,'0'));
							sR.Append(" </span>");
							sR.Append("<span class=diffadd>");
							sR.Append(right_lines[rx++]);
							sR.Append("</span>\n");
						}
                        else if (line.StartsWith("P"))
                        {
                            sR.Append("<span class=diffnum>");
                            sR.Append(Convert.ToString(rx + 1).PadLeft(digit_places, '0'));
                            sR.Append(" </span>");
                            
                            string part1 = "";
                            string part2 = "";
                            string part3 = "";
                            
							which_chars_changed(changed_lines_saved_for_later_compare[
                                index_of_changed_lines], 
                                right_lines[rx++],
                                ref part1, ref part2, ref part3);

                            sR.Append("<span class=diffchg>");
                            sR.Append(part1);
                            sR.Append("</span>");

                            sR.Append("<span class=diffchg2>");
                            sR.Append(part2);
                            sR.Append("</span>");

                            sR.Append("<span class=diffchg>");
                            sR.Append(part3);
                            sR.Append("</span>");

                            index_of_changed_lines++;
                            sR.Append("</span>\n");
                        }
                        else if (line.StartsWith("\\") || line == "" || line.StartsWith("M"))
						{
						}
						else
						{
							sR.Append("<span class=diffnum>");
							sR.Append(Convert.ToString(rx+1).PadLeft(digit_places,'0'));
							sR.Append(" </span>");
							sR.Append(right_lines[rx++]);
							sR.Append("\n");
						}

						dx++;

						if (dx < diff_lines.Length)
						{
							line = diff_lines[dx];
						}

					} // end of range block
				}

				if (dx < diff_lines.Length && line.StartsWith("@@ -") && line.EndsWith(" @@"))
				{
					continue;
				}
				else
				{
					break;
				}

			} // end of all blocks

			// advance through right file until we're done

			while (rx < right_lines.Length)
			{
				sR.Append("<span class=diffnum>");
				sR.Append(Convert.ToString(rx+1).PadLeft(digit_places,'0'));
				sR.Append(" </span>");
				sR.Append(right_lines[rx++]);
				sR.Append("\n");
			}

			left_out = sL.ToString();
			right_out = sR.ToString();

			return "";
		}

/*

hg

*/

		
		///////////////////////////////////////////////////////////////////////
		public static string hg_log(string repo, string revision, string file_path)
		{
		    
		    string args = "log  --style btnet -r :";
		    args += revision;
		    args += " \"";
		    args += file_path;
		    args += "\"";
		        
			string result = VersionControl.run_hg(args, repo);
			return result;
		}
		
		///////////////////////////////////////////////////////////////////////
		public static string hg_blame(string repo, string file_path, string revision)
		{
			string args = "blame -u -d -c -l -v -r ";
			args += revision;
			args += " \"";
			args += file_path;
			args += "\"";

			string result = VersionControl.run_hg(args, repo);
			return result;
		}
		
		///////////////////////////////////////////////////////////////////////
		public static string hg_get_file_contents(string repo, string revision, string file_path)
		{
		
		    string args = "cat -r ";
		    args += revision;
		    args += " \"";
		    args += file_path;
		    args += "\"";
		
		    string result = VersionControl.run_hg(args, repo);
		    return result;
		}
		
		///////////////////////////////////////////////////////////////////////
		public static string hg_get_unified_diff_two_revisions(string repo, string revision0, string revision1, string file_path)
		{

			string	args = "diff -r ";
			args += revision0;
			args += " -r ";
			args += revision1;

			args += " \"";
			args += file_path;
			args += "\"";

			string result = VersionControl.run_hg(args, repo);
			return result;
		}
		
/*

git

*/
		
		///////////////////////////////////////////////////////////////////////
		public static string git_log(string repo, string commit, string file_path)
		{

			string args = "log --name-status --date=iso ";
			args += commit;
			args += " -- \"";
			args += file_path;
			args += "\"";

			string result = VersionControl.run_git(args, repo);
			return result;
		}		
		///////////////////////////////////////////////////////////////////////
		public static string git_blame(string repo, string file_path, string commit)
		{
		    
		    string args = "blame ";
		    args += " -- \"";
		    args += file_path;
		    args += "\" ";
		    args += commit;
		        
			string result = VersionControl.run_git(args, repo);
			return result;
		}

		///////////////////////////////////////////////////////////////////////
		public static string git_get_file_contents(string repo, string commit, string file_path)
		{

			string args = "show --pretty=raw ";
			args += commit;
			args += ":\"";
			args += file_path;
			args += "\"";

			string result = VersionControl.run_git(args, repo);
			return result;
		}

		
		///////////////////////////////////////////////////////////////////////
		public static string git_get_unified_diff_two_commits(string repo, string commit0, string commit1, string file_path)
		{

			string	args = "diff ";
			args += commit0;
			args += " ";
			args += commit1;

			args += " -- \"";
			args += file_path;
			args += "\"";

			string result = VersionControl.run_git(args, repo);
			return result;
		}


		///////////////////////////////////////////////////////////////////////
		public static string git_get_unified_diff_one_commit(string repo, string commit, string file_path)
		{

			string args = "show  --pretty=format: ";
			args += commit;

			args += " -- \"";
			args += file_path;
			args += "\"";

			string result = VersionControl.run_git(args, repo);
			return result;
		}

/*

svn

*/

		///////////////////////////////////////////////////////////////////////
		public static string svn_log(string repo, string file_path, int rev)
		{
			StringBuilder args = new StringBuilder();

			args.Append("log ");
			args.Append(repo);
			args.Append(file_path.Replace(" ", "%20"));
			args.Append("@" + Convert.ToString(rev)); // peg to revision rev in case file deleted
			args.Append(" -r ");
			args.Append(Convert.ToString(rev)); // view log from beginning to rev
			args.Append(":0 --xml -v");

			return run_svn(args.ToString(), repo);
		}


		///////////////////////////////////////////////////////////////////////
		public static string svn_blame(string repo, string file_path, int rev)
		{
			StringBuilder args = new StringBuilder();

			args.Append("blame ");
			args.Append(repo);
			args.Append(file_path.Replace(" ", "%20"));
			args.Append("@");
			args.Append(Convert.ToString(rev));
			args.Append(" --xml");

			return run_svn(args.ToString(), repo);
		}


		///////////////////////////////////////////////////////////////////////
		public static string svn_cat(string repo, string file_path, int rev)
		{
			StringBuilder args = new StringBuilder();

			args.Append("cat ");
			args.Append(repo);
			args.Append(file_path.Replace(" ", "%20"));
			args.Append("@");
			args.Append(Convert.ToInt32(rev));

			return run_svn(args.ToString(), repo);
		}

		///////////////////////////////////////////////////////////////////////
		public static string svn_diff(string repo, string file_path, int revision, int old_revision)
		{
			StringBuilder args = new StringBuilder();

			if (old_revision != 0)
			{
				args.Append("diff -r ");

				args.Append(Convert.ToString(old_revision));
				args.Append(":");
				args.Append(Convert.ToString(revision));
				args.Append(" ");
				args.Append(repo);
				args.Append(file_path.Replace(" ", "%20"));
			}
			else
			{
				args.Append("diff -c ");
				args.Append(Convert.ToString(revision));
				args.Append(" ");
				args.Append(repo);
				args.Append(file_path.Replace(" ","%20"));
			}

			string result = run_svn(args.ToString(), repo);
			return result;
		}

    } 
}
