window.cancelFunctionKeys = function (e) {
    if (e.key.startsWith("F")) {
        e.preventDefault();
        return false;
    }
    return true;

}