// Mientras se arrastra una foto en Carga, el navegador deja bajar la página y no subirla:
// la barra fija ocupa la franja de arriba. La rueda mueve la página en los dos sentidos.
(() => {
    const edge = 140;
    const speed = 1.2;
    let dragging = false;
    let last = 0;

    const page = () => document.scrollingElement || document.documentElement;

    document.addEventListener("dragstart", (event) => {
        const item = event.target instanceof Element ? event.target.closest(".mud-drop-item") : null;
        dragging = !!item && location.pathname.startsWith("/carga");
        last = 0;
    }, true);

    const stop = () => {
        dragging = false;
    };
    document.addEventListener("dragend", stop, true);
    document.addEventListener("drop", stop, true);

    document.addEventListener("dragover", (event) => {
        if (!dragging) return;
        const now = performance.now();
        const dt = last ? Math.min(now - last, 40) : 16;
        last = now;
        let direction = 0;
        if (event.clientY < edge) direction = -((edge - event.clientY) / edge);
        else if (event.clientY > window.innerHeight - edge) direction = (event.clientY - (window.innerHeight - edge)) / edge;
        if (direction !== 0) page().scrollTop += direction * speed * dt;
    }, true);

    document.addEventListener("wheel", (event) => {
        if (!dragging) return;
        const line = event.deltaMode === 1 ? 16 : event.deltaMode === 2 ? window.innerHeight : 1;
        page().scrollTop += event.deltaY * line;
        event.preventDefault();
    }, { capture: true, passive: false });
})();
