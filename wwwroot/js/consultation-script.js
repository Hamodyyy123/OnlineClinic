// ==============================================
// MEDICONNECT - MAIN JAVASCRIPT FILE (updated)
// Defensive DOM checks, improved routing, and safer event handling.
// ==============================================

document.addEventListener("DOMContentLoaded", function () {
	try {
		// ===== navigation highlighting =====
		const navLinks = Array.from(document.querySelectorAll(".nav-links a"));
		const currentPath = (window.location.pathname || "/").toLowerCase();

		navLinks.forEach((link) => {
			let href = link.getAttribute("href") || "";
			// Resolve relative hrefs to absolute path and compare pathname
			try {
				href = new URL(href, location.origin).pathname.toLowerCase();
			} catch {
				// keep original if URL parsing fails
				href = href.toLowerCase();
			}

			const isHomePath =
				currentPath === "/" ||
				currentPath === "/index" ||
				currentPath === "/home" ||
				currentPath === "/home/index";

			if (href === currentPath || (isHomePath && (href === "/" || href === "/index" || href === "/home" || href === "/home/index"))) {
				link.classList.add("active");
			} else {
				link.classList.remove("active");
			}
		});

		// ===== scroll-to-top button =====
		const scrollTopBtn = document.getElementById("scrollTopBtn");
		if (scrollTopBtn) {
			window.addEventListener("scroll", function () {
				if (window.pageYOffset > 300) scrollTopBtn.classList.add("show");
				else scrollTopBtn.classList.remove("show");
			});

			scrollTopBtn.addEventListener("click", function () {
				window.scrollTo({ top: 0, behavior: "smooth" });
			});
		}

		// ===== login modal handlers =====
		const loginBtn = document.querySelector(".login-btn");
		const loginModal = document.getElementById("loginModal");
		const loginForm = document.getElementById("loginForm");

		if (loginBtn) {
			loginBtn.addEventListener("click", function (e) {
				// If there's a client-side modal available, open it.
				// Otherwise let the anchor navigate to /Login/Index as configured server-side.
				if (loginModal) {
					e.preventDefault();
					loginModal.classList.add("show");
				}
			});
		}

		if (loginForm) {
			loginForm.addEventListener("submit", function (e) {
				e.preventDefault();
				// Simple demo UX: show success message and close modal.
				// Replace with real authentication POST if needed.
				const email = document.getElementById("loginEmail")?.value || "";
				console.log("Login attempt (demo):", email);
				showSuccessMessage("Login successful! Welcome back.");
				if (loginModal) loginModal.classList.remove("show");
				loginForm.reset();
			});
		}

		// ===== booking modal handlers =====
		const bookingModal = document.getElementById("bookingModal");
		const bookingForm = document.getElementById("bookingForm");
		const heroBookBtn = document.getElementById("heroBookBtn");
		const bookButtons = Array.from(document.querySelectorAll(".btn-book"));

		// min date for appointment
		const dateInput = document.getElementById("appointmentDate");
		if (dateInput) {
			const today = new Date().toISOString().split("T")[0];
			dateInput.setAttribute("min", today);
		}

		if (heroBookBtn && bookingModal) {
			heroBookBtn.addEventListener("click", function () {
				bookingModal.classList.add("show");
			});
		}

		if (bookButtons.length > 0 && bookingModal) {
			bookButtons.forEach((button) => {
				button.addEventListener("click", function () {
					bookingModal.classList.add("show");
					// pre-select doctor if possible
					try {
						const doctorCard = this.closest(".doctor-card");
						const doctorName = doctorCard?.querySelector("h3")?.textContent?.trim();
						const doctorSelect = document.getElementById("doctorSelect");
						if (doctorName && doctorSelect) {
							for (let i = 0; i < doctorSelect.options.length; i++) {
								if (doctorSelect.options[i].value === doctorName) {
									doctorSelect.selectedIndex = i;
									break;
								}
							}
						}
					} catch (err) {
						// non-critical
						console.warn("Could not preselect doctor:", err);
					}
				});
			});
		}

		if (bookingForm) {
			bookingForm.addEventListener("submit", function (e) {
				e.preventDefault();
				const patientName = document.getElementById("patientName")?.value || "";
				const doctor = document.getElementById("doctorSelect")?.value || "the selected doctor";
				const date = document.getElementById("appointmentDate")?.value || "";
				const time = document.getElementById("appointmentTime")?.value || "";
				showSuccessMessage(`Appointment booked successfully with ${doctor} on ${date} at ${time}!`);
				if (bookingModal) bookingModal.classList.remove("show");
				bookingForm.reset();
				console.log("Booking details (demo):", { patientName, doctor, date, time });
			});
		}

		// ===== contact form =====
		const contactForm = document.getElementById("contactForm");
		if (contactForm) {
			contactForm.addEventListener("submit", function (e) {
				e.preventDefault();
				const name = document.getElementById("contactName")?.value || "";
				showSuccessMessage("Thank you for contacting us! We will get back to you soon.");
				contactForm.reset();
				console.log("Contact (demo):", name);
			});
		}

		// ===== modal close behavior =====
		const closeButtons = Array.from(document.querySelectorAll(".close-modal"));
		const modals = Array.from(document.querySelectorAll(".modal"));

		if (closeButtons.length > 0) {
			closeButtons.forEach((button) => {
				button.addEventListener("click", function () {
					const modal = this.closest(".modal");
					modal?.classList.remove("show");
				});
			});
		}

		if (modals.length > 0) {
			modals.forEach((modal) => {
				modal.addEventListener("click", function (e) {
					if (e.target === modal) modal.classList.remove("show");
				});
			});
		}

		document.addEventListener("keydown", function (e) {
			if (e.key === "Escape") modals.forEach((m) => m.classList.remove("show"));
		});

		// ===== learn more / navigation fix =====
		const learnMoreBtn = document.getElementById("learnMoreBtn");
		if (learnMoreBtn) {
			learnMoreBtn.addEventListener("click", function () {
				// navigate to the MVC route
				window.location.href = "/Home/Services";
			});
		}

		// ===== success message helper =====
		function showSuccessMessage(message) {
			const successMessage = document.getElementById("successMessage");
			const successText = document.getElementById("successText");
			if (!successMessage || !successText) return;
			successText.textContent = message;
			successMessage.classList.add("show");
			setTimeout(function () {
				successMessage.classList.remove("show");
			}, 3000);
		}

		// ===== service card hover effect =====
		const serviceCards = Array.from(document.querySelectorAll(".service-card"));
		serviceCards.forEach((card) => {
			card.addEventListener("mouseenter", function () {
				this.style.boxShadow = "0 8px 12px rgba(0,0,0,0.15)";
			});
			card.addEventListener("mouseleave", function () {
				this.style.boxShadow = "0 4px 6px rgba(0,0,0,0.1)";
			});
		});

		console.log("MediConnect website loaded successfully!");
	} catch (err) {
		// Top-level safety: log any unexpected errors so they don't break the page
		console.error("Unexpected error in consultation-script.js:", err);
	}
});