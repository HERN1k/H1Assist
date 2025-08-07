const buttonsActionMap = new Map();

function init() {
    for (const [key, value] of buttonsActionMap) {
        document.getElementById(key)?.addEventListener("click", value);
    } 
}

function cleanup() {
    for (const [key, value] of buttonsActionMap) {
        document.getElementById(key)?.removeEventListener("click", value);
    }
} 

document.addEventListener("DOMContentLoaded", init);
window.addEventListener("beforeunload", cleanup);