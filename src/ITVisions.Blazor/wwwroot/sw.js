self.addEventListener('install', function (event) {
 console.log('Service Worker installiert');
 self.skipWaiting();
});

self.addEventListener('activate', function (event) {
 console.log('Service Worker aktiviert');
});

self.addEventListener('notificationclick', function (event) {
 event.notification.close();

 if (event.action === 'open') {
  clients.openWindow('/'); // Die gewünschte URL setzen
 }
});
