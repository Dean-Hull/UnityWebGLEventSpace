mergeInto(LibraryManager.library, {
  NotifyObjectClicked: function(namePtr) {
    var name = UTF8ToString(namePtr);
    console.log('Unity object clicked: ' + name);

    if (window.onUnityObjectSelected) {
        window.onUnityObjectSelected(name);
    }
  }
});