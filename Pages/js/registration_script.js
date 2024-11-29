document.querySelector("form").addEventListener("submit", function (event) {

    //TODO: Add check for empty input fields

    var password = document.getElementById("Password").value;
    var confirmPassword = document.getElementById("ConfirmPassword").value;

    if (password !== confirmPassword) {
        //prevents submission if passwords do not match
        event.preventDefault();
        alert("Passwords do not match. Please try again.");
    } else {
        //show spinner before submitting the form
        document.getElementById("spinner").style.display = "block";

        //add delay to simulate submission
        setTimeout(function() {
            event.target.submit();
        }, 2000); 

        event.preventDefault();
    }
});
