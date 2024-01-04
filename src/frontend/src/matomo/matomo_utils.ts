import type {Router} from "vue-router";

export function getMatomo () {
    return window.Piwik.getAsyncTracker()
}

export function loadScript (trackerScript: string, crossOrigin?: string) {
    return new Promise<Event>((resolve, reject) => {
        const script = document.createElement('script')
        script.async = true
        script.defer = true
        script.src = trackerScript

        if (crossOrigin && ['anonymous', 'use-credentials'].includes(crossOrigin)) {
            script.crossOrigin = crossOrigin
        }

        const head = document.head || document.getElementsByTagName('head')[0]

        head.appendChild(script)

        script.onload = resolve
        script.onerror = reject
    })
}

export function getResolvedHref (router: Router, path: string) {
    return router.resolve(path).href
}

export interface Options {
    debug: boolean,
    disableCookies: boolean,
    requireCookieConsent: boolean,
    enableHeartBeatTimer: boolean,
    enableLinkTracking: boolean,
    heartBeatTimerInterval: number,
    requireConsent: boolean,
    trackInitialView: boolean,
    trackerFileName: string,
    trackerUrl?: string,
    trackerScriptUrl?: string,
    userId?: string,
    cookieDomain?: string,
    domains?: string,
    preInitActions: [],
    crossOrigin?: string
    router?: Router
}

declare global {
    export interface Window {
        // biome-ignore lint/suspicious/noExplicitAny: Matomo
        Piwik: any;
        // biome-ignore lint/suspicious/noExplicitAny: Matomo
        _paq: any;
        // biome-ignore lint/suspicious/noExplicitAny: Matomo
        _mtm: any;
    }
}
