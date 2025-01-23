document.querySelector("#loginForm").addEventListener("submit", async function (event) {
    event.preventDefault(); // prevent form from submitting normally

    function showSpinner() {
        const spinner = document.getElementById("spinner");
        spinner.style.display = "block";
    }
    
    //get form data
    const email = document.getElementById("Email").value;
    const password = document.getElementById("Password").value;

    //prep the payload
    const payload = {
        email: email,
        password: password
    };

    try {
        //POST request to the Login endpoint
        const response = await fetch("/Login", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(payload),
        });

        //handle the resp
        if (response.ok) {
            const result = await response.json();
            alert("Login successful! Token: " + result); // needs appropriate success handling
            window.location.href = "/Home"; 
        } else {
            const error = await response.json();
            alert("Error: " + (error.message || "Login failed!"));
        }
    } catch (error) {
        console.error("Error during login request:", error);
        alert("An error occurred. Please try again.");
    } finally {
        // hide spinner
        document.getElementById("spinner").style.display = "none";
    }
});