import { ModRegistrar } from "cs2/modding";
import { TooltipToggle } from "mods/TooltipToggle";

function findContainer(el: HTMLElement | null) {
    let current = el;

    while (current) {
        console.log(current.className);

        if (
            current.className?.includes("list") ||
            current.className?.includes("sidebar") ||
            current.className?.includes("content")
        ) {
            console.log("FOUND TARGET:", current);
            return current;
        }

        current = current.parentElement!;
    }

    return null;
}


function injectButton(container: HTMLElement) {
    if (container.querySelector("#my-dummy-btn")) return;

    const btn = document.createElement("button");
    btn.id = "my-dummy-btn";
    btn.textContent = "My Button";

    btn.style.margin = "8px";
    btn.style.padding = "6px 10px";
    btn.style.background = "red";
    btn.style.color = "white";

    btn.onclick = () => console.log("Clicked dummy");

    container.appendChild(btn);
}

const register: ModRegistrar = (moduleRegistry) => {
    moduleRegistry.append("Menu", TooltipToggle);


window.addEventListener("click", (e) => {
    const el = document.elementFromPoint(e.clientX, e.clientY);

    const container = findContainer(el as HTMLElement);

    if (container) {
        injectButton(container);
    }
}, true);


};

export default register;