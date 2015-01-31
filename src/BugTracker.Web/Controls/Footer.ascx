<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Footer.ascx.cs" Inherits="btnet.Controls.Footer" %>
<footer class="page-footer">
    <div class="container">
        <ul class="text-muted">
            <li>BugTracker.NET</li>
            <li>·</li>
            <li><a target="_blank" href="http://ifdefined.com/README.html">Help</a></li>
            <li>·</li>
            <li><a target="_blank" href="mailto:ctrager@yahoo.com">Feedback</a></li>
            <li>·</li>
            <li><a target="_blank" href="about.html">About</a></li>
            <li>·</li>
            <li><a target="_blank" href="http://ifdefined.com/README.html">Donate</a></li>
        </ul>
    </div>
    <% Response.Write(Application["custom_footer"]); %>
</footer>
