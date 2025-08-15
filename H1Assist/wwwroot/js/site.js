class CopyToClipboard {
    static fromElementById(id) {
        const text = document.getElementById(id)?.innerText;
        if (text !== undefined && text !== null) {
            navigator.clipboard.writeText(text.trim()).catch(err => console.error(err));
        }
    }

    static fromRawHtmlContainer(id) {
        const htmlContent = document.getElementById(id)?.innerHTML;

        if (!htmlContent) return;

        const textarea = document.createElement("textarea");
        textarea.value = htmlContent;
        document.body.appendChild(textarea);
        textarea.select();
        try {
            navigator.clipboard.writeText(textarea.value.trim()).catch(err => console.error(err));
        } finally {
            document.body.removeChild(textarea);
        }
    }
}