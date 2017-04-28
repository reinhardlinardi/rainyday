function ShowTime() {
    // Array of days and months name
    var days = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
    var months = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];

    // Create new date object
    var d = new Date();
    var s = '';

    // Get date information
    s += days[d.getDay()];
    s += ', ' + d.getDate();
    s += ' ' + months[d.getMonth()];
    s += ' ' + d.getFullYear();

    // Format string to be displayed
    var val = d.getHours();
    if (val < 10) s += '\xa0\xa00'; // \xa0 character = non breaking space, will not be ignored by browser
    else s += '\xa0\xa0';
    s += val;

    val = d.getMinutes();
    if (val < 10) s += ':0';
    else s += ':';
    s += val;

    val = d.getSeconds();
    if (val < 10) s += ':0';
    else s += ':';
    s += val;

    // Update HTML view
    document.getElementById('time').innerHTML = s;

    // Run ShowTime in 1000 ms from now
    setTimeout(ShowTime, 1000);
}

var message_timer;
var animation_timer;

function ClearMessage() {
    $('#message').text(""); // clear update messages
    clearTimeout(message_timer); // clear timeout
}

function Animation(dots)
{
    if (dots == 4) dots = 1;

    var s = "Updating RSS";
    for (i = 1; i <= dots; i++) s += ".";

    $('#message').text(s); // print updating message

    animation_timer = setTimeout(function () { Animation(dots + 1); }, 1000); // repeat animation every 1s
}

function SendKeyword() {
    var keyword_val = $('#keyword').val(); // get value of keyword
    var algorithm_val = $('#algorithm').val(); // get value of algorithm

    $.ajax({
        url: '/Home/SearchKeyword', // send to Home Controller, method SearchKeyword
        type: 'POST', // method = POST
        dataType: 'text', // data = text
        data: { keyword: keyword_val, algorithm: algorithm_val }, // key = parameter name, value = value
        success:
        function (result) {
            $('#news_feed').html(''); // remove all html inside div
            $('#news_feed').append(result); // append result to div
            }
        }
    );
}

$(document).ready( // when jQuery and HTML document has loaded
    function () {
        ShowTime(); // show current time

        $('#updater').click( // when update button clicked
            function () {
                $('#message').text("Updating RSS"); // change status to updating

                $.ajax({
                    url: '/Home/StartUpdate', // controller to handle ajax, /ControllerName/MethodName
                    type: 'POST', // HTTP method
                    success: // if server responds with HTTP 200 OK
                        function () { // show update succeed
                            var d = new Date();
                            var h = d.getHours();
                            var m = d.getMinutes();

                            var hours = '';
                            var minutes = '';

                            if (h < 10) hours += '0';
                            hours += h;

                            if (m < 10) minutes += '0';
                            minutes += m;

                            var s = "RSS updated. Last update on " + hours + ":" + minutes; // show message and last update time
                            $('#message').text(s); // change update message
                            clearTimeout(animation_timer); // stop animation
                        },
                    error: // if server does not responds with HTTP 200 OK
                        function () {
                            $('#message').text("Unable to update RSS. Please try again later."); // show error message
                            clearTimeout(animation_timer); // stop animation
                            message_timer = setTimeout(ClearMessage, 3000); // hide message in 3s
                        }
                    }
                );

                animation_timer = setTimeout(function () { Animation(1); }, 1000); // play animation in 1s
            }
        );

        $('#keyword').keyup(SendKeyword); // when keyword changes, search keyword
        $('#algorithm').change(SendKeyword); // when algorithm changes, search keyword
    }
);