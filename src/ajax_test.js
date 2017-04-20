$(document).ready(
    function() {
        $("#keyword").on("change keyup",
            function() {
                $("#you_typed").text("You typed : " + $(this).val());
            }
        );
    });