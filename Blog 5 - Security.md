#Security

Between David and I we have about a decade of post secondary education in computery things. Despite that neither of us have either had a class on how to program securely. Secure programming is, honestly, not something that is even on the radar of most developers.

Even those developers who should be highly concerned with security are often lacking experience and training. Consider the recent [bugs](http://www.trendmicro.com/us/security/heartbleed/index.html) [in](http://arstechnica.com/security/2014/06/still-reeling-from-heartbleed-openssl-suffers-from-crypto-bypass-flaw/) [OpenSSL](https://devcentral.f5.com/articles/june-2014-openssl-vulnerabilities): this is a piece of software that is crucial in the security of the whole Internet. Everything from banking to shopping to browsing redit relies on SSL. Some of these bugs could have been avoided had OpenSSL been written in a memory managed environment such as C# or Java. However we cannot grow complacent in our confidence around memory management. From a security perspective memory management only solves the problems of walking off the end of arrays and mixing instructions and data. These are huge problems but if we look at the [top 10 security exploits](https://www.owasp.org/index.php/Top_10_2013-Top_10) as listed by the [Open Web Security Project(OWSP)](https://www.owasp.org) they don't even figure in.

In BugTracker.NET we have identified a couple of poor practices that may be exploitable. Some of them are easy fixes and others are, well, catastrophically difficult. Let's examine BugTracker.NET's security issues by going through the top 10 security exploits on OWSP's list.  Before we start remember that these aren't the only security exploits in the world.

I once did some work for a large engineering multinational who had a software security group. This group's mandate was to review applications deployed internally and access their security. Unfortunately the security team had been pulled from the general population of programmers and were not given any additional training. Thus their entire approach to security was predicated on complying with the top 10 list. We mortal developers had to address each point before being permitted to deploy our application. Despite the application I developed having no backing database I still had to come up with an approach for eliminating SQL injection attacks. Much hilarity ensued.

We'll just take a cursory look at all the potential issues in this post. Those that require more investigation and correction will be addressed in follow up posts.

##Injection Attacks

Holding #1 place on the list: injection attacks. An injection attack is where a value entered by the user contains an escape sequence such that the data is interpreted as an instruction by a lower level in the application. The most common form of injection attack is an SQL injection attack. When a user enters an SQL escape sequence into the application the application passes that straight through to the database.

As discussed in the [second post](http://blogs.msdn.com/b/cdndevs/archive/2014/09/19/evolving-asp-net-apps-evaluating-the-code.aspx) in this series there are some potential issues with SQL insertion attacks. Although I was unable to actually exploit an injection attack the insecure nature in which the site is programmed is problematic; black listing escape sequences is [prone to error](http://stackoverflow.com/questions/139199/can-i-protect-against-sql-injection-by-escaping-single-quote-and-surrounding-use). We will certainly need to correct this problem.

##Authentication and Session Management

The first thing I always check when looking at a legacy application is how passwords are stored. In the worst case scenario the passwords are stored in plain text. Almost as bad are applications where passwords are encrypted and not hashed. Even in cases where the password is hashed there are countless ways to mess up hashing (failing to use nonance, hashing with a poor algorithm, hashing with a checksum algorithm,...).

BugTracker.NET is about middle of the road when it comes to how it stores passwords. Passwords are hashed but not well hashed. We'll need to look at that in some detail.

##Cross Site Scripting

We are fortunate that by default ASP.net blocks most attempts at injecting scripts en route to the server. This is done through request validation. There is actually a great, if dated, [article in MSDN](http://msdn.microsoft.com/en-us/library/ff649310.aspx) that describes a multi-step process to protect against cross site scripting in ASP.net. The first step in the process is to enable request validation. This has been done correctly in the web.config

```
<httpRuntime requestValidationMode="2.0" executionTimeout="110" maxRequestLength="51200"/>
```
However it possible to override this setting on a page by page basis in the unlikely event that you need to submit information that contains tags. This is done in the ASP.net page declaration through the use of

```
validateRequest="false"
```
Searching the project for these yields [quite a few examples](https://github.com/dpaquette/BugTracker.NET/search?p=2&q=validateRequest%3D%22false%22&type=Code&utf8=%E2%9C%93). The thing is that turning off request validation is not necessarily bad, unless we're printing the content back to the web browser unencoded. This is however difficult to find every case where unescaped values can be written back out to the browser. The difficulty of tracking down possible cross site scripting vulnerabilities when request validation is disabled is half the problem.

This topic is complicated enough to warrant an entire blog post as we investigate whether proper care has been taken to avoid cross site scripting attacks. We'll introduce some tricks to make it easier for future developers to know that cross site scripting attacks have been mitigated.

##Insecure Direct Object References

Often there are parts of the system to which a user might have only partial access. In BugTracker.NET a great example would be a bug. A user from project A should not have access to a bug from project B. However the same screen is used for accessing each bug with the only difference being the bug ID. Checks have to be made in the code to avoid displaying bugs that users should not be able to see.  This is actually a pretty common exploit.

Fortunately, it looks like there are extensive checks throughout the application for user permissions. The checks look something like

```
 // look at permission level and react accordingly
    permission_level = (int)dr_bug["pu_permission_level"];

    if (permission_level == Security.PERMISSION_NONE)
    {
        btnet.Util.display_you_dont_have_permission(Response, security);
        return;
    }
```

They aren't written in the clearest code but they do seem effective. We probably don't need to investigate this issue right now. It would be nice to have some test in place to reassure ourselves that we're not leaking data but it isn't critical right now. We'll look at testing as a whole later in the series.

##Security Misconfiguration

Much of the configuration for hosting BugTracker.NET resides in IIS. It is possible to use the web.config to drive this configuration. If we look at the web.config file we find that it is just about the best documented configuration file in the world. There are dozens and dozens of settings you can change.

BugTracker.NET was designed to be highly configurable. You can add custom fields to bugs, login with LDAP and integrate with a handfull of different source control tools. This is all set up in the configuration file.

Configuring complex applications is a real problem these days. Systems like the ERPs from large companies whose names are similar to SAT and Boracle can take months to properly configure. Many people earn a good living as implementation specialists on these systems, moving from company to company adapting the software to match the company's business requirements.

Does BugTracker.NET really need to be this complicated? It is difficult to say. At some point in the past the functionality was important to somebody otherwise it would not have been written. What we need is some tooling in place to gather information about which services are being used and which aren't. Removing unused code will reduce the surface area of the application making it easier to maintain and easier to guard against attacks.

We'll skip over streamlining the configuration in this section on security. Simplifying the configuration will be the topic of, oh, at least one post in the future.

##Sensitive Data Exposure

There is frequently some information in our system that we don't want to disclose to outside parties. This can be as minor as an error message revealing a stack trace or as serious as exposing SQL or even full database dumps on the web.

Weirdly, all three of these data exposures are present in BugTracker.NET. The default configuration in the web.config reveals full stack traces for any error, there are several places where SQL is displayed and there is a way to generate and download a full database dump.  The latter two are actual features in the application. They are, however, extremely high risk features. Being able to download a database dump in particular exposes the application passwords as well as pretty much everything else.

We're going to drop these features. It is possible that we'll lose some customers as a result of this but we have a responsibility to deliver secure software. BugTracker.NET is open source so somebody is able to branch the application before we remove the features and release a competitor. They are welcome to take on the responsibility of distributing insecure software; I'm not interested in that.

##Missing Function Level Access Control

Ensuring that lower level users cannot visit pages that are restricted to higher level users is important. Attacks on your site may come from users who are valid. It is also possible that an external attacker may have an easier time exploiting user level accounts and then elevating their privileges.

There are extensive checks in place inside BugTracker.NET to prevent these sorts of attacks. The difficulty again is that the checks for this are scattered all over the application. It is hard to know if every possible escalation is protected against. For the moment I'm comfortable leaving this alone. At some later point it may be worthwhile extracting the user level functionality to some common module. This may actually be an ideal place to leverage aspect oriented programming.

##Cross-Site Request Forgery

Cross-site request forgery is an interesting problem. By tricking authenticated users into going to a specific URL at attacker can perform an action as if they are that user. So if I sent you a link in your e-mail to

    http://www.yourbank.com/sendallmymoneyto?person=simon

and you clicked on it having already authenticated with the bank then you could, inadvertently, send me all your money.

The easiest way to avoid these sorts of attacks is to ensure that each action submission requires a token be submitted along with the form and then matched with a second value from a cookie. This needs to be done for every form. Under certain configurations this is all handled for us by ASP.net. In fact you pretty much have to go out of your way and disable event validation to make your web forms site vulnerable to cross-site request forgery. We really don't need to worry about this.

##Using Components with Known Vulnerabilities

In the previous section we went though all the third party components in BugTracker.NET and updated them to the latest. There are no known security flaws in these components. However, our vigilance cannot end there. A secure application can rapidly become an insecure application as vulnerabilities are discovered in upstream components. This is why we put so much effort into finding components on nuget for our component upgrades: it is far easier to simply issue an

```
Update-Packages
```

command than to have to dig around every time there is a new release.

For the moment no further action needs to be taken to secure ourselves form insecure components.

##Unvalidated Redirects and Forwards

The concern here is that we might have a page that forwards a user onto another page and that the URL to which to forward be specified in a query parameter. This would allow an attacker to create a link that seemed to go to a BugTracker.NET deployment but didn't actually. For instance one might create a URL like so

    http://legitimate.example.com/refer?url=http://evil.example.com

The page refer would send a user on to another site. This other site could be made to look exactly like an install of BugTracker.NET such that it would trick users into revealing secret data. For instance imagine a fake password change page that required the users to enter their old password. The password would then be captured by the evil site allowing them to exploit the original site. A typical example of a place where this might be used is on a login screen where it refers the user back to the page they tried to visit without being authenticated.

Testing this with exact exploit with BugTracker.net we find that it is indeed vulnerable to such attacks. On my local build of BugTracker.NET I can enter

    http://localhost:33616/default.aspx?url=http://google.com

This takes me to the login page and once I log in I find my browser redirected to the google home page.

So this is for sure going to need to be addressed. There may be some other places in the application that also allow for this. Because the application allows for users to enter their own links as part of the bug description all the links in bugs can also be exploited.

##Where We Stand

So far we've looked at ten possible security issues in the application. Of them five of them look to be of concern.

 - SQL Injection
 - Passwords strength
 - Cross site scripting
 - Sensitive data revealed
 - Unvalidated forwards

 Over the next couple of posts we'll address each of these in BugTracker.NET.
