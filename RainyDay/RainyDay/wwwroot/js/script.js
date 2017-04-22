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

var timer;

function ClearMessage() {
    $('message').text(""); // clear update messages
    clearTimeout(message_timer); // clear timeout
}

$(document).ready( // when jQuery and HTML document has loaded
    function () {
        ShowTime(); // show current time
        /*
        $('#keyword').change( // add event handler for element with id='keyword' (CSS selector) on change
            function () {
                var value = $(this).val(); // get value from input element, this = current element

                $.ajax( { // send ajax async HTTP request
                    url: '/Home/HandleAjax', // target url, in this case : /ControllerName/HandlerMethod
                    type: 'POST', // HTTP method : POST
                    dataType: 'text', // data type : text
                    data: { query : value }, // data to be sent : text input, key = parameter name in controller, value = data value
                    success: // if ajax request success, server response with HTTP 200 OK
                        function (response_data) { // received data : response_data
                            $('#your_keyword').text(response_data); // change content of element with id = "your_keyword" to response_data
                        }
                    } 
                );
            }
        );
        */
        // update feed and show progress

        $('#updater').click( // when update button clicked
            function () {
                $('#message').text("Updating RSS. This may take a while."); // change status to updating

                $.ajax({
                    url: '/Home/StartUpdate', // controller to handle ajax, /ControllerName/MethodName
                    type: 'POST', // HTTP method
                    success: // if server responds with HTTP 200 OK
                        function () { // show update succeed
                            var d = new Date();
                            var h = d.getHours();
                            var m = d.getMinutes();

                            var s = "RSS updated. Last udpate on " + h + ":" + m; // show message and last update time
                            $('#message').text(s); // change update message
                        },
                    error: // if server does not responds with HTTP 200 OK
                        function () {
                            $('message').text("Unable to update RSS. Please try again later."); // show error message
                            timer = setTimeout(ClearMessage, 3000); // hide message in 3s
                        }
                    }
                );
            }
        );
    }
);