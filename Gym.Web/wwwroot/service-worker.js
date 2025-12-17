self.addEventListener('install', e => {
    e.waitUntil(
        caches.open('gym-cache').then(cache => {
            return cache.addAll(['/']);
        })
    );
});
