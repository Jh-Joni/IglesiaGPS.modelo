// ===== Scripts para la vista ListaCanciones/Index =====

document.addEventListener('DOMContentLoaded', function () {
    var searchInput = document.getElementById('searchInput');

    if (searchInput) {
        searchInput.addEventListener('keyup', function () {
            let filter = this.value.toLowerCase();
            let cards = document.querySelectorAll('.lista-card');

            cards.forEach(function (card) {
                let titulo = card.getAttribute('data-titulo') || "";
                if (titulo.indexOf(filter) > -1) {
                    card.style.display = "";
                } else {
                    card.style.display = "none";
                }
            });
        });
    }
});
