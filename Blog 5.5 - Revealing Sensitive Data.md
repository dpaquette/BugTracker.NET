#Revealing Sensitive Data

The final security issue we're going to address is that of revealing sensitive data. This data can be revealed through a misapplication of a cryptographic mechanism such as SSL or by leaving unencrypted such things as database backups. We have, in recent months, seen a large number of SSL vulnerabilities that may be used to reveal data. BugTracker.NET does not explicitly require SSL and the implementation of SSL is left up to the end user.

Deploying SSL certificates was once a very expensive proposition. In fact the first version of the authentication protocol OAuth protocol was designed in a way that would maintain security without having to resort to using SSL. Since that time SSL certificates have dropped in price and can even be acquired for free. Security guru and Microsoft MVP Troy Hunt has an [excellent tutorial](http://www.troyhunt.com/2013/09/the-complete-guide-to-loading-free-ssl.html) on how to implement SSL on Azure using a free SSL certificate. The [Electronic Frontier Foundation](https://www.eff.org) have partnered with a number of other players such as mozilla and Cisco to produce [Let's Encrypt](https://letsencrypt.org/) which is "a free, automated, and open certificate authority (CA), run for the public’s benefit". Meaning that it will be trivial to generate new SSL certificates for 0 cost and easily integrate them with your existing server. The HTTP 2.0 protocol at one point required full time SSL but that requirement has since, disapointingly, been removed.  The point of all of this is that SSL adoption should no longer be seen as difficult or expensive: you should just do it. Once you have it set up then you can use a tool such as [Qulays SSL Labs](https://www.ssllabs.com/ssltest) to ensure that the certificate has been properly applied and that the certificate is sufficiently secure.

Even with SSL properly deployed some programming mistakes can expose the application to attack. BugTracker.NET has a couple of rather interesting features that put it at a huge risk. The first of these is the Ad Hoc Query Tool. This tool is designed to allow administrators to run whatever query they would liek agianst the database.
![](Images/adhoc.jpg)

From the warning on the page it is clear that the author knew that there was a security issue with the page. This is because any data from the database can be retrieved and displayed.

![Hashed passwords from the users table](Images/adhoc1.jpg)

It is not just queries that can be run but also delete rows or update them

![Hashed passwords from the users table](Images/adhoc2.jpg)

Shipping the page at all is a bad idea.
