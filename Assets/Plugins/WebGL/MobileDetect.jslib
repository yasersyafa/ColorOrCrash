mergeInto(LibraryManager.library, {
  IsMobilebrowser: function () {
    // Touch capability check (catches iPadOS 13+ which reports as macOS)
    if (navigator.maxTouchPoints && navigator.maxTouchPoints > 1) return 1;

    // User-agent fallback
    var ua = (navigator.userAgent || navigator.vendor || '').toLowerCase();
    if (/android|iphone|ipad|ipod|blackberry|iemobile|opera mini|mobile|webos|windows phone/.test(ua)) return 1;

    return 0;
  },
});
