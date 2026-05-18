window.cookieHelper = {
    getCookie: function (name) {
        let match = document.cookie.match(new RegExp('(^| )' + name + '=([^;]+)'));
        if (match) return match[2];
        return null;
    },
    setCookie: function (name, value, days) {
        let expires = "";
        if (days) {
            let date = new Date();
            date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
            expires = "; expires=" + date.toUTCString();
        }
        document.cookie = name + "=" + (value || "") + expires + "; path=/";
    },
    deleteAllCookies: function () {
        const cookies = document.cookie.split(";");

        for (let i = 0; i < cookies.length; i++) {
            const cookie = cookies[i].split("=")[0].trim();
            document.cookie = cookie + "=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/";
        }
    }
};
