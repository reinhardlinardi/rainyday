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

    var val = d.getMinutes();
    if (val < 10) s += ':0';
    else s += ':';
    s += val;

    var val = d.getSeconds();
    if (val < 10) s += ':0';
    else s += ':';
    s += val;

    // Update HTML view
    document.getElementById('time').innerHTML = s;

    // Run ShowTime in 1000 ms from now
    setTimeout(ShowTime, 1000);
}

$(document).ready( // when jquery and HTML document has loaded
    function () {
        ShowTime(); // show current time

        $('#keyword').change( // add event handler for element with id='keyword' (CSS selector) on change
            function () {
                var value = $(this).val(); // get value from input element, this = current element

                $.ajax( { // send ajax async HTTP request
                    url : '/Home/HandleAjax', // target url, in this case : /ControllerName/HandlerMethod
                    type : 'POST', // HTTP method : POST
                    dataType : 'text', // data type : text
                    data: { query: value }, // data to be sent : text input, key = parameter name in controller, value = data value
                    traditional : true,
                    success: // if ajax request success, server response with HTTP 200 OK
                        function (response_data) { // received data : response_data
                            $('#your_keyword').text(response_data); // change content of element with id = "your_keyword" to response_data
                        }
                    } 
                );
            }
        );
    }
);

/*
    function () {
        $("#keyword").change(
            function () {
                var value = $(this).val();
                $("#you_typed").text("You typed : " + value);
            }
        );
    }
*/