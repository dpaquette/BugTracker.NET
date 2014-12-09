#Cookie Authentication
We have already addressed how to securely hash a password and check username and password on login. In this section, we will explore how to authenticate and authorize requests after the user has successfully logged in.

In many web applications, once a user has authenticated using their username and password, the user is issued a token that can be used to identify the user and validate that they have already been authenticated. That token is most often stored as a cookie that accompanies every request the client makes. Traditionally, in an ASP.NET Web Forms application, generating and subsequently validating this cookie was the responsibility of the [Forms Authentication module](http://msdn.microsoft.com/en-us/library/system.web.security.formsauthentication). In the latest versions of ASP.NET, this is done using [OWIN Cookie Authentication middleware](http://blogs.msdn.com/b/webdev/archive/2013/07/03/understanding-owin-forms-authentication-in-mvc-5.aspx).

Based on our experience with the code-base so far, I was not surprised to see that BugTracker.NET had implemented cookie authentication on its own. Generally speaking, the implementation is not that bad, but there are definitely some things that could be done better. First of all, the contents of the authentication cookie are stored as plain text. A more secure implementation would encrypt all the values into a single string and store those as a single cookie that doesn't make sense to people looking at it.

[View the current implementation](https://github.com/dpaquette/BugTracker.NET/blob/0846d1401c2b7db4f7810fcbc9ef180f403bbcc7/src/BugTracker.Web/btnet/security.cs)

Another potential security risk is that the implementation is using a GUID as a random session id. As it turns out, GUIDs generators are [not  random and are actually predictable](http://blogs.msdn.com/b/oldnewthing/archive/2012/05/23/10309199.aspx).

We could try to address these issues with the current implementation, but I really think it would be better to make use of a solid existing implementation. I just don't want to deal with this code in my application if I don't have to.

Before we get started, it is important to note that the implications these changes are significant. The Security class is used everywhere to check authorization of whether or not a user should be given access to a particular page or feature within a page. You will want to set aside a good amount of time for testing when making changes of this magnitude.

Let's break this into 2 pieces: Authentication and Authorization.

##Authentication
Simon already tackled the first part of Authentication when he improved the password hashing algorithm. The next step is issuing the user some sort of token that we can use to determine that they have already authenticated. We do this because we do not want to pass the username and password with each request. Not only would that be potentially insecure because it would require us to store the password somewhere on the client, but the overhead of checking the password every time could also cause performance issues.

In BugTracker, the token is generated in the `btnet.Security.create_session` method:

		public static void create_session(HttpRequest Request, HttpResponse Response, int userid, string username, string NTLM)
		{

			// Generate a random session id
			// Don't use a regularly incrementing identity
			// column because that can be guessed.
			string guid = Guid.NewGuid().ToString();

			btnet.Util.write_to_log("guid=" + guid);

			var sql = new SQLString(@"insert into sessions (se_id, se_user) values(@gu, @us)");
			sql = sql.AddParameterWithValue("gu", guid);
			sql = sql.AddParameterWithValue("us", Convert.ToString(userid));

			btnet.DbUtil.execute_nonquery(sql);

			HttpContext.Current.Session[guid] = userid;

			string sAppPath = Request.Url.AbsolutePath;
			sAppPath = sAppPath.Substring(0, sAppPath.LastIndexOf('/'));
			Util.write_to_log("AppPath:" + sAppPath);

			Response.Cookies["se_id"].Value = guid;
			Response.Cookies["se_id"].Path = sAppPath;
			Response.Cookies["user"]["name"] = username;
			Response.Cookies["user"]["NTLM"] = NTLM;
			Response.Cookies["user"].Path = sAppPath;
			DateTime dt = DateTime.Now;
			TimeSpan ts = new TimeSpan(365, 0, 0, 0);
			Response.Cookies["user"].Expires = dt.Add(ts);
		}

In subsequent requests, BugTracker compares the session_id and username from the cookie with the session_id and username stored in the database. If the values match, then the user is considered to be authenticated. The code that does this is rather convoluted as it seems to intertwine both Authentication and Authorization concerns in a single method. We are going to separate these out to make it much easier. In a normal ASP.NET application we can check if a user is authenticated by inspecting `User.IsAuthenticated`. The `User` property is available from any Page, UserControl, MVC controller or Web API controller.

###Adding and Configuring Owin Middleware Components
The first thing we need to do is add the Owin Cookie Authentication middleware packages.

   Install-Package Microsoft.Owin.Security.Cookies
   Install-Package Microsoft.Owin.Host.SystemWeb

This will install a handful of other required packages. To configure the Owin middleware, we will add file named Startup.Auth.cs to the App_Start folder. In that file, we will add the following code:


    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            // Enable the application to use a cookie to store information for the signed in user
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "ApplicationCookie",
                LoginPath = new PathString("/default.aspx")
            });
        }
    }

