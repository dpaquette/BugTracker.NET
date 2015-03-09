# JavaScript
The growing importance of JavaScript as a language cannot be ignored. It is possible and even common to see large applications written in JavaScript. JavaScript is not a new language and it is certainly one that has evolved over the years. During this evolution we have added much more rigor to it but inventing a way to organize code into classes and modules. Testability of the code has moved from a rare activity done only by running the code in a full browser to one integrated with command line runners that have no DOM. We are no longer in an age where it is acceptable to simply slap a few lines of JavaScript into an existing HTML page and call it done.

At least we hope that we're no longer in that era. BugTracker.net is again a product of the times. The JavaScript is minimal and completely embedded into individual pages. There is no sharing of code between pages through the use of external JavaScript files. In fact the most common piece of JavaScript in the application: a chunk used to submit the current form

```
<script>
function submit_form() {
    var frm = document.getElementById("<%:Form.ClientID%>");
    frm.submit();
    return true;
}
</script>
```

exists in near identical form on 14 pages.
