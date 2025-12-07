const screenEl = document.getElementById("screen");
const inputEl = document.getElementById("command-input");
const selfDestructOverlay = document.getElementById("self-destruct-overlay");
const countdownEl = document.getElementById("countdown");

let selfDestructInProgress = false;

// ---------------- Helpers ----------------

function addLine(text, type = "system") {
    const line = document.createElement("div");
    line.classList.add("line", type);
    line.textContent = text;
    screenEl.appendChild(line);

    if (screenEl.children.length > 300) {
        screenEl.removeChild(screenEl.firstChild);
    }

    screenEl.scrollTop = screenEl.scrollHeight;
}

function printResponse(text) {
    if (!text) return;

    const lines = text.split(/\r?\n/);
    for (const raw of lines) {
        const line = raw.trimEnd();
        if (!line) continue;

        const isFlag = line.includes("FAKE{");
        const type = isFlag ? "flag" : "system";
        addLine(line, type);
    }
}

async function fetchJson(url, options = {}) {
    try {
        const res = await fetch(url, options);
        if (!res.ok) {
            throw new Error(res.statusText);
        }
        return await res.json();
    } catch (err) {
        addLine("ERROR: " + err.message, "error");
        return null;
    }
}

// ---------------- Command handling ----------------

inputEl.addEventListener("keydown", async (e) => {
    if (e.key !== "Enter") return;

    const cmd = inputEl.value.trim();
    if (!cmd) return;

    addLine("$ " + cmd, "user");
    inputEl.value = "";

    if (cmd.toLowerCase() === "selfdestruct" && !selfDestructInProgress) {
        startSelfDestruct();
    }

    const data = await fetchJson("/api/terminal/interpret", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ command: cmd })
    });

    if (!data || typeof data.text !== "string") return;

    if (data.text === "__clear__") {
        screenEl.innerHTML = "";
    } else {
        printResponse(data.text);
    }
});

// ---------------- Self destruct (fun extra) ----------------

function startSelfDestruct() {
    selfDestructInProgress = true;
    selfDestructOverlay.classList.remove("hidden");

    let count = 5;
    countdownEl.textContent = String(count);

    const timer = setInterval(() => {
        count -= 1;
        countdownEl.textContent = String(count);

        if (count <= 0) {
            clearInterval(timer);

            screenEl.innerHTML = "";
            addLine("System rebooted after self-destruct.", "system");

            setTimeout(() => {
                selfDestructOverlay.classList.add("hidden");
                selfDestructInProgress = false;
            }, 1000);
        }
    }, 1000);
}

// ---------------- Matrix-style code rain ----------------

const canvas = document.getElementById("matrix-canvas");
const ctx = canvas.getContext("2d");

let width, height, columns, drops;
const matrixChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ#$%&*@";

function initMatrix() {
    width = canvas.width = window.innerWidth;
    height = canvas.height = window.innerHeight;

    const fontSize = 16;
    columns = Math.floor(width / fontSize);
    drops = new Array(columns).fill(1);
}

function drawMatrix() {
    ctx.fillStyle = "rgba(0, 0, 0, 0.1)";
    ctx.fillRect(0, 0, width, height);

    ctx.fillStyle = "#0F0";
    ctx.font = "16px monospace";

    for (let i = 0; i < drops.length; i++) {
        const text = matrixChars.charAt(Math.floor(Math.random() * matrixChars.length));
        const x = i * 16;
        const y = drops[i] * 16;

        ctx.fillText(text, x, y);

        if (y > height && Math.random() > 0.975) drops[i] = 0;
        drops[i]++;
    }

    requestAnimationFrame(drawMatrix);
}

window.addEventListener("resize", initMatrix);

initMatrix();
requestAnimationFrame(drawMatrix);

// ---------------- Initial lines ----------------

addLine("Fake Hacker Terminal CTF v1.0", "system");
addLine("", "system");
addLine("FLAG FORMAT: FAKE{CONTENT_HERE}", "flag");
addLine("All flags follow this format with UPPERCASE content.", "system");
addLine("", "system");
addLine("HOW TO SUBMIT: submit FAKE{YOUR_FLAG_HERE}", "info");
addLine("Include the entire flag with FAKE{ and }", "system");
addLine("", "system");
addLine("Use small, familiar moves. Try 'help' or 'ls' to see where you woke up.", "system");

// Focus input on load and when clicking terminal
window.addEventListener("load", () => inputEl.focus());
document.getElementById("terminal")?.addEventListener("click", () => inputEl.focus());
