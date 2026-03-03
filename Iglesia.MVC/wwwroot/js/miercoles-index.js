document.addEventListener('DOMContentLoaded', function () {
    const contenedorCanciones = document.getElementById('cancionesContainer');
    const contadorBadge = document.getElementById('contadorSeleccion');
    const btnGuardar = document.getElementById('btnGuardarRepertorio');
    const panelFijo = document.querySelector('.panel-flotante');

    const MAX_CANCIONES = 5;
    const MIN_CANCIONES = 2;

    function getSelectedCount() {
        return document.querySelectorAll('.cancion-card.selected').length;
    }

    function updateTotal() {
        const total = getSelectedCount();
        contadorBadge.textContent = total;

        // Efecto visual al actualizar
        contadorBadge.style.transform = 'scale(1.2)';
        setTimeout(() => contadorBadge.style.transform = 'scale(1)', 200);

        if (total >= MIN_CANCIONES && total <= MAX_CANCIONES) {
            btnGuardar.disabled = false;
            panelFijo.style.boxShadow = '0 0 20px rgba(234, 179, 8, 0.6)';
            panelFijo.style.borderColor = '#eab308';
        } else {
            btnGuardar.disabled = true;
            panelFijo.style.boxShadow = 'none';
            panelFijo.style.borderColor = 'rgba(234, 179, 8, 0.3)';
        }
    }

    // Inicializar estado botón
    updateTotal();

    // Toggle de Selección de Canciones
    window.toggleSeleccion = function (element) {
        const currentTotal = getSelectedCount();
        const card = element.closest('.cancion-card');
        const badge = card.querySelector('.seleccion-badge');

        // Si ya está seleccionado, permitir deseleccionar
        if (card.classList.contains('selected')) {
            card.classList.remove('selected', 'border-warning', 'shadow-lg');
            card.style.background = 'rgba(30, 41, 59, 0.6)';
            card.style.borderColor = 'rgba(255, 255, 255, 0.05)';
            badge.classList.replace('d-flex', 'd-none');
        }
        // Si no está seleccionado, validar límite
        else {
            if (currentTotal >= MAX_CANCIONES) {
                // Flash rojo para indicar límite
                contadorBadge.style.background = '#ef4444';
                contadorBadge.style.color = '#fff';
                setTimeout(() => {
                    contadorBadge.style.background = '#eab308';
                    contadorBadge.style.color = '#000';
                }, 300);
                return;
            }

            card.classList.add('selected', 'border-warning', 'shadow-lg');
            card.style.background = 'rgba(234, 179, 8, 0.1)';
            card.style.borderColor = '#eab308';
            badge.classList.replace('d-none', 'd-flex');
        }

        updateTotal();
    };

    // Guardar vía AJAX
    btnGuardar.addEventListener('click', async function () {
        const total = getSelectedCount();
        if (total < MIN_CANCIONES || total > MAX_CANCIONES) {
            alert(`Deben ser entre ${MIN_CANCIONES} y ${MAX_CANCIONES} canciones para el miércoles.`);
            return;
        }

        const idPadre = document.getElementById('idListaPadre').value;
        const idMiercolesExistente = document.getElementById('idListaMiercoles').value;
        const selectedCards = document.querySelectorAll('.cancion-card.selected');

        const cancionesSeleccionadas = Array.from(selectedCards).map(card => parseInt(card.dataset.id));

        const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

        // Efectos del botón
        this.disabled = true;
        document.getElementById('spinnerGuardar').style.display = 'inline-block';
        document.getElementById('spinnerGuardar').classList.add('spinner-border', 'spinner-border-sm');
        this.innerHTML = 'Guardando...';

        try {
            const urlParams = new URLSearchParams({
                idPadre: idPadre,
                idMiercolesExistente: idMiercolesExistente
            });

            const response = await fetch(`/ListaCanciones/ActualizarMiercoles?${urlParams.toString()}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token
                },
                body: JSON.stringify(cancionesSeleccionadas)
            });

            if (response.ok) {
                const result = await response.json();

                // Efecto de Éxito Hacker (Verde)
                panelFijo.style.background = 'rgba(21, 128, 61, 0.95)';
                panelFijo.style.borderColor = '#22c55e';
                panelFijo.style.boxShadow = '0 0 30px rgba(34, 197, 94, 0.8)';
                this.classList.replace('btn-warning', 'btn-success');
                this.innerHTML = '<i class="bi bi-check-circle-fill me-2"></i>¡Actualizado!';
                this.style.background = '#22c55e';

                setTimeout(() => {
                    window.location.href = '/Home/Index';
                }, 1200);
            } else {
                const err = await response.json();
                throw new Error(err.mensaje || 'Error del servidor');
            }
        } catch (error) {
            alert(error.message);

            // Restablecer botón
            this.disabled = false;
            this.innerHTML = '<i class="bi bi-arrow-repeat me-1" id="spinnerGuardar" style="display:none;"></i> Actualizar Miércoles';
        }
    });

});
