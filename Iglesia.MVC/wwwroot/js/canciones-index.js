// ===== Scripts para la vista Canciones/Index =====

document.addEventListener('DOMContentLoaded', function () {
    // ---- 1. Lógica de búsqueda ----
    const searchInput = document.getElementById('searchInput');
    const canciones = document.querySelectorAll('.cancion-card');

    if (searchInput) {
        searchInput.addEventListener('input', function (e) {
            const searchTerm = e.target.value.toLowerCase().trim();

            canciones.forEach(card => {
                const autor = card.getAttribute('data-autor') || '';
                const titulo = card.getAttribute('data-titulo') || '';
                if (autor.includes(searchTerm) || titulo.includes(searchTerm)) {
                    card.style.display = 'block';
                    card.style.opacity = '0';
                    setTimeout(() => card.style.opacity = '1', 50);
                } else {
                    card.style.display = 'none';
                }
            });
        });
    }

    // ---- 2. Lógica para el Modal de Notas ----
    const btnVerNotas = document.querySelectorAll('.btn-ver-notas');
    const modalElement = document.getElementById('notasModal');

    if (btnVerNotas.length > 0 && modalElement) {
        const notasModal = new bootstrap.Modal(modalElement);
        const modalTitle = document.getElementById('notasModalLabel');
        const modalBody = document.getElementById('notasModalBody');

        btnVerNotas.forEach(btn => {
            btn.addEventListener('click', async function (e) {
                e.stopPropagation();
                const cancionId = this.getAttribute('data-id');
                const titulo = this.getAttribute('data-titulo');

                modalTitle.textContent = 'Notas Musicales: ' + titulo;
                modalBody.innerHTML = `
                    <div class="text-center py-5">
                        <div class="spinner-border text-primary" role="status">
                            <span class="visually-hidden">Cargando...</span>
                        </div>
                        <p class="mt-2 text-muted">Obteniendo notas...</p>
                    </div>
                `;

                notasModal.show();

                try {
                    const response = await fetch(`/Canciones/GetNotas?id=${cancionId}`);
                    if (!response.ok) throw new Error('No se pudieron obtener los datos.');
                    const notas = await response.json();

                    if (!notas || notas.length === 0) {
                        modalBody.innerHTML = `
                            <div class="alert alert-info d-flex align-items-center" role="alert">
                                <i class="bi bi-info-circle-fill me-2 fs-4"></i>
                                <div>No hay notas musicales disponibles para esta canción.</div>
                            </div>`;
                        return;
                    }

                    let notasHtml = '<div class="row g-3">';
                    notas.forEach((nota) => {
                        const instrumento = nota.instrumento || 'General';
                        notasHtml += `
                            <div class="col-12">
                                <div class="card border-0 shadow-sm">
                                    <div class="card-header bg-white border-bottom-0 pt-3 pb-0">
                                        <h6 class="text-primary fw-bold mb-0">
                                            <i class="bi bi-music-note-list me-2"></i>${instrumento}
                                        </h6>
                                    </div>
                                    <div class="card-body">
                                        <pre class="bg-dark text-light p-3 rounded mb-0" style="white-space: pre-wrap; font-family: 'Courier New', Courier, monospace;">${nota.contenido}</pre>
                                        <small class="text-muted mt-2 d-block text-end">Actualizado: ${new Date(nota.ultimaEdicion).toLocaleDateString()}</small>
                                    </div>
                                </div>
                            </div>
                        `;
                    });
                    notasHtml += '</div>';
                    modalBody.innerHTML = notasHtml;

                } catch (error) {
                    modalBody.innerHTML = `
                        <div class="alert alert-danger d-flex align-items-center" role="alert">
                            <i class="bi bi-exclamation-triangle-fill me-2 fs-4"></i>
                            <div>Error al cargar las notas: ${error.message}</div>
                        </div>`;
                }
            });
        });
    }

    // ---- 3. Lógica de Selección de Canciones (Director/Desarrollador) ----
    const panelSeleccion = document.getElementById('panelSeleccion');
    if (panelSeleccion) {
        let cancionesSeleccionadas = [];
        const btnGuardarLista = document.getElementById('btnGuardarLista');
        const contadorUI = document.getElementById('contadorSeleccionadas');
        const containersIconos = document.querySelectorAll('.cancion-selector');

        // Función para actualizar UI del panel flotante
        function actualizarPanel() {
            const count = cancionesSeleccionadas.length;
            contadorUI.textContent = count;

            if (count > 0) {
                panelSeleccion.classList.remove('d-none');
                panelSeleccion.classList.add('d-flex');
            } else {
                panelSeleccion.classList.add('d-none');
                panelSeleccion.classList.remove('d-flex');
            }

            // Habilitar botón solo si hay entre 5 y 7 canciones
            btnGuardarLista.disabled = !(count >= 5 && count <= 7);
        }

        // Función para actualizar visualmente todas las etiquetas de orden
        function refrescarNumeros() {
            containersIconos.forEach(contenedor => {
                const btnVerNotas = contenedor.closest('.card-body').querySelector('.btn-ver-notas');
                const cancionId = btnVerNotas.getAttribute('data-id');
                const index = cancionesSeleccionadas.indexOf(cancionId);

                const badge = contenedor.querySelector('.seleccion-badge');
                const circuloContenedor = contenedor.querySelector('.song-icon') || contenedor.querySelector('img');

                if (index !== -1) {
                    // Está seleccionada
                    badge.classList.remove('d-none');
                    badge.querySelector('.seleccion-numero').textContent = index + 1;

                    if (circuloContenedor) {
                        circuloContenedor.style.opacity = '0.3';
                        contenedor.closest('.song-panel').style.borderColor = '#0D6845';
                        contenedor.closest('.song-panel').style.borderWidth = '2px';
                    }
                } else {
                    // No está seleccionada
                    badge.classList.add('d-none');
                    if (circuloContenedor) {
                        circuloContenedor.style.opacity = '1';
                        contenedor.closest('.song-panel').style.borderColor = 'transparent';
                        contenedor.closest('.song-panel').style.borderWidth = '0px';
                    }
                }
            });
        }

        // Asignar evento click a los íconos/imágenes
        containersIconos.forEach(contenedor => {
            contenedor.addEventListener('click', function (e) {
                const cardBody = this.closest('.card-body');
                const cancionId = cardBody.querySelector('.btn-ver-notas').getAttribute('data-id');

                const index = cancionesSeleccionadas.indexOf(cancionId);

                if (index === -1) {
                    // Si no está y límite no es 7, se añade
                    if (cancionesSeleccionadas.length < 7) {
                        cancionesSeleccionadas.push(cancionId);
                    } else {
                        alert("Has alcanzado el límite máximo de 7 canciones por lista.");
                    }
                } else {
                    // Si ya está, se quita
                    cancionesSeleccionadas.splice(index, 1);
                }

                refrescarNumeros();
                actualizarPanel();
            });
        });

        // Evento Enviar Lista
        btnGuardarLista.addEventListener('click', async function () {
            if (cancionesSeleccionadas.length < 5 || cancionesSeleccionadas.length > 7) return;

            const token = document.querySelector('#antiForgeryForm input[name="__RequestVerificationToken"]')?.value || '';

            // Desabilitar botón y mostrar carga
            const originalText = this.innerHTML;
            this.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Guardando...';
            this.disabled = true;

            try {
                const response = await fetch('/ListaCanciones/CrearDesdeSeleccion', {
                    method: 'POST',
                    credentials: 'include', // asegura que viaje la cookie de sesión
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': token
                    },
                    body: JSON.stringify(cancionesSeleccionadas)
                });

                if (response.ok) {
                    window.location.href = '/ListaCanciones/Index'; // Redirigir al éxito
                } else {
                    const data = await response.json();
                    alert("Error al guardar la lista: " + (data.mensaje || "Ocurrió un error inesperado"));
                    this.innerHTML = originalText;
                    this.disabled = false;
                }
            } catch (error) {
                alert("Error de conexión al guardar la lista.");
                this.innerHTML = originalText;
                this.disabled = false;
            }
        });
    }
});
