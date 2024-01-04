<script setup lang="ts">
import { useI18n } from 'vue-i18n';
import { onClickOutside, useLocalStorage } from '@vueuse/core'
import { ref, watchEffect } from 'vue';
import { useRoute } from 'vue-router';
import { useFolkeavstemningStore } from '@/stores';
import { OpenAPI } from '@/api/folkeavstemning';

const { locale, availableLocales } = useI18n()

const expanded = ref(false);
const target = ref(null);
onClickOutside(target, () => { expanded.value = false });

const getLanguageFromBrowser = () => {
    let langFromBrowser = navigator.languages.find((lang) => ['nb'].find((l) => lang.startsWith(l))) || 'nb'
    return convertToSupportedLanguage(langFromBrowser)
}

const convertToSupportedLanguage = (lang: string) => {
    if (lang.startsWith('nb')) return 'nb'
    if (lang.startsWith('nn')) return 'nn'
    return 'nb'
}

const selectedLanguage = useLocalStorage('language', getLanguageFromBrowser())
const route = useRoute()
if (route.query && route.query.lang) selectedLanguage.value = route.query.lang.toString()

watchEffect(() => {
    locale.value = convertToSupportedLanguage(selectedLanguage.value);
})

const folkeavstemingStore = useFolkeavstemningStore();
const setLanguage = async (value: string) => {
    OpenAPI.HEADERS = {
        "x-csrf": "1",
	    "Accept-Language": value,
    }
    await folkeavstemingStore.loadFolkeavstemningerAsync();
    selectedLanguage.value = value;
}

</script>
<template>
    <div class="relative" ref="target">
        <button @click="expanded = !expanded"
            class="flex items-center tracking-wider focus:ring-2 p-1 ring-white outline-none uppercase font-bold group">
            <span class="group-hover:underline underline-offset-4">{{ $t('språkvalg.tittel') }}</span> <span
                aria-hidden="true" class="transition-transform material-symbols-outlined"
                :class="{ 'rotate-180': expanded }">expand_more</span>
        </button>
        <div v-if="expanded"
            class="absolute top-10 right-0 bg-white border-gray-10 border w-40 cursor-default select-none gap-2 py-2 flex flex-col">
            <button v-for="locale in availableLocales" @click="setLanguage(locale)"
                class="py-2 px-4 cursor-pointer text-primary-600 border-2 hover:border-primary-500 border-transparent focus:ring-2 ring-primary-600 m-2 outline-none flex gap-2">
                <span>{{ $t('språkvalg.språk.' + locale) }}</span>
                <span v-if="selectedLanguage === locale" aria-hidden="true"
                    class="absolute right-2 material-symbols-outlined">check</span></button>
        </div>
    </div>
</template>