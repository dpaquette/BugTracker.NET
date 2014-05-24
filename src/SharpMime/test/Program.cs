using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using anmar.SharpMimeTools;


namespace SharpMimeToolsTester
{
    class Program
    {
        static void Main(string[] args)
        {
            test1();
            test2();
            test2b();
            test5();
        }

        static void test1() {
            String s = @"Received: from EMEAMAICLI-ED33.main.glb.corp.local ([169.254.3.202]) by
 EMEAMAICLI-EH01.main.glb.corp.local ([10.30.30.30]) with mapi; Thu, 4 Oct
 2012 12:53:30 +0200
From: Corey Trager <corey.trager@example.com>
To: HOLDING df-cgf-systemes-test <holding.df-cgf-systemes-test@example.com>
Date: Thu, 4 Oct 2012 12:53:29 +0200
Subject: This is the mail title for Test 1
Thread-Topic: This is the mail title for Test 1
Thread-Index: Ac2iG4qeNG4GodS/RhCEZtD6FTjmFAAAT0XQAABcA7A=
Message-ID:
 <1372B39E66C04B4E9D95CF73184032B006FD4ABDA0@EMEAMAICLI-ED33.main.glb.corp.local>
Accept-Language: fr-FR, en-US
Content-Language: fr-FR
X-MS-Exchange-Organization-AuthAs: Internal
X-MS-Exchange-Organization-AuthMechanism: 04
X-MS-Exchange-Organization-AuthSource: EMEAMAICLI-EH01.main.glb.corp.local
X-MS-Has-Attach:
X-MS-Exchange-Organization-SCL: -1
X-MS-TNEF-Correlator:
acceptlanguage: fr-FR, en-US
Content-Type: text/plain; charset=""iso-8859-1""
Content-Transfer-Encoding: quoted-printable
MIME-Version: 1.0

Hello Test 1,

Description of the test :
There is no attachment, no enclosed image, just plain text

Regards,
Rapha=EBl
";
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            System.IO.MemoryStream stream = new System.IO.MemoryStream(bytes);
            SharpMimeMessage mime_message = new SharpMimeMessage(stream);
            System.Diagnostics.Debug.Print(mime_message.BodyDecoded);

        
        }

        static void test2()
        {
            String s = @"Received: from EMEAMAICLI-ED33.main.glb.corp.local ([169.254.3.202]) by
 EMEAMAICLI-EH02.main.glb.corp.local ([10.30.30.31]) with mapi; Thu, 4 Oct
 2012 15:14:34 +0200
From: Corey Trager <corey.trager@example.com>
To: HOLDING df-cgf-systemes-test <holding.df-cgf-systemes-test@example.com>
Date: Thu, 4 Oct 2012 15:14:32 +0200
Subject: This is the mail title for Test 2
Thread-Topic: This is the mail title for Test 2
Thread-Index: Ac2iG4qeNG4GodS/RhCEZtD6FTjmFAAAT0XQAABcA7AAABrJgA==
Message-ID:
 <1372B39E66C04B4E9D95CF73184032B006FD4ABDF2@EMEAMAICLI-ED33.main.glb.corp.local>
Accept-Language: fr-FR, en-US
Content-Language: fr-FR
X-MS-Exchange-Organization-AuthAs: Internal
X-MS-Exchange-Organization-AuthMechanism: 04
X-MS-Exchange-Organization-AuthSource: EMEAMAICLI-EH02.main.glb.corp.local
X-MS-Has-Attach: yes
X-MS-Exchange-Organization-SCL: -1
X-MS-TNEF-Correlator:
acceptlanguage: fr-FR, en-US
Content-Type: multipart/related;
	boundary=""_002_1372B39E66C04B4E9D95CF73184032B006FD4ABDF2EMEAMAICLIED3_"";
	type=""text/plain""
MIME-Version: 1.0

--_002_1372B39E66C04B4E9D95CF73184032B006FD4ABDF2EMEAMAICLIED3_
Content-Type: text/plain; charset=""iso-8859-1""
Content-Transfer-Encoding: quoted-printable

Hello Test 2,

Description of the test :
There is no attachment but there is one image enclosed within the mail text=
, here it is :

[cid:image001.png@01CDA238.DCAFF600]
Regards,
Rapha=EBl



--_002_1372B39E66C04B4E9D95CF73184032B006FD4ABDF2EMEAMAICLIED3_
Content-Type: image/png; name=""image001.png""
Content-Description: image001.png
Content-Disposition: inline; filename=""image001.png""; size=11296;
	creation-date=""Thu, 04 Oct 2012 13:14:33 GMT"";
	modification-date=""Thu, 04 Oct 2012 13:14:33 GMT""
Content-ID: <image001.png@01CDA238.DCAFF600>
Content-Transfer-Encoding: base64

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
gg==

--_002_1372B39E66C04B4E9D95CF73184032B006FD4ABDF2EMEAMAICLIED3_--";

            byte[] bytes = Encoding.UTF8.GetBytes(s);
            System.IO.MemoryStream stream = new System.IO.MemoryStream(bytes);
            SharpMimeMessage mime_message = new SharpMimeMessage(stream);
            String comment = get_comment(mime_message);
            System.Diagnostics.Debug.Print(comment);

        }

