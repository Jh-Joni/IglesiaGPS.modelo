// ===== Scripts para la vista Home/Index =====

// ===== MODAL ANUNCIOS =====
function abrirModal() {
    var modal = document.getElementById('modalAnuncio');
    if (modal) {
        modal.classList.add('activo');
        document.body.style.overflow = 'hidden';
    }
}

function cerrarModal() {
    var modal = document.getElementById('modalAnuncio');
    if (modal) {
        modal.classList.remove('activo');
        document.body.style.overflow = '';
    }
}

document.addEventListener('click', function (e) {
    var modal = document.getElementById('modalAnuncio');
    if (modal && e.target === modal) cerrarModal();
});

document.addEventListener('keydown', function (e) {
    if (e.key === 'Escape') cerrarModal();
});

// ===== COLAPSAR / EXPANDIR SEMANAS =====
window.toggleSemana = function (index) {
    var wrapper = document.getElementById('canciones-' + index);
    var icon = document.getElementById('toggle-icon-' + index);

    if (!wrapper || !icon) return;

    if (wrapper.classList.contains('expandido')) {
        wrapper.classList.remove('expandido');
        wrapper.classList.add('colapsado');
        icon.textContent = "▶";
        icon.style.transform = "rotate(0deg)";
    } else {
        // Cerrar todos los demás (Opcional, estilo acordeón)
        document.querySelectorAll('.canciones-wrapper').forEach(w => {
            w.classList.remove('expandido');
            w.classList.add('colapsado');
        });
        document.querySelectorAll('.toggle-icon').forEach(ic => {
            ic.textContent = "▶";
            ic.style.transform = "rotate(0deg)";
        });

        wrapper.classList.remove('colapsado');
        wrapper.classList.add('expandido');
        icon.textContent = "▼";
    }
};

// ===== ANIMACIONES DE ENTRADA =====
document.addEventListener('DOMContentLoaded', function () {
    var observer = new IntersectionObserver(function (entries) {
        entries.forEach(function (entry) {
            if (entry.isIntersecting) {
                entry.target.classList.add('visible');
            }
        });
    }, { threshold: 0.1 });

    document.querySelectorAll('.historial-lista').forEach(function (el) {
        observer.observe(el);
    });
});

// ===== ALTERNAR DOMINGO VS MIÉRCOLES =====
window.switchTab = function (index, tabId) {
    // Referencias a Paneles de Canciones
    var gridDom = document.getElementById('grid-domingo-' + index);
    var gridMie = document.getElementById('grid-miercoles-' + index);
    
    // Botones de tabs en el header
    var btnMie = document.getElementById('btn-miercoles-' + index);
    var btnDom = document.getElementById('btn-domingo-' + index);

    if (tabId === 'domingo') {
        // MOSTRAR DOMINGO
        if (gridDom) gridDom.classList.remove('d-none');
        if (gridMie) gridMie.classList.add('d-none');
        if (btnMie) btnMie.classList.remove('d-none');
        if (btnDom) btnDom.classList.add('d-none');
    }
    else if (tabId === 'miercoles') {
        // MOSTRAR MIÉRCOLES
        if (gridMie) gridMie.classList.remove('d-none');
        if (gridDom) gridDom.classList.add('d-none');
        if (btnMie) btnMie.classList.add('d-none');
        if (btnDom) btnDom.classList.remove('d-none');
    }
    
    // Si estaba colapsado, expandirlo al cambiar de tab
    var wrapper = document.getElementById('canciones-' + index);
    if (wrapper && wrapper.classList.contains('colapsado')) {
        toggleSemana(index);
    }
};

// ===== REPRODUCTOR FLOTANTE YOUTUBE ======
window.abrirYoutubeFlotante = function(url) {
    if (!url) return;
    
    // Si ya existe borralo
    var existingModal = document.getElementById('youtube-modal-flotante');
    if (existingModal) existingModal.remove();

    // Parsear el ID de Youtube de la URL (ej: watch?v=ABC o youtu.be/ABC)
    var videoId = "";
    var match = url.match(/(?:youtu\.be\/|youtube\.com\/(?:embed\/|v\/|watch\?v=|watch\?.+&v=))([\w-]{11})/);
    if (match && match[1]) videoId = match[1];

    var iframeSrc = videoId ? "https://www.youtube.com/embed/" + videoId + "?autoplay=1" : url;

    var html = `
    <div id="youtube-modal-flotante" style="position: fixed; bottom: 20px; right: 20px; width: 350px; background: #0f172a; border-radius: 12px; box-shadow: 0 10px 30px rgba(0,0,0,0.5); z-index: 2000; overflow: hidden; border: 1px solid rgba(0,242,254,0.3); animation: slideUpYt 0.3s ease-out;">
        <div style="background: rgba(15,23,42,0.9); padding: 8px 15px; display: flex; justify-content: space-between; align-items: center; border-bottom: 1px solid rgba(255,255,255,0.1);">
            <span style="color: #00f2fe; font-size: 13px; font-weight: bold;"><i class="bi bi-youtube text-danger"></i> Reproductor</span>
            <button onclick="document.getElementById('youtube-modal-flotante').remove()" style="background: none; border: none; color: white; font-size: 16px; cursor: pointer;">✕</button>
        </div>
        <div style="position: relative; padding-bottom: 56.25%; height: 0; overflow: hidden; background: #000;">
            <iframe src="${iframeSrc}" style="position: absolute; top:0; left: 0; width: 100%; height: 100%; border:0;" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>
        </div>
    </div>
    <style>
        @keyframes slideUpYt { from { transform: translateY(400px); opacity: 0; } to { transform: translateY(0); opacity: 1; } }
        /* Responsivo en móviles */
        @media (max-width: 600px) {
            #youtube-modal-flotante {
                width: 100% !important;
                bottom: 0 !important;
                right: 0 !important;
                border-radius: 20px 20px 0 0 !important;
                border-bottom: none !important;
                border-left: none !important;
                border-right: none !important;
            }
        }
    </style>
    `;

    document.body.insertAdjacentHTML('beforeend', html);
}
