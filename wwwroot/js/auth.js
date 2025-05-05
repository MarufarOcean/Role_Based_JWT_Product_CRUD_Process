const API_BASE = "https://localhost:7187/api"; // Change if needed

async function login() {
    const username = document.getElementById("username").value;
    const password = document.getElementById("password").value;

    const response = await fetch(`${API_BASE}/Auth/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ Username: username, Password: password })
    });

    if (response.ok) {
        const data = await response.json();
        localStorage.setItem("token", data.token);
       // localStorage.setItem("role", data.role);
        window.location.href = "product.html";
    } else {
        alert("Login failed");
    }
}
