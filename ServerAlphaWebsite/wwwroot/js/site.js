window.scrollToBottom = (elementId) => {
    var element = document.getElementById(elementId);
    if (element) {
        element.scrollTop = element.scrollHeight;
    }
};
function showModal(id) {
    var myModal = new bootstrap.Modal(document.getElementById(id));
    myModal.show();
}

function getPageReferrer() {
    return document.referrer
}

let dotNetHelper = null;

window.initHistoryListener = (dotNetHelperRef) => {
    dotNetHelper = dotNetHelperRef;
    window.addEventListener('popstate', handlePopState);
    console.log("PopState listener initialized.");
};