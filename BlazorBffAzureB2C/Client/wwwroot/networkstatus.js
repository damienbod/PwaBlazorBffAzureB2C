function addOnOfflineEventListener(dotNetObjectRef) {
    window.addEventListener("online",
        (event) => {
            dotNetObjectRef.invokeMethodAsync("OnNetworkStatusChanged", true);
        });

    window.addEventListener("offline",
        (event) => {
            dotNetObjectRef.invokeMethodAsync("OnNetworkStatusChanged", false);
        });
}