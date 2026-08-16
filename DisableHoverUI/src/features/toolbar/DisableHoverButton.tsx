import React from "react";
import { bindValue, trigger } from "cs2/api";

const GROUP = "DisableHover";

const tooltipsDisabled = bindValue<boolean>(GROUP, "GetDisableUIToolTips");
const highlightsDisabled = bindValue<boolean>(GROUP, "GetDisableBlueHighlights");
const backendReady = bindValue<boolean>(GROUP, "BackendReady");

const baseButtonStyle: React.CSSProperties = {
    minWidth: "42rem",
    height: "42rem",
    marginLeft: "6rem",
    padding: "0 10rem",
    borderRadius: "6rem",
    color: "white",
    fontSize: "13rem",
    fontWeight: 600,
    pointerEvents: "auto",
};

export const DisableHoverButton = () => {
    const [tooltips, setTooltips] = React.useState(tooltipsDisabled.value ?? false);
    const [highlights, setHighlights] = React.useState(highlightsDisabled.value ?? false);
    const [ready, setReady] = React.useState(backendReady.value ?? false);

    React.useEffect(() => {
        tooltipsDisabled.subscribe(setTooltips);
    }, []);

    React.useEffect(() => {
        highlightsDisabled.subscribe(setHighlights);
    }, []);

    React.useEffect(() => {
        backendReady.subscribe(setReady);
    }, []);

    const toggleHighlights = () => {
        if (!ready) {
            console.warn("[DisableHover] C# backend is not ready; DH click ignored");
            return;
        }

        trigger(GROUP, "ToggleBlueHighlights");
    };

    const toggleTooltips = () => {
        if (!ready) {
            console.warn("[DisableHover] C# backend is not ready; DT click ignored");
            return;
        }

        trigger(GROUP, "ToggleTooltips");
    };

    const styleFor = (active: boolean): React.CSSProperties => ({
        ...baseButtonStyle,
        border: ready
            ? "1rem solid rgba(255,255,255,0.22)"
            : "1rem solid rgba(255,100,100,0.55)",
        background: active
            ? "rgba(30, 120, 210, 0.88)"
            : ready
                ? "rgba(30, 30, 30, 0.72)"
                : "rgba(90, 30, 30, 0.72)",
        opacity: ready ? 1 : 0.65,
        cursor: ready ? "pointer" : "not-allowed",
    });

    return (
        <>
            <button
                type="button"
                onClick={toggleHighlights}
                title={ready
                    ? (highlights ? "Enable blue hover highlights" : "Disable blue hover highlights")
                    : "DisableHover backend is not ready"}
                style={styleFor(highlights)}
            >
                DH
            </button>

            <button
                type="button"
                onClick={toggleTooltips}
                title={ready
                    ? (tooltips ? "Enable tooltips" : "Disable tooltips")
                    : "DisableHover backend is not ready"}
                style={styleFor(tooltips)}
            >
                DT
            </button>
        </>
    );
};
