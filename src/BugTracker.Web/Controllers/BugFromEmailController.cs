using System;
using System.Data;
using System.IO;
using System.Web.Http;
using btnet.Mail;
using btnet.Models;
using btnet.Security;
using OpenPop.Mime;

namespace btnet.Controllers
{
    [Authorize]
    public class BugFromEmailController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Post([FromBody] BugFromEmail bugFromEmail)
        {
            if (bugFromEmail != null && ModelState.IsValid)
            {
                if (bugFromEmail.ShortDescription == null)
                {
                    bugFromEmail.ShortDescription = "";
                }
                else if (bugFromEmail.ShortDescription.Length > 200)
                {
                    bugFromEmail.ShortDescription = bugFromEmail.ShortDescription.Substring(0, 200);
                }

                Message mimeMessage = null;

                if (!string.IsNullOrEmpty(bugFromEmail.Message))
                {
                    mimeMessage = Mime.GetMimeMessage(bugFromEmail.Message);

                    bugFromEmail.Comment = Mime.get_comment(mimeMessage);

                    string headers = Mime.get_headers_for_comment(mimeMessage);
                    if (headers != "")
                    {
                        bugFromEmail.Comment = string.Format("{0}{1}{2}", headers, Environment.NewLine, bugFromEmail.Comment);
                    }

                    bugFromEmail.FromAddress = Mime.get_from_addr(mimeMessage);

                }
                else
                {
                    if (bugFromEmail.Comment == null)
                    {
                        bugFromEmail.Comment = string.Empty;
                    }
                }                                

                // Even though btnet_service.exe has already parsed out the bugid,
                // we can do a better job here with SharpMimeTools.dll
                string subject = "";

                if (mimeMessage != null)
                {
                    subject = Mime.get_subject(mimeMessage);

                    if (subject != "[No Subject]")
                    {
                        bugFromEmail.BugId = Mime.get_bugid_from_subject(ref subject);
                    }

                    bugFromEmail.CcAddress = Mime.get_cc(mimeMessage);
                }

                SQLString sql;

                if (bugFromEmail.BugId != 0)
                {
                    // Check if the bug is still in the database
                    // No comment can be added to merged or deleted bugids
                    // In this case a new bug is created, this to prevent possible loss of information

                    sql = new SQLString(@"select count(bg_id)
			from bugs
			where bg_id = @id");

                    sql = sql.AddParameterWithValue("id", Convert.ToString(bugFromEmail.BugId));

                    if (Convert.ToInt32(DbUtil.execute_scalar(sql)) == 0)
                    {
                        bugFromEmail.BugId = 0;
                    }
                }


                // Either insert a new bug or append a commment to existing bug
                // based on presence, absence of bugid
                if (bugFromEmail.BugId == 0)
                {
                    // insert a new bug

                    if (mimeMessage != null)
                    {

                        // in case somebody is replying to a bug that has been deleted or merged
                        subject = subject.Replace(Util.get_setting("TrackingIdString", "DO NOT EDIT THIS:"), "PREVIOUS:");

                        bugFromEmail.ShortDescription = subject;
                        if (bugFromEmail.ShortDescription.Length > 200)
                        {
                            bugFromEmail.ShortDescription = bugFromEmail.ShortDescription.Substring(0, 200);
                        }

                    }

                    DataRow defaults = Bug.get_bug_defaults();

                    // If you didn't set these from the query string, we'll give them default values
                    if (!bugFromEmail.ProjectId.HasValue || bugFromEmail.ProjectId == 0) { bugFromEmail.ProjectId = (int)defaults["pj"]; }
                    bugFromEmail.OrganizationId = bugFromEmail.OrganizationId ?? User.Identity.GetOrganizationId();
                    bugFromEmail.CategoryId = bugFromEmail.CategoryId ?? (int)defaults["ct"];
                    bugFromEmail.PriorityId = bugFromEmail.PriorityId ?? (int)defaults["pr"];
                    bugFromEmail.StatusId = bugFromEmail.StatusId ?? (int)defaults["st"];
                    bugFromEmail.UdfId = bugFromEmail.UdfId ?? (int)defaults["udf"];
                    
                    // but forced project always wins
                    if (User.Identity.GetForcedProjectId() != 0)
                    {
                        bugFromEmail.ProjectId = User.Identity.GetForcedProjectId();
                    }

                    Bug.NewIds newIds = Bug.insert_bug(
                        bugFromEmail.ShortDescription,
                        User.Identity,
                        "", // tags
                        bugFromEmail.ProjectId.Value,
                        bugFromEmail.OrganizationId.Value,
                        bugFromEmail.CategoryId.Value,
                        bugFromEmail.PriorityId.Value,
                        bugFromEmail.StatusId.Value,
                        bugFromEmail.AssignedTo ?? 0,
                        bugFromEmail.UdfId.Value,
                        bugFromEmail.Comment,
                        bugFromEmail.Comment,
                        bugFromEmail.FromAddress,
                        bugFromEmail.CcAddress,
                        "text/plain",
                        false, // internal only
                        null, // custom columns
                        false);  // suppress notifications for now - wait till after the attachments

                    if (mimeMessage != null)
                    {
                        Mime.add_attachments(mimeMessage, newIds.bugid, newIds.postid, User.Identity);

                        Email.auto_reply(newIds.bugid, bugFromEmail.FromAddress, bugFromEmail.ShortDescription, bugFromEmail.ProjectId.Value);

                    }
                    else if (bugFromEmail.Attachment != null && bugFromEmail.Attachment.Length > 0)
                    {
                        Stream stream = new MemoryStream(bugFromEmail.Attachment);

                        Bug.insert_post_attachment(
                            User.Identity,
                            newIds.bugid,
                            stream,
                            bugFromEmail.Attachment.Length,
                            bugFromEmail.AttachmentFileName ?? string.Empty,
                            bugFromEmail.AttachmentDescription ?? string.Empty,
                            bugFromEmail.AttachmentContentType ?? string.Empty,
                            -1, // parent
                            false, // internal_only
                            false); // don't send notification yet
                    }

                    // your customizations
                    Bug.apply_post_insert_rules(newIds.bugid);

                    Bug.send_notifications(Bug.INSERT, newIds.bugid, User.Identity);
                    WhatsNew.add_news(newIds.bugid, bugFromEmail.ShortDescription, "added", User.Identity);

                    return Ok(newIds.bugid);
                }
                else // update existing bug
                {

                    string statusResultingFromIncomingEmail = Util.get_setting("StatusResultingFromIncomingEmail", "0");


                    if (statusResultingFromIncomingEmail != "0")
                    {

                        sql = new SQLString(@"update bugs
				set bg_status = @st
				where bg_id = @bg
				");

                        sql = sql.AddParameterWithValue("st", statusResultingFromIncomingEmail);
                        sql = sql.AddParameterWithValue("bg", bugFromEmail.BugId);
                        DbUtil.execute_nonquery(sql);

                    }

                    sql = new SQLString("select bg_short_desc from bugs where bg_id = @bg");

                    sql = sql.AddParameterWithValue("bg", bugFromEmail.BugId);
                    DataRow dr2 = DbUtil.get_datarow(sql);


                    // Add a comment to existing bug.
                    int postid = Bug.insert_comment(
                        bugFromEmail.BugId,
                        User.Identity.GetUserId(), // (int) dr["us_id"],
                        bugFromEmail.Comment,
                        bugFromEmail.Comment,
                        bugFromEmail.FromAddress,
                        bugFromEmail.CcAddress,
                        "text/plain",
                        false); // internal only

                    if (mimeMessage != null)
                    {
                        Mime.add_attachments(mimeMessage, bugFromEmail.BugId, postid, User.Identity);
                    }
                    else if (bugFromEmail.Attachment != null && bugFromEmail.Attachment.Length > 0)
                    {
                        Stream stream = new MemoryStream(bugFromEmail.Attachment);
                        Bug.insert_post_attachment(
                            User.Identity,
                            bugFromEmail.BugId,
                            stream,
                            bugFromEmail.Attachment.Length,
                            bugFromEmail.AttachmentFileName ?? string.Empty,
                            bugFromEmail.AttachmentDescription ?? string.Empty,
                            bugFromEmail.AttachmentContentType ?? string.Empty,
                            -1, // parent
                            false, // internal_only
                            false); // don't send notification yet
                    }

                    Bug.send_notifications(Bug.UPDATE, bugFromEmail.BugId, User.Identity);
                    WhatsNew.add_news(bugFromEmail.BugId, (string)dr2["bg_short_desc"], "updated", User.Identity);

                    return Ok(bugFromEmail.BugId);
                }
            }
            else
            {
                return BadRequest(ModelState);
            }            
        }
    }
}
