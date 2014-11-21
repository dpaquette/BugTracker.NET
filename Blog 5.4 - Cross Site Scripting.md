#Cross Site Scripting

One of the most common exploits on the web is cross site scripting or XSS. As you likely know you can embed JavaScript nearly anywhere in the DOM and the browser will happily execute it. This is a throw back to the days when all JavaScript looked like

   < button onclick="if(document.forms[0].elements[0].value == '') return false; return true;"/>

You may have noticed that you can include script tags in the head (as used to be suggested years ago) and in the footer (as is recommended for most scripts these days).

The problem with this is that it means that if users enter something that looks like JavaScript and the sever returns it then the browser will interpret it as JavaScript and execute it. This sort of behaviour is more common that you would expect. Any time that you show the search criteria that lead to a search results page this is an opportunity for a cross site scripting attack.

Being able to place arbitrary JavaScript on the page opens up all sorts of potential issues. On a login page the JavaScript could capture keystrokes and forward them to an attacker. On other pages actions could be executed. It is even possible that the injected script could perform AJAX actions. That is especiall problematic on a single page application where all actions are executed via AJAX end points. The requests sent from the injected script will carry with them the authentication cookies from the user's session granting it the same permissions as the user.

In the case of showing the search criteria this is a non-persistent XSS. This means that it will not remain on the site in a persisten fashion that could harm another person.
