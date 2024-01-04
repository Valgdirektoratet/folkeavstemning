<script setup lang="ts">
import { useAuthStore } from '@/stores';
const authStore = useAuthStore();
</script>

<template>
    <main class="text-center md:text-left border-box">
        <header class="pt-16 pb-8 sm:pb-10 sm:bg-primary-100">

            <Container>
                <div class="px-4 sm:px-0 relative">
                    <div>
                        <h1 class="text-2xl sm:text-4xl sm:py-1 font-bold text-primary-500" id="main">
                            {{ $t('page.title') }}
                        </h1>
                        <div class="max-w-[85ch] flex flex-col gap-3">
                            <p>{{ $t('landing_view.jumbotron_beskrivelse_del_1') }}</p>
                            <p>{{ $t('landing_view.jumbotron_beskrivelse_del_2') }}</p>
                            <p>
                                <i18n-t keypath="landing_view.jumbotron_beskrivelse_del_3">
                                    <a class="underline hover:text-primary-500 focus:ring-2 py-1 ring-primary-500 outline-none"
                                        href="https://valg.no/folkeavstemning" target="_blank" rounded primary>{{
                                            $t('landing_view.jumbotron_link_tekst') }}</a>
                                </i18n-t>
                            </p>
                        </div>
                    </div>
                    <img src="/img/stemmeseddel.svg"
                        class="mx-auto mt-3 md:mt-0 md:absolute md:top-[50%] md:-translate-y-[50%] md:-left-16 md:w-10"
                        alt="Illustrasjon av stemmeseddel" />
                </div>
            </Container>
        </header>
        <section class="mt-10 sm:px-0 px-4" :aria-label="$t('aria.aktive_folkavstemninger')">
            <Container>
                <AktiveFolkeavstemninger />
                <div class="text-center sm:text-left mt-8"
                    v-if="!authStore.state.isLoggedIn">
                    <StyledLink :href="`/bff/login?returnUrl=${encodeURIComponent('/?sendToFolkeavstemning=true')}`"
                        primary>
                        {{ $t('button.logg_inn') }}
                    </StyledLink>
                </div>
                <div class="mb-10 sm:mb-16 mt-4">
                    <a v-if="!authStore.state.isLoggedIn" class="underline hover:text-primary-500 focus:ring-2 py-1 ring-primary-500 outline-none"
                        href="https://eid.difi.no/nb/id-porten/slik-skaffer-du-deg-elektronisk-id" target="_blank" rounded
                        primary> {{ $t('landing_view.hvordan_skaffe_elektronisk_id') }}</a>
                </div>
            </Container>
        </section>
    </main>
</template>
