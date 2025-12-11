document.addEventListener("DOMContentLoaded", function () {
    let form = document.getElementById("loginForm");
    let errorDiv = document.getElementById("loginError");

    form.addEventListener("submit", function (e) {
        e.preventDefault();

        errorDiv.classList.add("d-none");
        errorDiv.innerText = "";

        let formData = new FormData(form);

        fetch("/Login/Authenticate", {
            method: "POST",
            headers: { "X-Requested-With": "XMLHttpRequest" },
            body: formData
        })
            .then(resp => resp.json())
            .then(data => {
                if (data.success) {
                    window.location.href = data.redirectUrl;
                } else {
                    errorDiv.innerText = data.message || "Login failed.";
                    errorDiv.classList.remove("d-none");
                }
            })
            .catch(err => {
                errorDiv.innerText = "An error occurred. Please try again.";
                errorDiv.classList.remove("d-none");
            });
    });
});