        static void test2b() {

            String s = @"To: ""aaa""
Subject: bbb
Date: Wed, 23 Jan 2013 20:46:58 -0600
MIME-Version: 1.0
Content-Type: multipart/alternative;
	boundary=""----=_NextPart_000_0006_01CDF9AA.CE5B7830""
X-Priority: 3
X-MSMail-Priority: Normal
Importance: Normal
X-Unsent: 1
X-MimeOLE: Produced By Microsoft MimeOLE V15.4.3538.513

This is a multi-part message in MIME format.

------=_NextPart_000_0006_01CDF9AA.CE5B7830
Content-Type: text/plain;
	charset=""iso-8859-1""
Content-Transfer-Encoding: quoted-printable

qqqqq ccccc
qqqqq ccccc
qqqqq ccccc
------=_NextPart_000_0006_01CDF9AA.CE5B7830
Content-Type: text/html;
	charset=""iso-8859-1""
Content-Transfer-Encoding: quoted-printable

<HTML><HEAD></HEAD>
<BODY dir=3Dltr>
<DIV dir=3Dltr>
<DIV style=3D""FONT-FAMILY: 'Calibri'; COLOR: #000000; FONT-SIZE: 12pt"">
<DIV>qqqqqq ccccc</DIV></DIV></DIV></BODY></HTML>

------=_NextPart_000_0006_01CDF9AA.CE5B7830--";

            byte[] bytes = Encoding.UTF8.GetBytes(s);
            System.IO.MemoryStream stream = new System.IO.MemoryStream(bytes);
            SharpMimeMessage mime_message = new SharpMimeMessage(stream);
            String comment = get_comment(mime_message);
            System.Diagnostics.Debug.Print(comment);

        }

