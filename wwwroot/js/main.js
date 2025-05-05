const API_BASE = "https://localhost:7187/api";

const token = localStorage.getItem("token");
const role = localStorage.getItem("role");

if (!token) {
    window.location.href = "login.html";
}

document.addEventListener("DOMContentLoaded", loadProducts);

async function loadProducts() {
    const res = await fetch(`${API_BASE}/Products`, {
        headers: { Authorization: `Bearer ${token}` }
    });

    if (res.ok) {
        const products = await res.json();
        const list = document.getElementById("productList");
        list.innerHTML = "";
        products.forEach(p => {
            const item = document.createElement("div");
            item.innerText = `${p.name} - $${p.price}`;
            list.appendChild(item);
        });

        if (role === "Admin") {
            document.getElementById("adminSection").style.display = "block";
        }
    } else {
        alert("Failed to load products");
    }
}

async function addProduct() {
    const name = document.getElementById("name").value;
    const price = parseFloat(document.getElementById("price").value);

    const res = await fetch(`${API_BASE}/Products`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`
        },
        body: JSON.stringify({ name, price })
    });

    if (res.ok) {
        loadProducts();
    } else {
        alert("Failed to add product");
    }
}

function logout() {
    localStorage.clear();
    window.location.href = "login.html";
}