And finally, a Startup.cs file in the root folder with the following code:

      [assembly: OwinStartup(typeof(btnet.Startup))]
      namespace btnet
      {
          public partial class Startup
          {
              public void Configuration(IAppBuilder app)
              {
                  ConfigureAuth(app);
              }
          }
      }

If you are wondering where this code comes from, it is a stripped down version of the code that is generated by the MVC5 File -> New Project template. All we have done is hooked in to the application startup and configured Owin Cookie authentication.

[View the commit - Added and configured OWIN Cookie Authentication Middleware](https://github.com/dpaquette/BugTracker.NET/commit/be03b88a0d2a44b4eb8ff149af5723c46e451397)

###Signing In
Our new approach to signing in will involve loading data from BugTracker.NET's User table and adding them as Claims into a new `ClaimsIdentity`. A Claim can be any name-value pair that will be associated with a particular user. The Owin cookie middleware will then encrypt that information and store it in a cookie. In subsequent requests, the middleware will decrypt the cookie to check if the user is currently authenticated. In the application, we will have the access to the Claims without needed to check the database with every request.

The first step will be to create a new ClaimsIdentity after the username and password have been confirmed. For starters, we will add claims for the User ID, User Name and Organization ID as these appear to be the most commonly used throughout the application.

        public static void SignIn(HttpRequest request, string username)
        {
            SQLString sql = new SQLString("select us_id, us_username, us_org from users where us_username = @us");
            sql = sql.AddParameterWithValue("us", username);
            DataRow dr = btnet.DbUtil.get_datarow(sql);

            var claims = new List<Claim>
            {
                new Claim(BtnetClaimTypes.UserId, Convert.ToString(dr["us_id"])),
                new Claim(ClaimTypes.Name, Convert.ToString(dr["us_username"])),
                new Claim(BtnetClaimTypes.OrganizationId, Convert.ToString(dr["us_org"]))
            };

            var identity = new ClaimsIdentity(claims, "ApplicationCookie", ClaimTypes.Name, ClaimTypes.Role);
            var owinContext = request.GetOwinContext();
            owinContext.Authentication.SignIn(identity);
        }

Now, instead of calling btnet.Security.create_session, we call btnet.Security.SignIn:

        if (authenticated)
        {
            //... Code omitted for brevity
            btnet.Security.SignIn(Request, username);
            btnet.Util.redirect(Request, Response);
        }
        else
        {
            msg.InnerText = "Invalid User or Password.";
        }

I used the built in `ClaimTypes.Name` for the username, but created custom `BtnetClaimTypes` for any claims that are unique to BugTracker. Claims types are simply unique string values.

If we inspect the request headers after the user has signed in, we can see that each request includes a cookie named .AspNet.ApplicationCookie. You can see that the Value is encrypted and not human readable.

![New Authentication Cookie](images/applicationcookie.png)

[View the commit - Signing in using Owin Cookie middleware](https://github.com/dpaquette/BugTracker.NET/commit/6fdd6bd350fd7e6020c0c098dd3b0dd760f0a2de)

###Signing Out
Signing a user out of the application is a simple process with Owin Authentication.

     public static void SignOut(HttpRequest request)
     {
         var owinContext = request.GetOwinContext();
		     owinContext.Authentication.SignOut();
     }

Now, on the logoff page we can simply call this SignOut method instead of manually deleting cookies.

[View the commit - Sign Out using Owin Cookie Authentication](https://github.com/dpaquette/BugTracker.NET/commit/f3ba11f37e50f75ffc204f713c256297cc80d4f7)


###Access User Claims
Our next step will be to modify all the application pages to use the ClaimsIdentity to check for user information rather than the current approach of calling btnet.Security.check_security. For example, the bugs.aspx page starts out by doing the following:

            security = new Security();
            security.check_security(HttpContext.Current, Security.ANY_USER_OK);

The check_security method is a complex piece of code that compares custom session cookies with value from the users and sessions tables. Our new code will bypass this entirely. We can easily check if a user is signed in by checking the `User.Identity.IsAuthenticated` property. This property is available from any Page, UserControl, MVC Controller or Web API controller in ASP.NET. If we inspect the User.Identity property at runtime, we can see that the value is set to the ClaimsIdentity that we created when signing in using Owin.

![Inspect User.Identity at runtime](images/claimsidentityruntime.png)

The Owin middleware has decrypted the cookie value and populated the ASP.NET User for us, making it much easier to gain access to the current users information.

The bugs.aspx page makes us of some additional user information to customize the contents of the page. The current implementation accesses this information from the btnet.Security.user property.

     sql = sql.AddParameterWithValue("us", security.user.usid);

In our new approach, we will get this information from the UserID claim on the User.Identity property.

     ClaimsIdentity identity = (ClaimsIdentity) User.Identity;
     Claim userIdClaim = identity.FindFirst(BtnetClaimTypes.UserId).
     int userId = Convert.ToInt32(userIdClaim.Value);
     sql = sql.AddParameterWithValue("us", userId);

This code is a little verbose so we are going to encapsulate it using an extension method in a way that can be easily tested and reused for other claims types.

    public static class ClaimsIdentityExtensions
    {
        public static int GetUserId(this IIdentity identity)
        {
            return Convert.ToInt32(GetClaimsValue(identity, BtnetClaimTypes.UserId));
        }

        private static string GetClaimsValue(IIdentity identity, string claimType)
        {
            if (identity is ClaimsIdentity)
            {
                ClaimsIdentity claimsIdentity = (ClaimsIdentity) identity;

                Claim userIdClaim = claimsIdentity.FindFirst(claimType);
                if (userIdClaim != null)
                {
                    return userIdClaim.Value;
                }
                else
                {
                    throw new SecurityException(string.Format("Identity is missing value for claim type {0}", claimType));
                }
            }
            else
            {
                throw new SecurityException("Identity is not a valid Claims Identity");
            }
        }
    }

Now, the code in bugs.aspx is much easier to understand.

     sql = sql.AddParameterWithValue("us", User.Identity.GetUserId());

[View the commit - Modified Bugs page to get user information from User.Identity](https://github.com/dpaquette/BugTracker.NET/commit/6abe4bbf794ff6242ac52491f4e8994743dcebf9)

In BugTracker, an instance of the Security class is passed along to a number of utility methods. In the example of Bugs.aspx, the security object is passed to `btnet.BugList.display_bugs`. This method in turn accesses user information to customize the way bugs are displayed on the screen. Since we are no longer checking authentication and authorization using the Security class, we will need to change a number of these methods. Instead of passing the Security class, we will pass the ClaimsIdentity. Since these methods require user information that we are not loading yet, we will need to a some new claim types to our ClaimsIdentity.

     new Claim(BtnetClaimTypes.BugsPerPage, Convert.ToString(dr["us_bugs_per_page"]))

NOTE: btnet.BugList.display_bugs is writing html to the Context.Response. This logic is better suited as a Web Forms UserControl or MVC PartialView. We will address this in the near future.

[View the commit - Modified DisplayBugs to use Identity](https://github.com/dpaquette/BugTracker.NET/commit/44e1bea734bbef97223aa30f86093e536473634d)

Another example from Bugs.aspx is `Util.alter_sql_per_project_permissions`. In this method, some custom SQL is appended to restrict what the user will see depending on the permissions that user has been granted. Again, we added some new claim types and modified the util method to expect an Identity object instead of the btnet.Security object:

     new Claim(BtnetClaimTypes.CanOnlySeeOwnReportedBugs, Convert.ToString(dr["og_can_only_see_own_reported"])),
     new Claim(BtnetClaimTypes.OtherOrgsPermissionLevel, Convert.ToString(dr["og_other_orgs_permission_level"]))

[View the commit - Row Level security using Identity](https://github.com/dpaquette/BugTracker.NET/commit/f00ad0c70a73243dbe0e7609adbb8d83443afebd)

A few more changes to remove any dependencies on btnet.Security:

- [View the commit - Added EnablePopup Claim](https://github.com/dpaquette/BugTracker.NET/commit/395c131f23eae530696fb43980098ece7ad701e5)
- [View the commit - Added CanAddBugs Claim](https://github.com/dpaquette/BugTracker.NET/commit/0de625228ee87d9d864614cecd4bc482eb7b0f45)
- [View the commit - Added TagsPermissionLevel Claim](https://github.com/dpaquette/BugTracker.NET/commit/b481023553743367a67e1cf33b75b61ec18ba21e)

There is one last change to make before we have a working version of Bugs.aspx. The code that creates the header menu makes use of special type of user claim that would be better suited as a Role.

###User Roles
A user role is a special type of claim that gives users access to a particular portion of the system. A standard example would be an Administrator role. In most systems, users that have been granted the Adminstrator role have access to pages that other users would never see.

In a typical ASP.NET application, we can check if a user has been granted a particular role by checking `User.IsInRole("RoleName")`. In BugTracker.NET, this is done by checking a variety of boolean values on the btnet.Security.user object instance. For example, to check for if a User is an administrator, we would check the `security.user.is_admin` property.

The current approach is a little messy and we know that it won't play well with role based authorization in MVC and WebAPI. Luckily, there is an easy way for us to tie in to the standard Role based approach using ClaimsIdentity. When we create a new ClaimsIdentity, we specify a parameter that identifies which claim type refers to roles. We can use the built in ClaimTypes.Role for this:

    var identity = new ClaimsIdentity(claims, "ApplicationCookie", ClaimTypes.Name, ClaimTypes.Role);

All we need to do is add a Role claim for any role that the user has been granted.

     if ((int)dr["us_admin"] == 1)
     {
          claims.Add(new Claim(ClaimTypes.Role, BtnetRoles.Admin));
     }

In the `write_menu` method we can check for the Admin role as follows:

     user.IsInRole(BtnetRoles.Admin)

Unfortunately, the current the write_menu method is located in btnet.security. This is a strange place and causes us some problems with naming conflicts. Let's move the implementation to a UserControl. The process of moving code from write_menu to a user control is a little tedious but not as bad as I originally anticipated. The HTML could use some improvement but the resulting code is much easier to understand.

[View the commit - Added MainMenu User Control](https://github.com/dpaquette/BugTracker.NET/commit/42f054f0f2b6b73021d3f8ba948cfa08e0552c42)

We now have a working Bugs.aspx page that no longer relies on the custom session logic and custom authentication cookies. Our new approach is more closely aligned with a standard Authentication and Authorization implementation ASP.NET, which means BugTracker.NET will play nice with MVC and WebAPI when we start using them.

##Authorization (Restricting Page Access)
Authorization refers to the process of checking whether or not a user should be granted access to a particular page or feature in the application. Previously, this was done by calling `btnet.Security.check_session` with some special parameters in Page_Load method of every single page in the application. Now that we are signing in using Owin cookie authentication middleware, we should be able to us a more standard approach to solving this problem.

In classic Web Forms, this is done by configuring the [location and authorization](http://weblogs.asp.net/gurusarkar/setting-authorization-rules-for-a-particular-page-or-folder-in-web-config) elements of Web.config. I prefer to keep this type of logic in code versus configuration.

In MVC and WebAPI, access to controllers and action methods can be restricted using the Authorize and AllowAnonymous attributes. Here is a simply example from [MSDN](http://msdn.microsoft.com/en-us/library/system.web.mvc.authorizeattribute.aspx).

	[Authorize]
	public class AccountController : Controller
	{
	    public AccountController () { . . . }

	    [AllowAnonymous]
	    public ActionResult Register() { . . . }

	    public ActionResult Manage() { . . . }

	    public ActionResult LogOff() { . . . }

	}

By adding the Authorize attribute to the controller, we are stating that users must be logged in before they can access any of the action methods and views associated with this controller. By adding the AllowAnonymous attribute to the Register method, we are saying that a user does not need to be authenticated to access the Register page.

Using the Authorize attribute, we can also specify that a user must have a particular role in order to access an action method.

	[Authorize(Roles="Admin")]
	public class AdminController : Controller
	{
		...
	}

We really like the attribute based approach, but unfortunately this feature is not built in to Web Forms. We can't use the MVC attributes because they rely on ActionFilters which just don't exist in the Web Forms page life-cycle.

Luckily, in one of our early changes we introduced a BasePage that all our pages inherit from. By creating our own attributes and  placing the authorization logic in our BasePage class, we can implement an authorization mechanism that is very similar to the MVC approach. We named our attribute PageAuthorize and PageAllowAnonymous to avoid confusion with the MVC attributes.

    [PageAuthorize(BtnetRoles.User)]
    public class BasePage : Page
    {
        protected override void OnPreInit(EventArgs e)
        {
            if (!IsUserAuthorized())
            {
                Response.Redirect(string.Format("default.aspx?returnUrl={0}", HttpUtility.UrlEncode(Request.RawUrl)));
            }
            base.OnPreInit(e);
        }

        private bool IsUserAuthorized()
        {
            bool hasAnonymous = Attribute.GetCustomAttributes(
							            GetType(),
						              typeof (PageAllowAnonymous)).Any();
            if (hasAnonymous)
            {
                return true;
            }

            var attributes = Attribute.GetCustomAttributes(
							              GetType(),
														typeof (PageAuthorizeAttribute)).Cast<PageAuthorizeAttribute>();
            return Page.User.Identity.IsAuthenticated
				 && attributes.All(a => a.OnAuthorize(Page.User));
        }
			}

By default every page will require that the User is authenticated and has been granted the User role. User is a role that we will grant to all users who have a valid account. We also provide an option to allow anonymous access because some pages should be accessible by users who are not signed in. For example, we can add the PageAllowAnonymous attribute to the login page (default.aspx):

    [PageAllowAnonymous]
    public partial class @default : BasePage

For the Bugs.aspx page, we don't need to add any attributes because the default behaviour is exactly what we want. If we wanted to restrict a page to only users who have either the Admin or ProjectAdmin role, we would add a PageAuthorize attribute to that specific page:

    [PageAuthorize(BtnetRoles.Admin, BtnetRoles.ProjectAdmin)]
    public partial class categories : BasePage
    {
    }

Note: This design was the result of a discussion that happened during a code review. You can progression of the design in the commits listed here.

- [View the commit - Added Authorization Rules to base page](https://github.com/dpaquette/BugTracker.NET/commit/7496fa1893a0a39686107d7dd61be3fbf3560d58)
- [View the commit - Modified all pages to use Authorization rules from base page](https://github.com/dpaquette/BugTracker.NET/commit/533c47e90362add59b54495fc3b51f78d9dbaa92)
- [View the commit - Attribute based page authorization ](https://github.com/dpaquette/BugTracker.NET/commit/4bcb1bb2d32852205e05f693a2616c9dbf158463)

##Wrapping it up
Now that we have a framework in place for Authentication and Authorization, we need to replace all references to the old custom session based implementation. This is a big task as the security code is used everywhere. You can see all the change summarized in the pull request below.

[View Pull Request](https://github.com/dpaquette/BugTracker.NET/pull/17)

Our new implementation makes use of the latest Owin Cookie Authentication middleware. As a result, our implementation is easier to understand and will work much better with newer technologies like MVC and WebApi. The new implementation is also arguably much more secure as it relies on a solid implementation from Microsoft rather than our own custom cookie authentication. Authorization has been simplified greatly as the developer is no longer required to call check_security on every page request: authorization behaviour is inherited automatically.
