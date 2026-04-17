export const HelloWorldComponent = () => {
    const style = document.createElement("style");

    style.innerHTML = `
        /* Main tooltip system */
        [class*="balloon"] {
            display: none !important;
        }

        /* Secondary tooltip system */
        [class*="tooltip"] {
            display: none !important;
        }

        /* Info overlays / stat hover panels */
        [class*="group-container"] {
            display: none !important;
        }

        [class*="row-item"] {
            display: none !important;
        }
    `;

    document.head.appendChild(style);

    console.log("All tooltip systems disabled");

    return null;
};