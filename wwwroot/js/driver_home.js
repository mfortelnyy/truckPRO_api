document.addEventListener("DOMContentLoaded", function () {
    const token = localStorage.getItem("token");
    const activeLogsList = document.getElementById("activeLogs");

    // Fetch Active Logs
    fetch("https://truckcheck.org/getActiveLogs", {
        headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
        },
    })
        .then((response) => {
            if (!response.ok) throw new Error("Failed to fetch active logs");
            return response.json();
        })
        .then((logs) => {
            logs.forEach((log) => {
                const logItem = document.createElement("li");
                logItem.textContent = `Log Type: ${log.type}, Start Time: ${log.startTime}`;
                activeLogsList.appendChild(logItem);
            });
        })
        .catch((error) => {
            console.error(error);
        });
});