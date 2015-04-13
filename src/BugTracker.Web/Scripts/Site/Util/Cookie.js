var BugTracker;
(function (BugTracker) {
    (function (Util) {
        var Cookie = (function () {
            function Cookie() {
            }
            Cookie.Set = function (name, value) {
                var date = new Date();

                // expire in 10 years
                date.setTime(date.getTime() + (3650 * 24 * 60 * 60 * 1000));

                document.cookie = name + "=" + value + ";expires=" + date.toUTCString();
                +";path=/";
            };

            Cookie.Get = function (name) {
                var nameEQ = name + "=";
                var ca = document.cookie.split(';');
                for (var i = 0; i < ca.length; i++) {
                    var c = ca[i];
                    while (c.charAt(0) == ' ')
                        c = c.substring(1, c.length);
                    if (c.indexOf(nameEQ) == 0)
                        return c.substring(nameEQ.length, c.length);
                }
                return null;
            };
            return Cookie;
        })();
        Util.Cookie = Cookie;
    })(BugTracker.Util || (BugTracker.Util = {}));
    var Util = BugTracker.Util;
})(BugTracker || (BugTracker = {}));
//# sourceMappingURL=Cookie.js.map
