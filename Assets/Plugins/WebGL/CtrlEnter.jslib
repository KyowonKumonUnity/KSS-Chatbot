mergeInto(LibraryManager.library, {
    InitCtrlEnterListener: function () {
        document.addEventListener("keydown", function (e) {
            if (e.key === "Enter" && e.ctrlKey && !e.isComposing) {
                // unityInstance는 일반적으로 index.html에서 정의됩니다.
                if (typeof unityInstance !== 'undefined') {
                    unityInstance.SendMessage("Controller", "PersonSendButton_OnClick");
                } else if (typeof window.unityInstance !== 'undefined') {
                    window.unityInstance.SendMessage("Controller", "PersonSendButton_OnClick");
                }
            }
        });
    }
});