//function to toggle password visibility
function togglePasswordVisibility() {
    const passwordField = document.getElementById("Password");
    const toggleButton = document.querySelector(".toggle-password");

    if (passwordField.type === "password") {
        passwordField.type = "text";
        toggleButton.textContent = "🔓"; //change icon to indicate password is visible
    } else {
        passwordField.type = "password";
        toggleButton.textContent = "🔒"; //change icon to indicate password is hidden
    }
}

//add event listener to the toggle button
document.querySelector(".toggle-password").addEventListener("click", togglePasswordVisibility);

//function to show spinner
function showSpinner() {
    const spinner = document.getElementById("spinner");
    spinner.style.display = "block";
}

//login form submission handler
document.querySelector("#loginForm").addEventListener("submit", async function (event) {
    event.preventDefault(); //prevent form from submitting normally

    showSpinner();

    //get form data
    const email = document.getElementById("Email").value;
    const password = document.getElementById("Password").value;

    //prepare the payload
    const payload = {
        email: email,
        password: password
    };

    try {
        //send POST request to the login endpoint
        const response = await fetch("/Login", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(payload),
        });

        //handle the response
        if (response.ok) {
            const result = await response.json();
            alert("Login successful! Token: " + result); //replace with proper success handling
            window.location.href = "/Home"; 
        } else {
            const error = await response.json();
            alert("Error: " + (error.message || "Login failed!"));
        }
    } catch (error) {
        console.error("Error during login request:", error);
        alert(error);
    } finally {
        //hide spinner
        document.getElementById("spinner").style.display = "none";
    }
});