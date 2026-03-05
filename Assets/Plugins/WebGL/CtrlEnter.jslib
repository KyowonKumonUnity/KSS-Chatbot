mergeInto(LibraryManager.library, {});

(function waitForUnity() {

    if (typeof unityInstance === "undefined") {
        setTimeout(waitForUnity, 100);
        return;
    }

    document.addEventListener("keydown", function (e) {
        if (e.key === "Enter" && e.ctrlKey && !e.isComposing) {
            unityInstance.SendMessage("Controller", "PersonSendButton_OnClick");
        }
    });

})();