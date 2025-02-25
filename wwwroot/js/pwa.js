// Registering av Service Worker för PWA
if ("serviceWorker" in navigator) {
    navigator.serviceWorker.register("/service-worker.js").catch(err => 
        console.log("Service Worker-registrering misslyckades", err)
    );
}