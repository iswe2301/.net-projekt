// Service Worker för PWA
const cacheName = "techstock-cache-v1";

// Statiska resurser att cacha
const resourcesToCache = [
    "/css/site.css",
    "/js/site.js",
    "https://iswe2301images.blob.core.windows.net/product-images/icon-192x192.png",
    "https://iswe2301images.blob.core.windows.net/product-images/icon-512x512.png"
];

// Installera Service Worker och cacha statiska resurser
self.addEventListener("install", event => {
    event.waitUntil(
        caches.open(cacheName).then(cache => {
            return cache.addAll(resourcesToCache);
        }).catch(error => console.error("Fel vid cachning:", error))
    );
});

// Rensa gammal cache vid aktivering av ny version
self.addEventListener("activate", event => {
    event.waitUntil(
        caches.keys().then(keys => {
            return Promise.all(
                keys.filter(key => key !== cacheName).map(key => caches.delete(key))
            );
        })
    );
});

// Hämta resurser från cache om de finns, annars från nätverket
self.addEventListener("fetch", event => {
    // Ignorera icke-HTTP requests
    if (!event.request.url.startsWith("http")) return;

    // Exkludera HTML-sidor från cache (för att alltid hämta ny inloggningsstatus)
    if (event.request.mode === "navigate") {
        return fetch(event.request);
    }

    event.respondWith(
        caches.match(event.request).then(cachedResponse => {
            if (cachedResponse) return cachedResponse;

            return fetch(event.request).then(response => {
                return caches.open(cacheName).then(cache => {
                    // Cacha endast giltiga resurser, inte HTML
                    if (response.ok && response.type === "basic") {
                        cache.put(event.request, response.clone());
                    }
                    return response;
                });
            });
        })
    );
});