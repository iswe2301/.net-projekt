"use strict";

// Lägg till händelselyssnare när sidan har laddats
document.addEventListener("DOMContentLoaded", function () {

    // Hämta element från DOM
    const filterForm = document.getElementById("filterForm");
    const searchInput = document.getElementById("searchInput");
    const categorySelect = document.getElementById("categoryId");
    const brandSelect = document.getElementById("brandId");
    const stockStatusSelect = document.getElementById("stockStatus");

    // Om formuläret inte finns, avbryt
    if (!filterForm) {
        return;
    }

    // Kontrollera om sökfältet finns
    if (searchInput) {
        // Lägg till händelselyssnare för sökfältet vid input
        searchInput.addEventListener("input", function () {

    // Rensa filtreringen om sökning pågår
        categorySelect.value = "";
        brandSelect.value = "";
        stockStatusSelect.value = "";
        brandSelect.value = "";

        // Kontrollera om sökfältet är tomt
        if (searchInput.value.trim() === "") {
            filterForm.submit(); // Skicka formuläret för att visa alla produkter
            }
    });

    // Kontrollera om filtreringselement finns
    [categorySelect, brandSelect, stockStatusSelect].forEach(element => {
        if (element) {
                // Lägg till händelselyssnare för filtreringselementen vid ändring
                element.addEventListener("change", function () {
                    searchInput.value = ""; // Rensa sökfältet
                    filterForm.submit(); // Skicka formuläret
                });
            }
        });
    }
});
    