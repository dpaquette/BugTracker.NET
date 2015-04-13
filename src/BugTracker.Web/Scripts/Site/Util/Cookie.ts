module BugTracker.Util {
    export class Cookie {
        public static Set(name: string, value: string) {
            var date = new Date();

            // expire in 10 years
            date.setTime(date.getTime() + (3650 * 24 * 60 * 60 * 1000));

            document.cookie = name + "=" + value
            + ";expires=" + date.toUTCString();
            + ";path=/";
        }

        public static Get(name:string): string {
            var nameEQ = name + "=";
            var ca = document.cookie.split(';');
            for (var i = 0; i < ca.length; i++) {
                var c = ca[i];
                while (c.charAt(0) == ' ') c = c.substring(1, c.length);
                if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
            }
            return null;
        }
    }
}