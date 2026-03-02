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
