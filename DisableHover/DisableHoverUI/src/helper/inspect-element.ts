export function inspectElement(el: HTMLElement | null) {
    if (!el) return;

    console.group("🔍 ELEMENT INSPECTOR");

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



export function enableDeepInspector() {
    window.addEventListener(
        "click",
        (e) => {
            const el = document.elementFromPoint(
                e.clientX,
                e.clientY
            ) as HTMLElement;

            console.log("========== CLICK ==========");
            inspectElement(el);

            // Also inspect parent (very useful)
            inspectElement(el?.parentElement || null);
        },
        true
    );

    console.log("[Inspector] Deep inspector enabled");
}