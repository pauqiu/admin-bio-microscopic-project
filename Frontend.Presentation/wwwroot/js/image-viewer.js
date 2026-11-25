// Visor de fotos: ajusta el tamaño según el aumento y permite mover la foto arrastrándola.

export function init(container) {
    let drag = null;

    container.addEventListener('pointerdown', (e) => {
        if (e.button !== 0) return;
        drag = { x: e.clientX, y: e.clientY, left: container.scrollLeft, top: container.scrollTop };
        container.setPointerCapture(e.pointerId);
        container.style.cursor = 'grabbing';
    });

    container.addEventListener('pointermove', (e) => {
        if (!drag) return;
        container.scrollLeft = drag.left - (e.clientX - drag.x);
        container.scrollTop = drag.top - (e.clientY - drag.y);
    });

    const stop = (e) => {
        if (!drag) return;
        drag = null;
        if (container.hasPointerCapture(e.pointerId)) container.releasePointerCapture(e.pointerId);
        container.style.cursor = 'grab';
    };
    container.addEventListener('pointerup', stop);
    container.addEventListener('pointercancel', stop);
    container.style.cursor = 'grab';
}

// scale = veces que se agranda respecto a la foto completa.
// resetCenter = true al cambiar de foto; si no, se mantiene el punto que se estaba mirando.
export function zoom(container, img, scale, resetCenter) {
    const apply = () => {
        const width = container.clientWidth;
        const height = container.clientHeight;
        if (!img.naturalWidth || !img.naturalHeight || !width || !height) return;

        const centerX = resetCenter || container.scrollWidth <= width
            ? 0.5 : (container.scrollLeft + width / 2) / container.scrollWidth;
        const centerY = resetCenter || container.scrollHeight <= height
            ? 0.5 : (container.scrollTop + height / 2) / container.scrollHeight;

        // Se mide el área completa (con barras de desplazamiento) para que cada aumento
        // tenga siempre el mismo tamaño, haya o no barras en ese momento.
        const fit = Math.min(container.offsetWidth / img.naturalWidth, container.offsetHeight / img.naturalHeight);
        img.style.width = `${img.naturalWidth * fit * scale}px`;
        img.style.height = `${img.naturalHeight * fit * scale}px`;

        container.scrollLeft = centerX * container.scrollWidth - width / 2;
        container.scrollTop = centerY * container.scrollHeight - height / 2;
    };

    if (img.complete && img.naturalWidth) apply();
    else img.addEventListener('load', apply, { once: true });
}
