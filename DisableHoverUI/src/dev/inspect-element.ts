const MAX_PARENT_DEPTH = 5; // 🔧 change this to control how many parents you inspect

export function inspectElement(el: HTMLElement | null, level = 0) {
    if (!el) return;

    console.group(`🔍 ELEMENT INSPECTOR (level ${level})`);

    console.log("👉 Element:", el);
    console.log("👉 Tag:", el.tagName);
    console.log("👉 Classes:", el.className);
    console.log("👉 ID:", el.id);

    console.log("👉 Computed styles:");
    const styles = window.getComputedStyle(el);
    console.log({
        display: styles.display,
        position: styles.position,
        width: styles.width,
        height: styles.height,
        background: styles.background,
        transform: styles.transform,
    });

    // 🔥 React internals (important for CS2)
    const keys = Object.keys(el);
    const reactFiberKey = keys.find(k => k.startsWith("__reactFiber"));
    const reactPropsKey = keys.find(k => k.startsWith("__reactProps"));

    if (reactFiberKey) {
        console.log("⚛️ React Fiber:", (el as any)[reactFiberKey]);
    }

    if (reactPropsKey) {
        console.log("⚛️ React Props:", (el as any)[reactPropsKey]);
    }

    console.groupEnd();
}

export function inspectWithParents(el: HTMLElement | null) {
    let current = el;
    let depth = 0;

    while (current && depth <= MAX_PARENT_DEPTH) {
        inspectElement(current, depth);

        current = current.parentElement;
        depth++;
    }
}

export function enableDeepInspector() {
    window.addEventListener(
        "click",
        (e) => {
            const el = document.elementFromPoint(
                e.clientX,
                e.clientY
            ) as HTMLElement;

            console.log("========== CLICK ==========");
            inspectWithParents(el);
        },
        true
    );

    console.log("[Inspector] Deep inspector enabled");
}



const HOVER_THROTTLE_MS = 100; // prevent spam

export function enableDeepInspectOnHover() {
    let lastTime = 0;

    window.addEventListener(
        "mousemove",
        (e) => {
            const now = performance.now();
            if (now - lastTime < HOVER_THROTTLE_MS) return;
            lastTime = now;

            const el = document.elementFromPoint(
                e.clientX,
                e.clientY
            ) as HTMLElement;

            if (!el) return;

            console.log("========== HOVER ==========");
            inspectWithParents(el);
        },
        true
    );

    console.log("[Inspector] Deep hover inspector enabled");
}