// Service Worker för PWA
const cacheName = "techstock-cache-v2";

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
    // Ignorera POST-förfrågningar helt så att inga dubbla anrop sker 
    if (event.request.method === "POST") {
        return;
    }
    // Ignorera navigeringsförfrågningar (HTML-sidor) så att inloggningsstatus alltid är aktuell
    if (event.request.mode === "navigate") {
        return;
    }
    // Ignorera icke-HTTP requests
    if (!event.request.url.startsWith("http")) return;

    event.respondWith(
        caches.match(event.request).then(cachedResponse => {
            if (cachedResponse) return cachedResponse;

            return fetch(event.request).then(response => {
                return caches.open(cacheName).then(cache => {
                    // Cacha endast statiska resurser (inte HTML eller POST)
                    if (response.ok && response.type === "basic" && event.request.method === "GET") {
                        cache.put(event.request, response.clone());
                    }
                    return response;
                });
            });
        })
    );
});