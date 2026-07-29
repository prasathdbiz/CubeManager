function ScrollTo(elementId) {
    var element = document.getElementById(elementId);
    element.scrollIntoView({
        behavior: 'smooth'
    });
}

function RefreshPdf(elementId) {
    document.getElementById(elementId).contentWindow.location.reload();
}


