$(document).ready(function () {
    //Hidden Password
    $("#show_hide_password a").on('click', function (event) {
        event.preventDefault();
        if ($('#show_hide_password input').attr("type") == "text") {
            $('#show_hide_password input').attr('type', 'password');
            $('#show_hide_password svg').addClass("fa-eye-slash");
            $('#show_hide_password svg').removeClass("fa-eye");
        } else if ($('#show_hide_password input').attr("type") == "password") {
            $('#show_hide_password input').attr('type', 'text');
            $('#show_hide_password svg').removeClass("fa-eye-slash");
            $('#show_hide_password svg').addClass("fa-eye");
        }
    });
    //Hidden Confirm Password
    $("#show_hide_confirm_password a").on('click', function (event) {
        event.preventDefault();
        if ($('#show_hide_confirm_password input').attr("type") == "text") {
            $('#show_hide_confirm_password input').attr('type', 'password');
            $('#show_hide_confirm_password svg').addClass("fa-eye-slash");
            $('#show_hide_confirm_password svg').removeClass("fa-eye");
        } else if ($('#show_hide_confirm_password input').attr("type") == "password") {
            $('#show_hide_confirm_password input').attr('type', 'text');
            $('#show_hide_confirm_password svg').removeClass("fa-eye-slash");
            $('#show_hide_confirm_password svg').addClass("fa-eye");
        }
    });
});