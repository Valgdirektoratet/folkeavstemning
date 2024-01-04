<script setup lang="ts">
import { useAuthStore } from '@/stores';
const authStore = useAuthStore();
</script>

<template>
  <nav class="relative z-20 drop-shadow-md px-8 bg-primary-500 text-white print:hidden h-12 sm:h-16"
    :aria-label="$t('aria.navigasjon')">
    <a href="#main" class="-top-96 absolute focus:top-2 bg-yellow-500 border-2 border-black text-black p-2 rounded-md"
      tabindex="0">
      {{ $t('nav.hopp_til_innhold') }}</a>
    <div class="container mx-auto flex justify-between items-center h-12 sm:h-16">
      <div>
        <router-link to="/"
          class="text-3xl first-letter:uppercase flex items-center gap-4  focus:ring-2 p-1 ring-white outline-none">
          <img src="/img/logo.svg" class="h-6 w-6" alt="Folkeavstemning logo" />
          <p class="hidden md:block" aria-hidden="true">
            {{ $t('page.title') }}
          </p>
        </router-link>
      </div>

      <div class="flex gap-6 uppercase items-center">
        <LanguageSelector />
        <a v-if="authStore.state.isLoggedIn" :href="authStore.state.logoutUrl ?? '/bff/logout'"
          class="flex items-center gap-2 group focus:ring-2 p-1 ring-white outline-none">
          <span class="sm:block hidden font-bold group-hover:underline" aria-hidden="true">{{ $t("nav.logg_ut") }}</span>
          <span :aria-label="$t('aria.logg_ut')" class="material-symbols-outlined text-white">logout</span>
        </a>
      </div>
    </div>
  </nav>
</template>
