let timeout;

export function onLoad() {
    registerEventListeners();
    startTimeout();
    window.Helpers = Helpers;
}

export function onUpdate() {

}

export function onDispose() {
    unregisterEventListeners();
    clearTimeout(timeout);
}

export function registerEventListeners() {
    document.addEventListener('click', refreshTimeout);
    document.addEventListener('keydown', refreshTimeout);
}

export function unregisterEventListeners() {
    document.removeEventListener('click', refreshTimeout);
    document.removeEventListener('keydown', refreshTimeout);
}

function refreshTimeout() {
    clearTimeout(timeout);
    startTimeout();
}

function startTimeout() {
    timeout = setTimeout(() => {
        Helpers.sessionTimeout();
    }, 7 * 60 * 1000); // 7 minutes
}

class Helpers {
    static dotNetHelper;

    static setDotNetHelper(value) {
        Helpers.dotNetHelper = value;
    }

    static triggerClick(elt) {
        elt.click();
    }

    static async sessionTimeout(tabText) {
        await Helpers.dotNetHelper.invokeMethodAsync('OnSessionTimeout');
    }
}

