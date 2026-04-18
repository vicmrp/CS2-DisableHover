export function logElementTree(el: HTMLElement | null) {
    if (!el) return;

    let current: HTMLElement | null = el;
    let depth = 0;

    console.group("🧭 Clicked element trace");

    while (current && depth < 15) {
        const info = {
            tag: current.tagName,
            id: current.id,
            class: current.className,
        };

        console.log(`[${depth}]`, info, current);

        current = current.parentElement;
        depth++;
    }

    console.groupEnd();
}