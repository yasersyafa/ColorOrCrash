mergeInto(LibraryManager.library, {
  IsMobilebrowser: function () {
    var userAgent = navigator.userAgent || navigator.vendor || window.opera;
    return /android|iphone|ipad|ipod|blackberry|iemobile|opera mini/i.test(
      userAgent.toLowerCase(),
    )
      ? 1
      : 0;
  },
});
