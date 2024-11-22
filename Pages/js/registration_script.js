document.querySelector("form").addEventListener("submit", function (event) {
    var password = document.getElementById("Password").value;
    var confirmPassword = document.getElementById("ConfirmPassword").value;

    if (password !== confirmPassword) {
        event.preventDefault();
        alert("Passwords do not match. Please try again.");
    } else {
        document.getElementById("spinner").style.display = "block";
        setTimeout(function() {
            event.target.submit();
        }, 2000);

        event.preventDefault();
    }
});
