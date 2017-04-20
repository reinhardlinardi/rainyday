$(document).ready(
    function ShowTime() {
        var days = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
        var months = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];

        var d = new Date();
        var s = "";

        s += days[d.getDay()];
        s += ", " + d.getDate();
        s += " " + months[d.getMonth()];
        s += " " + d.getFullYear();

        var val = d.getHours();
        if (val < 10) s += "\xa0\xa00"; // character \xa0 = non breaking space
        else s += "\xa0\xa0";
        s += val;

        var val = d.getMinutes();
        if (val < 10) s += ":0";
        else s += ":";
        s += val;

        var val = d.getSeconds();
        if (val < 10) s += ":0";
        else s += ":";
        s += val;

        document.getElementById("time").innerHTML = s;
    },

    function OnPageLoad() {
        setInterval(ShowTime, 1000);
    }
);