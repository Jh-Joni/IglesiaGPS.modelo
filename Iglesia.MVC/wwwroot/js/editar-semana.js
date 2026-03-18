document.addEventListener('DOMContentLoaded', function () {

    // 1. Buscador simple (igual que Canciones/Index)
    const searchInput = document.getElementById('searchInput');
    const cards = document.querySelectorAll('.cancion-card');

    if (searchInput) {
        searchInput.addEventListener('input', function (e) {
            const termino = e.target.value.toLowerCase();

            cards.forEach(card => {
                const titulo = card.getAttribute('data-titulo') || "";
                const autor = card.getAttribute('data-autor') || "";

                if (titulo.includes(termino) || autor.includes(termino)) {
                    card.classList.remove('d-none');
                } else {
                    card.classList.add('d-none');
                }
            });
        });
    }

    // 2. Lógica de Selección y Edición
    const listIdElement = document.getElementById('listaCancionesId');
    if (!listIdElement) return;

    const listaId = listIdElement.value;

    // Obtenemos las selecciones inyectadas desde C# (gracias a ViewBag)
    // Asegurar que sean strings para la comparación con los atributos data-id
    let cancionesSeleccionadas = [];
    if (typeof preSeleccionadasGlobal !== 'undefined' && Array.isArray(preSeleccionadasGlobal)) {
        cancionesSeleccionadas = preSeleccionadasGlobal.map(String);
    }

    const btnActualizarLista = document.getElementById('btnActualizarLista');
    const contadorUI = document.getElementById('contadorActualizadas');
    const containersIconos = document.querySelectorAll('.cancion-selector');

    // Función para verificar si podemos guardar (debe haber de 5 a 7 canciones)
    function validarBoton() {
        const count = cancionesSeleccionadas.length;
        btnActualizarLista.disabled = !(count >= 5 && count <= 7);
    }

    // Función que repinta el estado visual de cada carta basado en el Array en memoria
    function repintarSelecciones() {
        contadorUI.textContent = cancionesSeleccionadas.length;
        validarBoton();

        containersIconos.forEach(contenedor => {
            const panel = contenedor.closest('.card-body');
            const btnVerNotas = panel.querySelector('.btn-ver-notas');
            if (!btnVerNotas) return;

            const cancionId = btnVerNotas.getAttribute('data-id');
            const index = cancionesSeleccionadas.indexOf(cancionId);

            const badge = contenedor.querySelector('.seleccion-badge');
            const circuloContenedor = contenedor.querySelector('.song-icon') || contenedor.querySelector('img');

            if (index !== -1) {
                // Canción pre-seleccionada o seleccionada recién
                badge.classList.remove('d-none');
                badge.querySelector('.seleccion-numero').textContent = index + 1;

                if (circuloContenedor) {
                    circuloContenedor.style.opacity = '0.3';
                    contenedor.closest('.song-panel').style.borderColor = '#0D6845';
                    contenedor.closest('.song-panel').style.borderWidth = '2px';
                }
            } else {
                // Canción inactiva
                badge.classList.add('d-none');

                if (circuloContenedor) {
                    circuloContenedor.style.opacity = '1';
                    contenedor.closest('.song-panel').style.borderColor = 'transparent';
                    contenedor.closest('.song-panel').style.borderWidth = '0px';
                }
            }
        });
    }

    // Mapear los clics en la cuadrícula
    containersIconos.forEach(contenedor => {
        contenedor.addEventListener('click', function (e) {
            const cardBody = this.closest('.card-body');
            const btnVerNotas = cardBody.querySelector('.btn-ver-notas');
            if (!btnVerNotas) return;

            const cancionId = btnVerNotas.getAttribute('data-id');
            const index = cancionesSeleccionadas.indexOf(cancionId);

            if (index === -1) {
                // Intentar agregarla
                if (cancionesSeleccionadas.length < 7) {
                    cancionesSeleccionadas.push(cancionId);
                } else {
                    alert("Has alcanzado el límite máximo de 7 canciones por repertorio.");
                }
            } else {
                // Quitarla si ya estaba en la lista (para cambiarla por otra)
                cancionesSeleccionadas.splice(index, 1);
            }

            repintarSelecciones();
        });
    });

    btnActualizarLista.addEventListener('click', async function () {
        if (cancionesSeleccionadas.length < 5 || cancionesSeleccionadas.length > 7) return;

        // Desactivar temporalmente
        const originalText = this.innerHTML;
        this.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Guardando cambios...';
        this.disabled = true;

        try {
            // Conversión inversa a entero para el backend
            const arrayInts = cancionesSeleccionadas.map(Number);
            const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';

            const response = await fetch(`/ListaCanciones/ActualizarDesdeSeleccion/${listaId}`, {
                method: 'POST',
                credentials: 'include',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token
                },
                body: JSON.stringify(arrayInts)
            });

            if (response.ok) {
                // ¡Éxito! Retornamos al Home para que el usuario vea su lista actualizada.
                window.location.href = '/Home/Index';
            } else {
                const data = await response.json();
                alert("Error al actualizar la lista: " + (data.mensaje || "Ocurrió un error inesperado"));
                this.innerHTML = originalText;
                this.disabled = false;
            }
        } catch (error) {
            alert("Error de red o conexión al intentar actualizar.");
            console.error(error);
            this.innerHTML = originalText;
            this.disabled = false;
        }
    });

    // Escuchar el cambio de tono desde el select
    document.addEventListener('tonoCambiado', function (e) {
        const { oldId, newId } = e.detail;
        const index = cancionesSeleccionadas.indexOf(oldId);
        if (index !== -1) {
            cancionesSeleccionadas[index] = newId;
        }
    });

    // Inicializar la interfaz visualmente una vez el DOM se haya cargado
    repintarSelecciones();
});

// Funcion global para el onchange del Select de Tonos
window.cambiarTonoSelect = function (selectElement) {
    const cardBody = selectElement.closest('.card-body');
    const btnVerNotas = cardBody.querySelector('.btn-ver-notas');
    if (btnVerNotas) {
        const oldId = btnVerNotas.getAttribute('data-id');
        const newId = selectElement.value;
        btnVerNotas.setAttribute('data-id', newId);

        // Evitar que el clic en el selector se confunda con el clic principal de la tarjeta
        const event = new CustomEvent('tonoCambiado', { detail: { oldId, newId } });
        document.dispatchEvent(event);
    }
};