        static void test5() {
            String s = @"Received: from EMEAMAICLI-ED33.main.glb.corp.local ([169.254.3.202]) by
 EMEAMAICLI-EH01.main.glb.corp.local ([10.30.30.30]) with mapi; Thu, 4 Oct
 2012 15:26:16 +0200
From: Corey Trager <corey.trager@example.com>
To: HOLDING df-cgf-systemes-test <holding.df-cgf-systemes-test@example.com>
Date: Thu, 4 Oct 2012 15:26:15 +0200
Subject: This is the mail title for Test 5
Thread-Topic: This is the mail title for Test 5
Thread-Index:
 Ac2iG4qeNG4GodS/RhCEZtD6FTjmFAAAT0XQAABcA7AAABrJgAAE/dogAAAv3cA=
Message-ID:
 <1372B39E66C04B4E9D95CF73184032B006FD4ABE03@EMEAMAICLI-ED33.main.glb.corp.local>
Accept-Language: fr-FR, en-US
Content-Language: fr-FR
X-MS-Exchange-Organization-AuthAs: Internal
X-MS-Exchange-Organization-AuthMechanism: 04
X-MS-Exchange-Organization-AuthSource: EMEAMAICLI-EH01.main.glb.corp.local
X-MS-Has-Attach: yes
X-MS-Exchange-Organization-SCL: -1
X-MS-TNEF-Correlator:
acceptlanguage: fr-FR, en-US
Content-Type: multipart/mixed;
	boundary=""_002_1372B39E66C04B4E9D95CF73184032B006FD4ABE03EMEAMAICLIED3_""
MIME-Version: 1.0

--_002_1372B39E66C04B4E9D95CF73184032B006FD4ABE03EMEAMAICLIED3_
Content-Type: text/plain; charset=""iso-8859-1""
Content-Transfer-Encoding: quoted-printable

Hello Test 5,

Description of the test :
There is no image enclosed but there is one mail (MSG format) attached to t=
he mail.

Regards,
Rapha=EBl


--_002_1372B39E66C04B4E9D95CF73184032B006FD4ABE03EMEAMAICLIED3_
Content-Type: message/rfc822

From: Corey Trager <corey.trager@example.com>
To: HOLDING df-cgf-systemes-test <holding.df-cgf-systemes-test@example.com>
Date: Thu, 4 Oct 2012 15:25:22 +0200
Subject: This is a dummy mail title
Thread-Topic: This is a dummy mail title
Thread-Index:
 Ac2iG4qeNG4GodS/RhCEZtD6FTjmFAAAT0XQAABcA7AAABrJgAAE/dogAAAzcaA=
Accept-Language: fr-FR, en-US
Content-Language: fr-FR
X-MS-Has-Attach:
X-MS-TNEF-Correlator:
Content-Type: text/plain; charset=""us-ascii""
Content-Transfer-Encoding: quoted-printable
MIME-Version: 1.0

This is a dummy mail body.

This mail will be put in attachment for the test 5.



R


--_002_1372B39E66C04B4E9D95CF73184032B006FD4ABE03EMEAMAICLIED3_--";

            byte[] bytes = Encoding.UTF8.GetBytes(s);
            System.IO.MemoryStream stream = new System.IO.MemoryStream(bytes);
            SharpMimeMessage mime_message = new SharpMimeMessage(stream);
            String comment = get_comment(mime_message);
            System.Diagnostics.Debug.Print(comment);

        }



        public static string get_comment(SharpMimeMessage mime_message)
        {
            string comment = extract_comment_text_from_email(mime_message, "text/plain");
            if (comment == null)
            {
                comment = extract_comment_text_from_email(mime_message, "text/html");
            }

            if (comment == null)
            {
                comment = "NO PLAIN TEXT MESSAGE BODY FOUND";
            }

            return comment;
        }

        ///////////////////////////////////////////////////////////////////////
        public static string extract_comment_text_from_email(SharpMimeMessage mime_message, string mimetype)
        {

            string comment = null;

            // use the first plain text message body
            foreach (SharpMimeMessage part in mime_message)
            {
                if (part.IsMultipart)
                {
                    foreach (SharpMimeMessage subpart in part)
                    {

                        if (subpart.IsMultipart)
                        {
                            foreach (SharpMimeMessage sub2 in subpart)
                            {
                                if (sub2.Header.ContentType.ToLower().IndexOf(mimetype) > -1
                                && !is_attachment(sub2))
                                {
                                    comment = sub2.BodyDecoded;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (subpart.Header.ContentType.ToLower().IndexOf(mimetype) > -1
                            && !is_attachment(subpart))
                            {
                                comment = subpart.BodyDecoded;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (part.Header.ContentType.ToLower().IndexOf(mimetype) > -1
                    && !is_attachment(part))
                    {
                        comment = part.BodyDecoded;
                        break;
                    }
                }
            }


            return comment;
        }

        ///////////////////////////////////////////////////////////////////////
        public static bool is_attachment(SharpMimeMessage part)
        {
            string filename = part.Header.ContentDispositionParameters["filename"];
            if (string.IsNullOrEmpty(filename))
            {
                filename = part.Header.ContentTypeParameters["name"];
            }

            if (filename != null && filename != "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
