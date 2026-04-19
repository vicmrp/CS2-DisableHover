import { trigger, bindValue } from "cs2/api";

const GROUP = "DisableHover";

export function testCommunication() {
    console.log("=== TESTING UI ↔ C# (LOOP) ===");

    try {
        const binding = bindValue<boolean>(GROUP, "GetTooltipsEnabled");

        binding.subscribe((value) => {
            console.log("[UI] Received value from C#:", value);
        });

        console.log("[UI] Subscription registered");

        let state = false;

        setInterval(() => {
            state = !state;

            console.log("[UI] Sending:", state);
            trigger(GROUP, "SetTooltipsEnabled", state);

        }, 10000);

    } catch (e) {
        console.error("[UI] Communication FAILED:", e);
    }
}