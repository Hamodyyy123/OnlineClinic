// wwwroot/js/Login.js
// Enhanced Login JS with field animations, spinner, safe JSON parsing,
// and simple mouse parallax for background layers. Also makes form bigger UX.

document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("loginForm");
    const errorDiv = document.getElementById("loginError");
    const submitBtn = document.getElementById("loginBtn");
    const btnText = submitBtn ? submitBtn.querySelector(".btn-text") : null;
    const fields = Array.from(document.querySelectorAll(".form-field"));
    const root = document.documentElement;

    if (!form) return;

    /* ---------------------------
       Background parallax (mouse)
       --------------------------- */
    (function setupParallax() {
        let mx = 0, my = 0;
        let tx = 0, ty = 0;
        const ease = 0.08;
        function onMove(e) {
            const cx = window.innerWidth / 2;
            const cy = window.innerHeight / 2;
            mx = (e.clientX - cx) / cx; // -1 .. 1
            my = (e.clientY - cy) / cy; // -1 .. 1
        }
        function loop() {
            tx += (mx * 18 - tx) * ease;
            ty += (my * 12 - ty) * ease;
            // convert to px string
            root.style.setProperty("--mx", `${tx}px`);
            root.style.setProperty("--my", `${ty}px`);
            requestAnimationFrame(loop);
        }
        document.addEventListener("mousemove", onMove);
        // small touch fallback
        document.addEventListener("touchmove", function (e) {
            const t = e.touches && e.touches[0];
            if (t) onMove(t);
        }, { passive: true });
        loop();
    })();

    /* ---------------------------
       Field UI behaviors
       --------------------------- */
    fields.forEach(container => {
        const input = container.querySelector("input");
        const underline = container.querySelector(".focus-underline");

        // mark filled if has value initially
        if (input.value && input.value.trim() !== "") container.classList.add("filled");

        input.addEventListener("focus", () => container.classList.add("focused"));
        input.addEventListener("blur", () => {
            container.classList.remove("focused");
            if (input.value && input.value.trim() !== "") container.classList.add("filled");
            else container.classList.remove("filled");
            input.value = input.value.trim();
        });
        input.addEventListener("input", () => {
            if (input.value && input.value.trim() !== "") container.classList.add("filled");
            else container.classList.remove("filled");
        });
    });

    /* ---------------------------
       Helpers
       --------------------------- */
    function showError(msg) {
        if (!errorDiv) return;
        errorDiv.textContent = msg;
        errorDiv.classList.remove("d-none");
        errorDiv.classList.add("alert-login");
        errorDiv.setAttribute("tabindex", "-1");
        errorDiv.focus({ preventScroll: true });
    }

    function clearError() {
        if (!errorDiv) return;
        errorDiv.classList.add("d-none");
        errorDiv.textContent = "";
    }

    function setSubmitting(is) {
        if (!submitBtn) return;
        if (is) {
            submitBtn.disabled = true;
            if (!submitBtn.querySelector(".spinner")) {
                const s = document.createElement("span");
                s.className = "spinner";
                submitBtn.insertBefore(s, btnText);
            }
            if (btnText) btnText.textContent = "Signing in...";
        } else {
            submitBtn.disabled = false;
            const s = submitBtn.querySelector(".spinner");
            if (s) s.remove();
            if (btnText) btnText.textContent = "Sign in";
        }
    }

    async function parseJsonResponse(resp) {
        const text = await resp.text();
        try { return JSON.parse(text); }
        catch (e) { return { success: false, message: "Invalid server response" }; }
    }

    /* ---------------------------
       Submit handler
       --------------------------- */
    form.addEventListener("submit", function (e) {
        e.preventDefault();
        clearError();

        const username = form.username?.value?.trim();
        const password = form.password?.value || "";

        if (!username || !password) {
            showError("Please enter both username and password.");
            return;
        }

        setSubmitting(true);

        const formData = new FormData(form);
        fetch("/Login/Authenticate", {
            method: "POST",
            headers: { "X-Requested-With": "XMLHttpRequest" },
            body: formData
        })
            .then(async resp => {
                const data = await parseJsonResponse(resp);
                return { ok: resp.ok, data };
            })
            .then(({ ok, data }) => {
                if (ok && data && data.success) {
                    window.location.href = data.redirectUrl || "/";
                    return;
                }
                showError(data?.message || "Login failed. Check credentials.");
                setSubmitting(false);
            })
            .catch(err => {
                console.error("Login error", err);
                showError("An error occurred. Please try again.");
                setSubmitting(false);
            });
    });

    // trim on blur, accessible enter handling done by form naturally
    form.querySelectorAll("input").forEach(inp => inp.addEventListener("blur", () => inp.value = inp.value.trim()));
});