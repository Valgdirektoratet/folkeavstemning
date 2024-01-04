/**
 * Klonet og modifisert til vårt bruk fra https://github.com/AmazingDreams/vue-matomo
 */

import type {App} from "vue";
import type {RouteLocationNormalized,_RouteLocationBase } from "vue-router";
import {type Options,getMatomo, getResolvedHref, loadScript } from './matomo_utils'

const defaultOptions: Options = {
    debug: false,
    disableCookies: true,
    requireCookieConsent: false,
    enableHeartBeatTimer: true,
    enableLinkTracking: true,
    heartBeatTimerInterval: 15,
    requireConsent: false,
    trackInitialView: true,
    trackerFileName: 'matomo',
    trackerUrl: undefined,
    trackerScriptUrl: undefined,
    userId: undefined,
    cookieDomain: undefined,
    domains: undefined,
    preInitActions: [],
    crossOrigin: undefined
}

export const matomoKey = 'Matomo'

export enum Category {
    ERROR = "Error",
    USER_INPUT = "UserInput",
}

export function trackEvent (category: Category, action: string, name?: string, value?: number) {
    try {
        window._paq.push(['trackEvent', category, action, name, value]);
    } catch (e) {
        if (import.meta.env.DEV) {
            console.log(e);
        }
    }
}

function trackMatomoPageView (options: Options, to: _RouteLocationBase, from?: _RouteLocationBase) {
    const Matomo = getMatomo()

    let title
    let url
    let referrerUrl

    if (options.router) {
        url = getResolvedHref(options.router, to.fullPath)
        referrerUrl = from?.fullPath
            ? getResolvedHref(options.router, from.fullPath)
            : undefined

        if (to.meta.analyticsIgnore) {
            options.debug && console.debug(`[vue-matomo] Ignoring ${url}`)
            return
        }

        options.debug && console.debug(`[vue-matomo] Tracking ${url}`)
        title = to.meta.title || url
    }

    if (referrerUrl) {
        Matomo.setReferrerUrl(window.location.origin + referrerUrl)
    }
    if (url) {
        Matomo.setCustomUrl(window.location.origin + url)
    }

    Matomo.trackPageView(title)
}

function initMatomo (Vue: App, options: Options) {
    const Matomo = getMatomo()

    Vue.config.globalProperties.$piwik = Matomo
    Vue.config.globalProperties.$matomo = Matomo
    Vue.provide(matomoKey, Matomo)

    if (options.trackInitialView && options.router) {
        
        const currentRoute = options.router.currentRoute.value;

        // Register first page view
        trackMatomoPageView(options, currentRoute)
    }

    // Track page navigations if router is specified
    if (options.router) {
        options.router.afterEach((to: RouteLocationNormalized, from: RouteLocationNormalized) => {
            trackMatomoPageView(options, to, from)

            if (options.enableLinkTracking) {
                Matomo.enableLinkTracking()
            }
        })
    }
}

function piwikExists () {
    // In case of TMS,  we load a first container_XXX.js which triggers aynchronously the loading of the standard Piwik.js
    // this will avoid the error throwed in initMatomo when window.Piwik is undefined
    // if window.Piwik is still undefined when counter reaches 3000ms we reject and go to error

    return new Promise<void>((resolve, reject) => {
        const checkInterval = 50
        const timeout = 3000
        const waitStart = Date.now()

        const interval = setInterval(() => {
            if (window.Piwik) {
                clearInterval(interval)

                return resolve()
            }

            if (Date.now() >= waitStart + timeout) {
                clearInterval(interval)

                throw new Error(`[vue-matomo]: window.Piwik undefined after waiting for ${timeout}ms`)
            }
        }, checkInterval)
    })
}

/**
 * Ved oppsett av matomo plugin kan man velge om man vil bruke en statisk siteId eller knytte seg mot en tag manager som styrer siteId via en lookup table
 * Hvis man oppgir både siteId og containerId vil siteId overskrive evt siteId fra tag manageren.
 */

// biome-ignore lint/suspicious/noExplicitAny: <explanation>
export  default function install (Vue: App, setupOptions: {[key: string]: any} = {}) {
    const options = Object.assign({}, defaultOptions, setupOptions)
    
    //disable for development
    if (import.meta.env.DEV && !options.debug) {
        console.warn("Disabled Matomo for dev");
        return;
    }

    const { host, siteId, containerId, trackerFileName, trackerUrl, trackerScriptUrl, tagManagerScriptUrl } = options
    
    const trackerEndpoint = trackerUrl || `${host}/${trackerFileName}.php`
    const trackerScript = trackerScriptUrl || `${host}/${trackerFileName}.js`
    
    const tagManagerScript = tagManagerScriptUrl || `${host}/js/container_${containerId}.js`;

    window._mtm = window._mtm || [];
    window._mtm.push({'mtm.startTime': (new Date().getTime()), event: 'mtm.Start'});
    
    window._paq = window._paq || []
    window._paq.push(['setTrackerUrl', trackerEndpoint])
    
    if(siteId) {
        window._paq.push(['setSiteId', siteId])
    }

    if (options.requireConsent) {
        window._paq.push(['requireConsent'])
    }

    if (options.userId) {
        window._paq.push(['setUserId', options.userId])
    }

    if (options.enableLinkTracking) {
        window._paq.push(['enableLinkTracking'])
    }

    if (options.disableCookies) {
        window._paq.push(['disableCookies'])
    }

    if (options.requireCookieConsent) {
        window._paq.push(['requireCookieConsent'])
    }

    if (options.enableHeartBeatTimer) {
        window._paq.push(['enableHeartBeatTimer', options.heartBeatTimerInterval])
    }

    if (options.cookieDomain) {
        window._paq.push(['setCookieDomain', options.cookieDomain])
    }

    if (options.domains) {
        window._paq.push(['setDomains', options.domains])
    }

    // biome-ignore lint/complexity/noForEach: Wont change
    options.preInitActions.forEach((action) => window._paq.push(action))
    
    const scriptToLoad = containerId ? tagManagerScript : trackerScript
    loadScript(scriptToLoad, options.crossOrigin)
            .then(() => piwikExists())
            .then(() => initMatomo(Vue, options))
        .catch((error) => {
            if (error.target) {
                return console.error(
                    `[vue-matomo] An error occurred trying to load ${error.target.src}. If the file exists you may have an ad- or trackingblocker enabled.`
                )
            }

            console.error(error)
        })
}