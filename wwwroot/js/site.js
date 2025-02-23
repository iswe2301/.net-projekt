"use strict";

// Lägg till händelselyssnare när sidan har laddats
document.addEventListener("DOMContentLoaded", function () {

    // Hämta element från DOM
    const filterForm = document.getElementById("filterForm");
    const searchInput = document.getElementById("searchInput");
    const categorySelect = document.getElementById("categoryId");
    const brandSelect = document.getElementById("brandId");
    const stockStatusSelect = document.getElementById("stockStatus");
    const existingImage = document.getElementById("existing-image");
    const removeImageBtn = document.getElementById("remove-image-btn");

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
    // Kontrollera om det redan finns en bild och visa "Ta bort bild"-knappen
    if (existingImage && existingImage.value.trim() !== "") {
        removeImageBtn.classList.remove("d-none");
    }
});

// Funktion för att förhandsvisa bild
function previewImage(event) {

    // Hämta element från DOM
    const imagePreview = document.getElementById("image-preview");
    const removeImageBtn = document.getElementById("remove-image-btn");

    // Hämtar filen som användaren valt
    let file = event.target.files[0];

    // Kontrollera om filen finns
    if (file) {
        // Skapa en file reader
        let reader = new FileReader();
        // När filen är inläst
        reader.onload = function (e) {
            // Sätt src-attributet till filens data-url
            imagePreview.src = e.target.result;
            // Ta bort klassen för att visa bilden
            imagePreview.classList.remove("d-none");
            // Visa knappen för att ta bort bilden (om knappen finns)
            if (removeImageBtn) {
                removeImageBtn.classList.remove("d-none");
            }
        }
        reader.readAsDataURL(file); // Läs in filen som en data-url
    }
}

// Funktion för att ta bort bild
function removeImage() {

    // Hämta element från DOM
    const imagePreview = document.getElementById("image-preview");
    const removeImageBtn = document.getElementById("remove-image-btn");
    const existingImage = document.getElementById("existing-image");
    const fileInput = document.getElementById("ImageFile");

    // Återställ till placeholder
    imagePreview.src = "https://iswe2301images.blob.core.windows.net/product-images/placeholder.png";
    // Rensa input för bild
    fileInput.value = "";
    // Dölj bilden
    existingImage.value = "";
    // Dölj knappen
    removeImageBtn.classList.add("d-none");
    // Dölj bilden
    imagePreview.classList.add("d-none");
}