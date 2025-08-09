class CopyToClipboard {
    static fromElementById(id) {
        const text = document.getElementById(id)?.innerText;
        if (text !== undefined && text !== null) {
            navigator.clipboard.writeText(text).catch(err => console.error(err));
        }
    }
}