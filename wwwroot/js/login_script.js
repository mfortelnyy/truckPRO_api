document.querySelector("#loginForm").addEventListener("submit", async function (event) {
    event.preventDefault(); // Prevent the default form submission behavior

    // Show the spinner
    const spinner = document.getElementById("spinner");
    spinner.style.display = "block";

    // Get form data
    const email = document.getElementById("Email").value;
    const password = document.getElementById("Password").value;

    // Prepare the payload
    const payload = {
        email: email,
        password: password,
    };

    try {
        // Make a POST request to the Razor Page handler
        const response = await fetch("/Login?handler=Login", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "RequestVerificationToken": document.querySelector("input[name='__RequestVerificationToken']").value,
            },
            body: JSON.stringify(payload),
        });

        if (response.ok) {
            const responseText = await response.text();

            // Extract the token
            const tokenPrefix = "Token: ";
            const tokenStartIndex = responseText.indexOf(tokenPrefix);

            if (tokenStartIndex >= 0) {
                const token = responseText.substring(tokenStartIndex + tokenPrefix.length).trim();
                alert("Login successful! Token: " + token);

                // Save the token to localStorage or sessionStorage
                localStorage.setItem("authToken", token);

                // Navigate to the appropriate home page
                window.location.href = "/Home";
            } else {
                throw new Error("Token not found in the response.");
            }
        } else {
            const errorText = await response.text();
            alert("Error: " + errorText);
        }
    } catch (error) {
        console.error("Error during login request:", error);
        alert("An error occurred. Please try again.");
    } finally {
        // Hide the spinner
        spinner.style.display = "none";
    }
});