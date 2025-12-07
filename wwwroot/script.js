const screenEl = document.getElementById("screen");
const inputEl = document.getElementById("command-input");
const selfDestructOverlay = document.getElementById("self-destruct-overlay");
const countdownEl = document.getElementById("countdown");

let selfDestructInProgress = false;
let logsPaused = false; // NEW

// ----------------------- Helpers -----------------------

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

// NEW — Pause logs for a few seconds when user runs a command
function pauseLogs(seconds = 3) {
    logsPaused = true;
    setTimeout(() => {
        logsPaused = false;
    }, seconds * 1000);
}

// ----------------------- Periodic API calls -----------------------

// Random system lines
setInterval(async () => {
    if (selfDestructInProgress || logsPaused) return;
    const data = await fetchJson("/api/terminal/random-line");
    if (data?.text) addLine(data.text, "system");
}, 1500);

// Fast password cracking lines
setInterval(async () => {
    if (selfDestructInProgress || logsPaused) return;
    const data = await fetchJson("/api/terminal/password-line");
    if (data?.text) addLine(data.text, "password");
}, 120);

// IP scan lines
setInterval(async () => {
    if (selfDestructInProgress || logsPaused) return;
    const data = await fetchJson("/api/terminal/ip-line");
    if (data?.text) addLine(data.text, "ip");
}, 400);

// ----------------------- Command Handling -----------------------

inputEl.addEventListener("keydown", async (e) => {
    if (e.key === "Enter") {
        const cmd = inputEl.value.trim();
        if (!cmd) return;

        addLine("$ " + cmd, "user");

        pauseLogs(3); // NEW — freeze logs so text stays readable

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

        if (!data || !data.text) return;

        if (data.text === "__clear__") {
            screenEl.innerHTML = "";
        } else {
            addLine(data.text, "system");
        }
    }
});

// ----------------------- Self Destruct -----------------------

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

// ----------------------- Matrix Code Rain -----------------------

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

// ----------------------- Initial Startup -----------------------

addLine("Fake Hacker Terminal v1.0", "system");
addLine("Type 'help' and press Enter to see available fake commands.", "system");
